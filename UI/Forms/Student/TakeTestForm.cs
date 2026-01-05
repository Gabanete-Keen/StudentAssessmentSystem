using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentAssessmentSystem.BusinessLogic.Services;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Results;
using StudentAssessmentSystem.Utilities;

namespace StudentAssessmentSystem.UI.Forms.Student
{
    
    /// FORM PURPOSE: Allows students to take a test by displaying questions one at a time
    public partial class TakeTestForm : Form
    {
        
        // SECTION 1: PRIVATE FIELDS (Data the form needs to work)
       

        private Test _test;                              // The test being taken (contains all questions)
        private List<Question> _questions;               // All questions in this test
        private int _currentQuestionIndex;               // Which question we're currently showing (0-based index)
        private Dictionary<int, int> _studentAnswers;    // Stores student's selected answers: QuestionId -> ChoiceId
        private DateTime _testStartTime;                 // When student started the test
        private TestResult _testResult;                  // Database record tracking this test attempt

        // Repositories (for database access)
        private TestRepository _testRepository;
        private TestResultRepository _resultRepository;
        private StudentAnswerRepository _answerRepository;
        private ScoringService _scoringService;

        // UI Controls (visual elements on the form)
        private Label lblQuestionNumber;                 // Shows "Question 1 of 5"
        private Label lblQuestionText;                   // Shows the actual question
        private GroupBox gbChoices;                      // Container for radio buttons
        private List<RadioButton> _choiceButtons;        // Radio buttons for answer choices
        private Button btnPrevious;                      // Go to previous question
        private Button btnNext;                          // Go to next question
        private Button btnSubmit;                        // Submit the test
        private ProgressBar progressBar;                 // Shows test completion progress
        private Label lblTimer;                          // Shows elapsed time
        private Timer timer;                             // Updates the timer display

        
        // SECTION 2: CONSTRUCTOR (Sets up the form when created)
        
