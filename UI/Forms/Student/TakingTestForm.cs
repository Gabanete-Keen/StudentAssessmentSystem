using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Results;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentModel = StudentAssessmentSystem.Models.Users.Student;

namespace StudentAssessmentSystem.UI.Forms.Student
{
    /// <summary>
    /// Form for students to take tests
    /// Shows one question at a time with timer
    /// </summary>
    public partial class TestTakingForm : Form
    {
        private StudentModel _currentStudent;
        private int _instanceId;
        private TestResult _currentResult;
        private List<MultipleChoiceQuestion> _questions;
        private Dictionary<int, int> _studentAnswers;
        private int _currentQuestionIndex;
        private Timer _testTimer;
        private int _remainingSeconds;
        private bool _isSubmitted = false;

        // UI Controls
        private Label lblTestTitle;
        private Label lblQuestionNumber;
        private Label lblTimer;
        private Label lblQuestionText;
        private Panel pnlChoices;
        private Button btnPrevious;
        private Button btnNext;
        private Button btnSubmit;
        private ProgressBar progressBar;

        private TestTakingManager _testTakingManager;

        public TestTakingForm(StudentModel student, int instanceId)
        {
            _currentStudent = student;
            _instanceId = instanceId;
            _studentAnswers = new Dictionary<int, int>();
            _currentQuestionIndex = 0;
            _testTakingManager = new TestTakingManager();

            InitializeComponent();
            LoadTest();
        }

        private void InitializeComponent()
        {
            this.Text = "Test Taking";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Header Panel
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(52, 152, 219),
                Padding = new Padding(20)
            };

            lblTestTitle = new Label
            {
                Text = "Loading Test...",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            lblTimer = new Label
            {
                Text = "Time: 60:00",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.Yellow,
                Location = new Point(700, 15),
                Size = new Size(150, 30)
            };

            headerPanel.Controls.AddRange(new Control[] { lblTestTitle, lblTimer });
            this.Controls.Add(headerPanel);

            // Progress Panel
            Panel progressPanel = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(900, 50),
                BackColor = Color.LightGray,
                Padding = new Padding(20, 10, 20, 10)
            };

            lblQuestionNumber = new Label
            {
                Text = "Question 1 of 10",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            progressBar = new ProgressBar
            {
                Location = new Point(200, 15),
                Size = new Size(660, 20),
                Minimum = 0,
                Maximum = 100
            };

            progressPanel.Controls.AddRange(new Control[] { lblQuestionNumber, progressBar });
            this.Controls.Add(progressPanel);

            // Question Panel
            Panel questionPanel = new Panel
            {
                Location = new Point(20, 150),
                Size = new Size(840, 400),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            lblQuestionText = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(800, 100),
                Font = new Font("Arial", 12, FontStyle.Regular),
                Text = "Question will appear here..."
            };

            pnlChoices = new Panel
            {
                Location = new Point(20, 130),
                Size = new Size(800, 250),
                AutoScroll = true
            };

            questionPanel.Controls.AddRange(new Control[] { lblQuestionText, pnlChoices });
            this.Controls.Add(questionPanel);

            // Navigation Buttons
            btnPrevious = new Button
            {
                Text = "◄ Previous",
                Location = new Point(20, 570),
                Size = new Size(120, 40),
                Font = new Font("Arial", 10, FontStyle.Bold),
                Enabled = false
            };
            btnPrevious.Click += BtnPrevious_Click;

            btnNext = new Button
            {
                Text = "Next ►",
                Location = new Point(640, 570),
                Size = new Size(120, 40),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightGreen
            };
            btnNext.Click += BtnNext_Click;

            btnSubmit = new Button
            {
                Text = "Submit Test",
                Location = new Point(770, 570),
                Size = new Size(120, 40),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.Orange,
                Visible = false
            };
            btnSubmit.Click += BtnSubmit_Click;

            this.Controls.AddRange(new Control[] { btnPrevious, btnNext, btnSubmit });

            // Timer
            _testTimer = new Timer();
            _testTimer.Interval = 1000;
            _testTimer.Tick += TestTimer_Tick;

            // Form Closing Event
            this.FormClosing += TestTakingForm_FormClosing;
        }

