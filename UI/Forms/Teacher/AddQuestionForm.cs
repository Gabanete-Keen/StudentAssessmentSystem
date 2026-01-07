using System;
using System.Drawing;
using System.Windows.Forms;
using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Enums;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    public partial class AddQuestionForm : Form
    {
        // ✅ PRIVATE FIELDS
        private int? _testId;  // Nullable - null means question bank
        private int _currentQuestionNumber;
        private QuestionBankManager _questionManager;
        private MultipleChoiceQuestion _existingQuestion;  // For edit mode
        private bool _isEditMode;
        private bool _isQuestionBank;

        // UI Controls
        private Label lblTitle;
        private Label lblQuestionText;
        private TextBox txtQuestionText;
        private Label lblTopic;
        private TextBox txtTopic;
        private Label lblPoints;
        private NumericUpDown numPoints;
        private Label lblDifficulty;
        private ComboBox cmbDifficulty;
        private Label lblCognitive;
        private ComboBox cmbCognitive;
        private Label lblExplanation;
        private TextBox txtExplanation;

        // Choices
        private Label lblChoices;
        private TextBox txtChoiceA;
        private TextBox txtChoiceB;
        private TextBox txtChoiceC;
        private TextBox txtChoiceD;
        private RadioButton rdoCorrectA;
        private RadioButton rdoCorrectB;
        private RadioButton rdoCorrectC;
        private RadioButton rdoCorrectD;

        // Buttons
        private Button btnSaveAndNext;
        private Button btnSaveAndClose;
        private Button btnCancel;

        #region Constructors

        /// <summary>
        /// Constructor for adding questions to a TEST
        /// </summary>
        public AddQuestionForm(int testId, int questionNumber = 1)
        {
            _testId = testId;
            _currentQuestionNumber = questionNumber;
            _questionManager = new QuestionBankManager();
            _isEditMode = false;
            _isQuestionBank = false;

            InitializeComponent();
        }

        /// <summary>
        /// Constructor for adding questions to QUESTION BANK (testId = null)
        /// </summary>
        public AddQuestionForm()
        {
            _testId = null;  // Question bank
            _currentQuestionNumber = 1;
            _questionManager = new QuestionBankManager();
            _isEditMode = false;
            _isQuestionBank = true;

            InitializeComponent();
            SetupQuestionBankMode();
        }

        /// <summary>
        /// Constructor for EDITING an existing question (Question Bank)
        /// </summary>
        public AddQuestionForm(MultipleChoiceQuestion question)
        {
            _testId = question.TestId;
            _existingQuestion = question;
            _currentQuestionNumber = 1;
            _questionManager = new QuestionBankManager();
            _isEditMode = true;
            _isQuestionBank = question.TestId == null;

            InitializeComponent();
            SetupQuestionBankMode();
            LoadQuestionData();
        }

        #endregion

        private void InitializeComponent()
        {
            this.Text = _isEditMode ? "Edit Question" : (_isQuestionBank ? "Add Question to Bank" : $"Add Question {_currentQuestionNumber}");
            this.Size = new Size(700, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int yPos = 20;

            // ===== Title =====
            lblTitle = new Label();
            lblTitle.Text = _isEditMode ? "Edit Question" : (_isQuestionBank ? "Add to Question Bank" : $"Question {_currentQuestionNumber}");
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, yPos);
            lblTitle.Size = new Size(650, 30);
            this.Controls.Add(lblTitle);
            yPos += 40;

            // ===== Question Text =====
            lblQuestionText = new Label();
            lblQuestionText.Text = "Question Text: *";
            lblQuestionText.Location = new Point(20, yPos);
            lblQuestionText.Size = new Size(150, 20);
            lblQuestionText.Font = new Font("Arial", 9, FontStyle.Bold);
            this.Controls.Add(lblQuestionText);
            yPos += 25;

            txtQuestionText = new TextBox();
            txtQuestionText.Location = new Point(20, yPos);
            txtQuestionText.Size = new Size(640, 60);
            txtQuestionText.Multiline = true;
            txtQuestionText.ScrollBars = ScrollBars.Vertical;
            txtQuestionText.Font = new Font("Arial", 10);
            this.Controls.Add(txtQuestionText);
            yPos += 70;

            // ===== Topic (Only for Question Bank) =====
            lblTopic = new Label();
            lblTopic.Text = "Topic:";
            lblTopic.Location = new Point(20, yPos);
            lblTopic.Size = new Size(60, 20);
            lblTopic.Visible = _isQuestionBank;
            this.Controls.Add(lblTopic);

            txtTopic = new TextBox();
            txtTopic.Location = new Point(90, yPos);
            txtTopic.Size = new Size(200, 25);
            txtTopic.Font = new Font("Arial", 10);
            txtTopic.Visible = _isQuestionBank;
            this.Controls.Add(txtTopic);

            if (_isQuestionBank)
                yPos += 35;

            // ===== Points and Difficulty =====
            lblPoints = new Label();
            lblPoints.Text = "Points: *";
            lblPoints.Location = new Point(20, yPos);
            lblPoints.Size = new Size(60, 20);
            lblPoints.Font = new Font("Arial", 9, FontStyle.Bold);
            this.Controls.Add(lblPoints);

            numPoints = new NumericUpDown();
            numPoints.Location = new Point(90, yPos);
            numPoints.Size = new Size(60, 25);
            numPoints.Minimum = 1;
            numPoints.Maximum = 10;
            numPoints.Value = 1;
            this.Controls.Add(numPoints);

            lblDifficulty = new Label();
            lblDifficulty.Text = "Difficulty: *";
            lblDifficulty.Location = new Point(180, yPos);
            lblDifficulty.Size = new Size(80, 20);
            lblDifficulty.Font = new Font("Arial", 9, FontStyle.Bold);
            this.Controls.Add(lblDifficulty);

            cmbDifficulty = new ComboBox();
            cmbDifficulty.Location = new Point(270, yPos);
            cmbDifficulty.Size = new Size(120, 25);
            cmbDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDifficulty.Items.AddRange(new string[] { "Easy", "Average", "Difficult" });
            cmbDifficulty.SelectedIndex = 1;
            this.Controls.Add(cmbDifficulty);

            lblCognitive = new Label();
            lblCognitive.Text = "Cognitive: *";
            lblCognitive.Location = new Point(410, yPos);
            lblCognitive.Size = new Size(80, 20);
            lblCognitive.Font = new Font("Arial", 9, FontStyle.Bold);
            this.Controls.Add(lblCognitive);

            cmbCognitive = new ComboBox();
            cmbCognitive.Location = new Point(500, yPos);
            cmbCognitive.Size = new Size(160, 25);
            cmbCognitive.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCognitive.Items.AddRange(new string[] { "Remember", "Understand", "Apply", "Analyze", "Evaluate", "Create" });
            cmbCognitive.SelectedIndex = 0;
            this.Controls.Add(cmbCognitive);
            yPos += 40;

            // ===== Explanation (Optional for Question Bank) =====
            if (_isQuestionBank)
            {
                lblExplanation = new Label();
                lblExplanation.Text = "Explanation (Optional):";
                lblExplanation.Location = new Point(20, yPos);
                lblExplanation.Size = new Size(200, 20);
                this.Controls.Add(lblExplanation);
                yPos += 25;

                txtExplanation = new TextBox();
                txtExplanation.Location = new Point(20, yPos);
                txtExplanation.Size = new Size(640, 50);
                txtExplanation.Multiline = true;
                txtExplanation.ScrollBars = ScrollBars.Vertical;
                txtExplanation.Font = new Font("Arial", 9);
                this.Controls.Add(txtExplanation);
                yPos += 60;
            }

            // ===== Choices Label =====
            lblChoices = new Label();
            lblChoices.Text = "Answer Choices (Select the correct answer): *";
            lblChoices.Font = new Font("Arial", 10, FontStyle.Bold);
            lblChoices.Location = new Point(20, yPos);
            lblChoices.Size = new Size(450, 20);
            this.Controls.Add(lblChoices);
            yPos += 30;

            // ===== Choice A =====
            rdoCorrectA = new RadioButton();
            rdoCorrectA.Location = new Point(20, yPos);
            rdoCorrectA.Size = new Size(30, 25);
            rdoCorrectA.Checked = true;
            this.Controls.Add(rdoCorrectA);

            Label lblA = new Label();
            lblA.Text = "A.";
            lblA.Location = new Point(50, yPos + 3);
            lblA.Size = new Size(20, 20);
            lblA.Font = new Font("Arial", 10, FontStyle.Bold);
            this.Controls.Add(lblA);

            txtChoiceA = new TextBox();
            txtChoiceA.Location = new Point(80, yPos);
            txtChoiceA.Size = new Size(580, 25);
            txtChoiceA.Font = new Font("Arial", 10);
            this.Controls.Add(txtChoiceA);
            yPos += 35;

            // ===== Choice B =====
            rdoCorrectB = new RadioButton();
            rdoCorrectB.Location = new Point(20, yPos);
            rdoCorrectB.Size = new Size(30, 25);
            this.Controls.Add(rdoCorrectB);

            Label lblB = new Label();
            lblB.Text = "B.";
            lblB.Location = new Point(50, yPos + 3);
            lblB.Size = new Size(20, 20);
            lblB.Font = new Font("Arial", 10, FontStyle.Bold);
            this.Controls.Add(lblB);

            txtChoiceB = new TextBox();
            txtChoiceB.Location = new Point(80, yPos);
            txtChoiceB.Size = new Size(580, 25);
            txtChoiceB.Font = new Font("Arial", 10);
            this.Controls.Add(txtChoiceB);
            yPos += 35;

            // ===== Choice C =====
            rdoCorrectC = new RadioButton();
            rdoCorrectC.Location = new Point(20, yPos);
            rdoCorrectC.Size = new Size(30, 25);
            this.Controls.Add(rdoCorrectC);

            Label lblC = new Label();
            lblC.Text = "C.";
            lblC.Location = new Point(50, yPos + 3);
            lblC.Size = new Size(20, 20);
            lblC.Font = new Font("Arial", 10, FontStyle.Bold);
            this.Controls.Add(lblC);

            txtChoiceC = new TextBox();
            txtChoiceC.Location = new Point(80, yPos);
            txtChoiceC.Size = new Size(580, 25);
            txtChoiceC.Font = new Font("Arial", 10);
            this.Controls.Add(txtChoiceC);
            yPos += 35;

            // ===== Choice D =====
            rdoCorrectD = new RadioButton();
            rdoCorrectD.Location = new Point(20, yPos);
            rdoCorrectD.Size = new Size(30, 25);
            this.Controls.Add(rdoCorrectD);

            Label lblD = new Label();
            lblD.Text = "D.";
            lblD.Location = new Point(50, yPos + 3);
            lblD.Size = new Size(20, 20);
            lblD.Font = new Font("Arial", 10, FontStyle.Bold);
            this.Controls.Add(lblD);

            txtChoiceD = new TextBox();
            txtChoiceD.Location = new Point(80, yPos);
            txtChoiceD.Size = new Size(580, 25);
            txtChoiceD.Font = new Font("Arial", 10);
            this.Controls.Add(txtChoiceD);
            yPos += 50;

            // ===== Buttons =====
            if (_isEditMode)
            {
                // Edit Mode: Only show Update and Cancel
                btnSaveAndClose = new Button();
                btnSaveAndClose.Text = "💾 Update Question";
                btnSaveAndClose.Location = new Point(370, yPos);
                btnSaveAndClose.Size = new Size(150, 35);
                btnSaveAndClose.BackColor = Color.FromArgb(52, 152, 219);
                btnSaveAndClose.ForeColor = Color.White;
                btnSaveAndClose.Font = new Font("Arial", 10, FontStyle.Bold);
                btnSaveAndClose.Cursor = Cursors.Hand;
                btnSaveAndClose.Click += BtnSaveAndClose_Click;
                this.Controls.Add(btnSaveAndClose);

                btnCancel = new Button();
                btnCancel.Text = "Cancel";
                btnCancel.Location = new Point(530, yPos);
                btnCancel.Size = new Size(100, 35);
                btnCancel.BackColor = Color.LightGray;
                btnCancel.Cursor = Cursors.Hand;
                btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
                this.Controls.Add(btnCancel);
            }
            else if (_isQuestionBank)
            {
                // Question Bank Mode: Save & Add Another, Save & Close
                btnSaveAndNext = new Button();
                btnSaveAndNext.Text = "Save & Add Another";
                btnSaveAndNext.Location = new Point(280, yPos);
                btnSaveAndNext.Size = new Size(150, 35);
                btnSaveAndNext.BackColor = Color.FromArgb(46, 204, 113);
                btnSaveAndNext.ForeColor = Color.White;
                btnSaveAndNext.Font = new Font("Arial", 10, FontStyle.Bold);
                btnSaveAndNext.Cursor = Cursors.Hand;
                btnSaveAndNext.Click += BtnSaveAndNext_Click;
                this.Controls.Add(btnSaveAndNext);

                btnSaveAndClose = new Button();
                btnSaveAndClose.Text = "Save & Close";
                btnSaveAndClose.Location = new Point(440, yPos);
                btnSaveAndClose.Size = new Size(110, 35);
                btnSaveAndClose.BackColor = Color.FromArgb(52, 152, 219);
                btnSaveAndClose.ForeColor = Color.White;
                btnSaveAndClose.Font = new Font("Arial", 10);
                btnSaveAndClose.Cursor = Cursors.Hand;
                btnSaveAndClose.Click += BtnSaveAndClose_Click;
                this.Controls.Add(btnSaveAndClose);

                btnCancel = new Button();
                btnCancel.Text = "Cancel";
                btnCancel.Location = new Point(560, yPos);
                btnCancel.Size = new Size(100, 35);
                btnCancel.BackColor = Color.LightGray;
                btnCancel.Cursor = Cursors.Hand;
                btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
                this.Controls.Add(btnCancel);
            }
            else
            {
                // Test Mode: Original buttons
                btnSaveAndNext = new Button();
                btnSaveAndNext.Text = "Save & Add Another";
                btnSaveAndNext.Location = new Point(280, yPos);
                btnSaveAndNext.Size = new Size(150, 35);
                btnSaveAndNext.BackColor = Color.LightGreen;
                btnSaveAndNext.Font = new Font("Arial", 10, FontStyle.Bold);
                btnSaveAndNext.Cursor = Cursors.Hand;
                btnSaveAndNext.Click += BtnSaveAndNext_Click;
                this.Controls.Add(btnSaveAndNext);

                btnSaveAndClose = new Button();
                btnSaveAndClose.Text = "Save & Finish";
                btnSaveAndClose.Location = new Point(440, yPos);
                btnSaveAndClose.Size = new Size(110, 35);
                btnSaveAndClose.BackColor = Color.LightBlue;
                btnSaveAndClose.Font = new Font("Arial", 10);
                btnSaveAndClose.Cursor = Cursors.Hand;
                btnSaveAndClose.Click += BtnSaveAndClose_Click;
                this.Controls.Add(btnSaveAndClose);

                btnCancel = new Button();
                btnCancel.Text = "Cancel";
                btnCancel.Location = new Point(560, yPos);
                btnCancel.Size = new Size(100, 35);
                btnCancel.BackColor = Color.LightGray;
                btnCancel.Cursor = Cursors.Hand;
                btnCancel.Click += (s, e) => this.Close();
                this.Controls.Add(btnCancel);
            }
        }

        private void SetupQuestionBankMode()
        {
            // Already handled in InitializeComponent with visibility flags
        }

        private void LoadQuestionData()
        {
            if (_existingQuestion == null) return;

            txtQuestionText.Text = _existingQuestion.QuestionText;
            if (_isQuestionBank && txtTopic != null)
                txtTopic.Text = _existingQuestion.Topic ?? "";
            numPoints.Value = _existingQuestion.PointValue;
            cmbDifficulty.SelectedIndex = (int)_existingQuestion.DifficultyLevel;
            cmbCognitive.SelectedIndex = (int)_existingQuestion.CognitiveLevel;

            if (_isQuestionBank && txtExplanation != null)
                txtExplanation.Text = _existingQuestion.Explanation ?? "";

            // Load choices
            if (_existingQuestion.Choices != null && _existingQuestion.Choices.Count > 0)
            {
                for (int i = 0; i < _existingQuestion.Choices.Count && i < 4; i++)
                {
                    var choice = _existingQuestion.Choices[i];
                    switch (i)
                    {
                        case 0:
                            txtChoiceA.Text = choice.ChoiceText;
                            rdoCorrectA.Checked = choice.IsCorrect;
                            break;
                        case 1:
                            txtChoiceB.Text = choice.ChoiceText;
                            rdoCorrectB.Checked = choice.IsCorrect;
                            break;
                        case 2:
                            txtChoiceC.Text = choice.ChoiceText;
                            rdoCorrectC.Checked = choice.IsCorrect;
                            break;
                        case 3:
                            txtChoiceD.Text = choice.ChoiceText;
                            rdoCorrectD.Checked = choice.IsCorrect;
                            break;
                    }
                }
            }
        }

        private void BtnSaveAndNext_Click(object sender, EventArgs e)
        {
            if (SaveQuestion())
            {
                // Clear form for next question
                ClearForm();
                _currentQuestionNumber++;
                if (!_isQuestionBank)
                {
                    lblTitle.Text = $"Question {_currentQuestionNumber}";
                    this.Text = $"Add Question {_currentQuestionNumber}";
                }
                txtQuestionText.Focus();
            }
        }

        private void BtnSaveAndClose_Click(object sender, EventArgs e)
        {
            if (SaveQuestion())
            {
                string message = _isEditMode
                    ? "Question updated successfully!"
                    : (_isQuestionBank
                        ? "Question added to bank successfully!"
                        : $"Question {_currentQuestionNumber} saved successfully!");

                MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Saves question - handles both test questions and question bank
        /// </summary>
        private bool SaveQuestion()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtQuestionText.Text))
            {
                MessageBox.Show("Please enter the question text.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuestionText.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtChoiceA.Text) || string.IsNullOrWhiteSpace(txtChoiceB.Text))
            {
                MessageBox.Show("Please enter at least two answer choices (A and B).", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                if (_isEditMode)
                {
                    // UPDATE existing question
                    _existingQuestion.QuestionText = txtQuestionText.Text.Trim();
                    _existingQuestion.PointValue = (int)numPoints.Value;
                    _existingQuestion.DifficultyLevel = (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), cmbDifficulty.SelectedItem.ToString());
                    _existingQuestion.CognitiveLevel = (CognitiveLevel)Enum.Parse(typeof(CognitiveLevel), cmbCognitive.SelectedItem.ToString());

                    if (_isQuestionBank)
                    {
                        _existingQuestion.Topic = txtTopic?.Text.Trim();
                        _existingQuestion.Explanation = txtExplanation?.Text.Trim();
                    }

                    // Update choices
                    _existingQuestion.Choices.Clear();
                    int choiceOrder = 1;
                    if (!string.IsNullOrWhiteSpace(txtChoiceA.Text))
                        _existingQuestion.AddChoice(txtChoiceA.Text.Trim(), rdoCorrectA.Checked, choiceOrder++);
                    if (!string.IsNullOrWhiteSpace(txtChoiceB.Text))
                        _existingQuestion.AddChoice(txtChoiceB.Text.Trim(), rdoCorrectB.Checked, choiceOrder++);
                    if (!string.IsNullOrWhiteSpace(txtChoiceC.Text))
                        _existingQuestion.AddChoice(txtChoiceC.Text.Trim(), rdoCorrectC.Checked, choiceOrder++);
                    if (!string.IsNullOrWhiteSpace(txtChoiceD.Text))
                        _existingQuestion.AddChoice(txtChoiceD.Text.Trim(), rdoCorrectD.Checked, choiceOrder++);

                    bool success = _questionManager.UpdateQuestion(_existingQuestion, out string error);
                    if (!success)
                    {
                        MessageBox.Show($"Failed to update question: {error}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return success;
                }
                else
                {
                    // CREATE new question
                    var question = new MultipleChoiceQuestion
                    {
                        TestId = _testId,  // Will be null for question bank
                        QuestionText = txtQuestionText.Text.Trim(),
                        QuestionType = "MultipleChoice",
                        PointValue = (int)numPoints.Value,
                        DifficultyLevel = (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), cmbDifficulty.SelectedItem.ToString()),
                        CognitiveLevel = (CognitiveLevel)Enum.Parse(typeof(CognitiveLevel), cmbCognitive.SelectedItem.ToString()),
                        OrderNumber = _currentQuestionNumber
                    };

                    if (_isQuestionBank)
                    {
                        question.Topic = txtTopic?.Text.Trim();
                        question.Explanation = txtExplanation?.Text.Trim();
                    }

                    // Add choices
                    int choiceOrder = 1;
                    if (!string.IsNullOrWhiteSpace(txtChoiceA.Text))
                        question.AddChoice(txtChoiceA.Text.Trim(), rdoCorrectA.Checked, choiceOrder++);
                    if (!string.IsNullOrWhiteSpace(txtChoiceB.Text))
                        question.AddChoice(txtChoiceB.Text.Trim(), rdoCorrectB.Checked, choiceOrder++);
                    if (!string.IsNullOrWhiteSpace(txtChoiceC.Text))
                        question.AddChoice(txtChoiceC.Text.Trim(), rdoCorrectC.Checked, choiceOrder++);
                    if (!string.IsNullOrWhiteSpace(txtChoiceD.Text))
                        question.AddChoice(txtChoiceD.Text.Trim(), rdoCorrectD.Checked, choiceOrder++);

                    int questionId;
                    if (_isQuestionBank)
                    {
                        // Save to question bank
                        questionId = _questionManager.AddToQuestionBank(question, out string error);
                        if (questionId <= 0)
                        {
                            MessageBox.Show($"Failed to add question: {error}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    else
                    {
                        // Save directly to test using repository
                        var questionRepo = new QuestionRepository();
                        questionId = questionRepo.AddQuestion(question);
                        if (questionId <= 0)
                        {
                            MessageBox.Show("Failed to save question.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving question: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ClearForm()
        {
            txtQuestionText.Clear();
            if (txtTopic != null) txtTopic.Clear();
            numPoints.Value = 1;
            cmbDifficulty.SelectedIndex = 1;
            cmbCognitive.SelectedIndex = 0;
            if (txtExplanation != null) txtExplanation.Clear();
            txtChoiceA.Clear();
            txtChoiceB.Clear();
            txtChoiceC.Clear();
            txtChoiceD.Clear();
            rdoCorrectA.Checked = true;
        }
    }
}