        /// CONSTRUCTOR: Called when you create the form like: new TakeTestForm(testId, instanceId)
        public TakeTestForm(int testId, int instanceId)
        {
            // Initialize repositories 
            _testRepository = new TestRepository();
            _resultRepository = new TestResultRepository();
            _answerRepository = new StudentAnswerRepository();
            _scoringService = new ScoringService();

            // Initialize the dictionary that stores student answers
            // Key = QuestionId, Value = ChoiceId
            _studentAnswers = new Dictionary<int, int>();

            // Record when the test started
            _testStartTime = DateTime.Now;

            // Build the UI
            InitializeComponent();

            // Load the test data from database
            LoadTest(testId, instanceId);

            // Create a database record for this test attempt
            CreateTestResult(instanceId);

            // Show the first question
            DisplayQuestion(_currentQuestionIndex);

            // Start the timer
            StartTimer();
        }

       
        // SECTION 3: UI INITIALIZATION (Creates all buttons, labels, etc.)
        private void InitializeComponent()
        {
            
            this.Text = "Take Test";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;  
            this.MaximizeBox = false;                            

            int yPos = 20;  

            // ===== HEADER: Timer Label (top right) =====
            lblTimer = new Label
            {
                Location = new Point(750, yPos),
                Size = new Size(120, 25),
                Text = "Time: 00:00",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleRight
            };
            this.Controls.Add(lblTimer);

            // ===== HEADER: Question Number Label =====
            lblQuestionNumber = new Label
            {
                Location = new Point(30, yPos),
                Size = new Size(300, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(lblQuestionNumber);
            yPos += 40;

            // ===== PROGRESS BAR (shows how many questions completed) =====
            progressBar = new ProgressBar
            {
                Location = new Point(30, yPos),
                Size = new Size(840, 20),
                Style = ProgressBarStyle.Continuous
            };
            this.Controls.Add(progressBar);
            yPos += 35;

            // ===== QUESTION TEXT (the actual question being asked) =====
            lblQuestionText = new Label
            {
                Location = new Point(30, yPos),
                Size = new Size(840, 100),
                Font = new Font("Arial", 12),
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(15),
                AutoSize = false
            };
            this.Controls.Add(lblQuestionText);
            yPos += 115;

            // ===== ANSWER CHOICES GROUP BOX (container for radio buttons) =====
            gbChoices = new GroupBox
            {
                Location = new Point(30, yPos),
                Size = new Size(840, 280),
                Text = "Select your answer:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };
            this.Controls.Add(gbChoices);
            yPos += 295;

            // Initialize list to hold radio buttons (one for each choice)
            _choiceButtons = new List<RadioButton>();

            // ===== NAVIGATION BUTTONS =====

            // PREVIOUS BUTTON (go back to previous question)
            btnPrevious = new Button
            {
                Location = new Point(30, yPos),
                Size = new Size(150, 40),
                Text = "⬅ Previous",
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.LightGray,
                Cursor = Cursors.Hand,
                Enabled = false  // Disabled on first question
            };
            btnPrevious.Click += BtnPrevious_Click;  // Wire up the click event
            this.Controls.Add(btnPrevious);

            // NEXT BUTTON (go to next question)
            btnNext = new Button
            {
                Location = new Point(570, yPos),
                Size = new Size(150, 40),
                Text = "Next ➡",
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.LightBlue,
                Cursor = Cursors.Hand
            };
            btnNext.Click += BtnNext_Click;
            this.Controls.Add(btnNext);

            // SUBMIT BUTTON (finish and submit test)
            btnSubmit = new Button
            {
                Location = new Point(730, yPos),
                Size = new Size(140, 40),
                Text = "Submit Test",
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.LightGreen,
                Cursor = Cursors.Hand,
                Visible = false  // Only show on last question
            };
            btnSubmit.Click += BtnSubmit_Click;
            this.Controls.Add(btnSubmit);

            // ===== TIMER (updates every second) =====
            timer = new Timer
            {
                Interval = 1000  // 1000 milliseconds = 1 second
            };
            timer.Tick += Timer_Tick;  // This method runs every second
        }

        
        // SECTION 4: DATA LOADING (Gets test from database)

        /// LOADS TEST DATA from database
        /// Gets the test and all its questions
        private void LoadTest(int testId, int instanceId)
        {
            try
            {
                // Get test from database
                _test = _testRepository.GetTestById(testId);

                if (_test == null || _test.Questions == null || _test.Questions.Count == 0)
                {
                    MessageBox.Show("Test not found or has no questions.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Get all questions for this test
                _questions = _test.Questions.OrderBy(q => q.OrderNumber).ToList();

                // Set progress bar maximum (total questions)
                progressBar.Maximum = _questions.Count;
                progressBar.Value = 0;

                // Start at first question
                _currentQuestionIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading test:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

       
        // SECTION 5: TEST RESULT CREATION (Database record tracking)

       
        /// CREATES A TEST RESULT RECORD in database
        /// This tracks that the student is taking the test
        private void CreateTestResult(int instanceId)
        {
            try
            {
                int studentId = SessionManager.GetCurrentUserId();

                // Create new test result object
                _testResult = new TestResult
                {
                    InstanceId = instanceId,
                    StudentId = studentId,
                    StartTime = _testStartTime,
                    IsCompleted = false,  // Not finished yet
                    TotalPoints = _test.TotalPoints,
                    RawScore = 0          // Will be calculated on submit
                };

                // Save to database and get the ResultId back
                _testResult.ResultId = _resultRepository.Add(_testResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating test record:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        
        // SECTION 6: QUESTION DISPLAY (Shows current question)

        /// DISPLAYS THE QUESTION at the given index
        /// Creates radio buttons for each answer choice
        private void DisplayQuestion(int questionIndex)
        {
            // Get the current question
            Question currentQuestion = _questions[questionIndex];

            // Update question number label (e.g., "Question 1 of 5")
            lblQuestionNumber.Text = $"Question {questionIndex + 1} of {_questions.Count}";

            // Display the question text
            lblQuestionText.Text = currentQuestion.QuestionText;

            // Update progress bar
            progressBar.Value = questionIndex;

            // Clear old radio buttons
            gbChoices.Controls.Clear();
            _choiceButtons.Clear();

            // Get this question as a multiple choice question (casting)
            var mcQuestion = currentQuestion as MultipleChoiceQuestion;
            if (mcQuestion == null || mcQuestion.Choices == null)
            {
                MessageBox.Show("Invalid question format.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // CREATE RADIO BUTTONS for each choice
            int yPosition = 40;
            foreach (var choice in mcQuestion.Choices.OrderBy(c => c.OrderNumber))
            {
                RadioButton rbChoice = new RadioButton
                {
                    Location = new Point(30, yPosition),
                    Size = new Size(780, 50),
                    Text = choice.ChoiceText,
                    Font = new Font("Arial", 11),
                    Tag = choice.ChoiceId,  // Store choiceId in Tag property
                    Cursor = Cursors.Hand,
                    AutoSize = false
                };

                // If student already answered this question, pre-select it
                if (_studentAnswers.ContainsKey(currentQuestion.QuestionId))
                {
                    if (_studentAnswers[currentQuestion.QuestionId] == choice.ChoiceId)
                    {
                        rbChoice.Checked = true;
                    }
                }

                gbChoices.Controls.Add(rbChoice);
                _choiceButtons.Add(rbChoice);
                yPosition += 55;
            }

            // UPDATE BUTTON VISIBILITY
            // Previous button: disable on first question
            btnPrevious.Enabled = (questionIndex > 0);

            // Next button: hide on last question
            // Submit button: show on last question
            if (questionIndex == _questions.Count - 1)
            {
                btnNext.Visible = false;
                btnSubmit.Visible = true;
            }
            else
            {
                btnNext.Visible = true;
                btnSubmit.Visible = false;
            }
        }

        
        // SECTION 7: ANSWER SAVING (Stores student's selection)

        /// SAVES THE CURRENT ANSWER before moving to next question
        private void SaveCurrentAnswer()
        {
            Question currentQuestion = _questions[_currentQuestionIndex];

            // Find which radio button is checked
            RadioButton selectedButton = _choiceButtons.FirstOrDefault(rb => rb.Checked);

            if (selectedButton != null)
            {
                // Get the ChoiceId from the Tag property
                int selectedChoiceId = (int)selectedButton.Tag;

                // Store or update the answer in dictionary
                _studentAnswers[currentQuestion.QuestionId] = selectedChoiceId;
            }
        }

       
        // SECTION 8: NAVIGATION EVENTS (Button clicks)
       
        /// PREVIOUS BUTTON CLICK: Go back to previous question
        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            // Save current answer first
            SaveCurrentAnswer();

            // Move to previous question
            _currentQuestionIndex--;

            // Display that question
            DisplayQuestion(_currentQuestionIndex);
        }

        
        /// NEXT BUTTON CLICK: Move to next question
        private void BtnNext_Click(object sender, EventArgs e)
        {
            // Check if an answer was selected
            if (!_choiceButtons.Any(rb => rb.Checked))
            {
                MessageBox.Show("Please select an answer before proceeding.", "Answer Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Save current answer
            SaveCurrentAnswer();

            // Move to next question
            _currentQuestionIndex++;

            // Display that question
            DisplayQuestion(_currentQuestionIndex);
        }

        /// SUBMIT BUTTON CLICK: Finish test and calculate score
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            // Confirm submission
            var result = MessageBox.Show(
                "Are you sure you want to submit your test?\n\n" +
                "You cannot change your answers after submission.",
                "Confirm Submission",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            // Save the last answer
            SaveCurrentAnswer();

            // Stop timer
            timer.Stop();

            // Submit to database
            SubmitTest();
        }

        
        // SECTION 9: TEST SUBMISSION (Final save and scoring)
       
        /// SUBMITS THE TEST: Saves all answers and calculates score
        /// saves everything to database
        private void SubmitTest()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Update test result with completion time
                _testResult.SubmitTime = DateTime.Now;
                _testResult.IsCompleted = true;

                int totalCorrect = 0;
                int totalPoints = 0;

                // SAVE EACH ANSWER TO DATABASE
                foreach (var questionId in _studentAnswers.Keys)
                {
                    // Get the question
                    var question = _questions.First(q => q.QuestionId == questionId);
                    var mcQuestion = question as MultipleChoiceQuestion;

                    // Get the selected choice
                    int selectedChoiceId = _studentAnswers[questionId];
                    var selectedChoice = mcQuestion.Choices.First(c => c.ChoiceId == selectedChoiceId);

                    // Check if correct
                    bool isCorrect = selectedChoice.IsCorrect;
                    int pointsEarned = isCorrect ? question.PointValue : 0;

                    if (isCorrect)
                        totalCorrect++;

                    totalPoints += pointsEarned;

                    // CREATE STUDENT ANSWER RECORD
                    StudentAnswer answer = new StudentAnswer
                    {
                        ResultId = _testResult.ResultId,
                        QuestionId = questionId,
                        SelectedChoiceId = selectedChoiceId,
                        IsCorrect = isCorrect,
                        PointsEarned = pointsEarned,
                        TimeSpentSeconds = 0  
                    };

                    // Save to database
                    _answerRepository.Add(answer);
                }

                // CALCULATE FINAL SCORE
                _testResult.RawScore = totalPoints;
                _testResult.Percentage = (_testResult.TotalPoints > 0)
                    ? (decimal)totalPoints / _testResult.TotalPoints * 100
                    : 0;

                // Determine letter grade and pass/fail
                _testResult.LetterGrade = CalculateLetterGrade(_testResult.Percentage);
                _testResult.Passed = _testResult.Percentage >= _test.PassingScore;

                // UPDATE TEST RESULT in database
                _resultRepository.Update(_testResult);

                this.Cursor = Cursors.Default;

                // SHOW SUCCESS MESSAGE
                MessageBox.Show(
                    $"Test submitted successfully!\n\n" +
                    $"Score: {totalPoints} / {_testResult.TotalPoints}\n" +
                    $"Percentage: {_testResult.Percentage:F2}%\n" +
                    $"Grade: {_testResult.LetterGrade}\n" +
                    $"Status: {(_testResult.Passed ? "PASSED ✓" : "FAILED ✗")}",
                    "Test Completed",
                    MessageBoxButtons.OK,
                    _testResult.Passed ? MessageBoxIcon.Information : MessageBoxIcon.Warning
                );

                // Close form and return to dashboard
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error submitting test:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        // SECTION 10: HELPER METHODS (Supporting functions)
        
        /// CALCULATES LETTER GRADE from percentage
        private string CalculateLetterGrade(decimal percentage)
        {
            if (percentage >= 90) return "A";
            if (percentage >= 80) return "B";
            if (percentage >= 70) return "C";
            if (percentage >= 60) return "D";
            return "F";
        }


        /// TIMER TICK EVENT: Updates the elapsed time display every second
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Calculate time remaining (not elapsed)
            TimeSpan elapsed = DateTime.Now - _testStartTime;
            int totalSeconds = _test.DurationMinutes * 60;
            int secondsRemaining = totalSeconds - (int)elapsed.TotalSeconds;

            // Check if time is up
            if (secondsRemaining <= 0)
            {
                timer.Stop();
                lblTimer.Text = "Time: 00:00";
                lblTimer.ForeColor = Color.Red;

                MessageBox.Show(
                    "Time's up! Your test will be submitted automatically.",
                    "Time Expired",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                // Auto-submit
                SubmitTest();
                return;
            }

            // Display remaining time (counting DOWN)
            int minutes = secondsRemaining / 60;
            int seconds = secondsRemaining % 60;
            lblTimer.Text = $"Time: {minutes:D2}:{seconds:D2}";

            // Change color when less than 5 minutes remain
            if (secondsRemaining <= 300) // 5 minutes
            {
                lblTimer.ForeColor = Color.Red;
                lblTimer.Font = new Font(lblTimer.Font, FontStyle.Bold);
            }
            else if (secondsRemaining <= 600) // 10 minutes
            {
                lblTimer.ForeColor = Color.Orange;
            }
            else
            {
                lblTimer.ForeColor = Color.DarkBlue;
            }
        }



        /// STARTS THE TIMER
        private void StartTimer()
        {
            timer.Start();
        }

        
        /// FORM CLOSING EVENT: Warn if user tries to close without submitting
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_testResult != null && !_testResult.IsCompleted)
            {
                var result = MessageBox.Show(
                    "You haven't submitted your test yet!\n\n" +
                    "Are you sure you want to exit? Your progress will be lost.",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.No)
                {
                    e.Cancel = true;  // Cancel the close
                }
            }

            base.OnFormClosing(e);
        }
    }
}