        private void LoadTest()
        {
            try
            {
                // Get test instance details
                string errorMessage;
                TestInstance instance = _testTakingManager.GetTestInstanceWithDetails(_instanceId);

                if (instance == null)
                {
                    MessageBox.Show($"Test instance not found. Please contact your instructor.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                if (instance.TestId <= 0)
                {
                    MessageBox.Show("Invalid test configuration. Please contact your instructor.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                lblTestTitle.Text = instance.InstanceTitle ?? "Test";

                // Start test session
                _currentResult = _testTakingManager.StartTestSession(_currentStudent.UserId, _instanceId, out errorMessage);

                if (_currentResult == null)
                {
                    MessageBox.Show($"Failed to start test session. Please try again.\n\n{errorMessage}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Load questions
                _questions = _testTakingManager.LoadTestQuestions(instance.TestId, out errorMessage);

                if (_questions == null || _questions.Count == 0)
                {
                    MessageBox.Show("No questions found for this test. Please contact your instructor.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Set timer
                _remainingSeconds = instance.DurationMinutes * 60;
                UpdateTimerDisplay();
                _testTimer.Start();

                // Display first question
                DisplayQuestion(_currentQuestionIndex);
            }
            catch (FormatException fex)
            {
                MessageBox.Show($"Data format error: {fex.Message}\n\nPlease contact your instructor.",
                    "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading test: {ex.Message}\n\nPlease try again or contact your instructor.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void DisplayQuestion(int questionIndex)
        {
            if (questionIndex < 0 || questionIndex >= _questions.Count)
                return;

            MultipleChoiceQuestion question = _questions[questionIndex];

            // Update question number and progress
            lblQuestionNumber.Text = $"Question {questionIndex + 1} of {_questions.Count}";
            progressBar.Value = (int)(((double)(questionIndex + 1) / _questions.Count) * 100);

            // Display question text
            lblQuestionText.Text = $"{questionIndex + 1}. {question.QuestionText}";

            // Clear previous choices
            pnlChoices.Controls.Clear();

            // Display choices
            int yPosition = 10;
            if (question.Choices != null)
            {
                foreach (var choice in question.Choices.OrderBy(c => c.OrderNumber))
                {
                    RadioButton rbChoice = new RadioButton
                    {
                        Text = $"{choice.ChoiceLetter}. {choice.ChoiceText}",
                        Tag = choice.ChoiceId,
                        Location = new Point(20, yPosition),
                        Size = new Size(750, 40),
                        Font = new Font("Arial", 11),
                        AutoSize = false
                    };

                    // Check if student already answered this question
                    if (_studentAnswers.ContainsKey(question.QuestionId))
                    {
                        if (_studentAnswers[question.QuestionId] == choice.ChoiceId)
                        {
                            rbChoice.Checked = true;
                        }
                    }

                    rbChoice.CheckedChanged += (s, e) =>
                    {
                        if (rbChoice.Checked)
                        {
                            _studentAnswers[question.QuestionId] = choice.ChoiceId;
                        }
                    };

                    pnlChoices.Controls.Add(rbChoice);
                    yPosition += 50;
                }
            }

            // Update navigation buttons
            btnPrevious.Enabled = questionIndex > 0;

            bool isLastQuestion = questionIndex == _questions.Count - 1;
            btnNext.Visible = !isLastQuestion;
            btnSubmit.Visible = isLastQuestion;
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentQuestionIndex > 0)
            {
                _currentQuestionIndex--;
                DisplayQuestion(_currentQuestionIndex);
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_currentQuestionIndex < _questions.Count - 1)
            {
                _currentQuestionIndex++;
                DisplayQuestion(_currentQuestionIndex);
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Are you sure you want to submit your test?\n\nYou will not be able to change your answers after submission.",
                    "Confirm Submission",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Stop the timer
                    _testTimer?.Stop();

                    // Set submitted flag
                    _isSubmitted = true;

                    // Submit the test
                    string errorMessage;
                    bool success = _testTakingManager.SubmitTest(_currentResult.ResultId, out errorMessage);

                    if (success)
                    {
                        MessageBox.Show(
                            "Test submitted successfully!\n\nYou can view your results in the 'View My Results' section.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show($"Error submitting test: {errorMessage}\n\nPlease try again.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        _isSubmitted = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting test: {ex.Message}\n\nPlease try again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _isSubmitted = false;
            }
        }

        private void SubmitTest()
        {
            try
            {
                _testTimer.Stop();

                // Save all answers
                foreach (var answer in _studentAnswers)
                {
                    int questionId = answer.Key;
                    int choiceId = answer.Value;

                    MultipleChoiceQuestion question = _questions.FirstOrDefault(q => q.QuestionId == questionId);
                    if (question != null)
                    {
                        string errorMessage;
                        _testTakingManager.SaveAnswer(_currentResult.ResultId, question, choiceId, out errorMessage);
                    }
                }

                // Submit test
                string submitError;
                bool success = _testTakingManager.SubmitTest(_currentResult.ResultId, out submitError);

                if (success)
                {
                    MessageBox.Show("Test submitted successfully!\n\nYou can view your results in the 'View My Results' section.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Error submitting test: {submitError}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestTimer_Tick(object sender, EventArgs e)
        {
            _remainingSeconds--;

            if (_remainingSeconds <= 0)
            {
                _testTimer.Stop();
                MessageBox.Show("Time's up! Your test will be auto-submitted.", "Time Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SubmitTest();
                return;
            }

            UpdateTimerDisplay();

            // Warning when 5 minutes remain
            if (_remainingSeconds == 300)
            {
                MessageBox.Show("5 minutes remaining!", "Time Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateTimerDisplay()
        {
            int minutes = _remainingSeconds / 60;
            int seconds = _remainingSeconds % 60;
            lblTimer.Text = $"Time: {minutes:D2}:{seconds:D2}";

            // Change color when time is running out
            if (_remainingSeconds <= 300)
                lblTimer.ForeColor = Color.Red;
            else if (_remainingSeconds <= 600)
                lblTimer.ForeColor = Color.Orange;
        }

        private void TestTakingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Only show warning if test is NOT submitted
            if (!_isSubmitted)
            {
                var result = MessageBox.Show(
                    "Your test is not submitted yet!\n\nAre you sure you want to exit? Your progress will be lost.",
                    "Confirm Exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    _testTimer?.Stop();
                    _testTimer?.Dispose();
                }
            }
            else
            {
                _testTimer?.Stop();
                _testTimer?.Dispose();
            }
        }
    }
}
