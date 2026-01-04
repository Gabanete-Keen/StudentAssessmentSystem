using System;
using System.Drawing;
using System.Windows.Forms;
using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Enums;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    public partial class AddQuestionForm : Form
    {
        private int _testId;
        private QuestionBankManager _questionManager;
        private int _currentQuestionNumber;

        // UI Controls
        private Label lblTitle;
        private Label lblQuestionText;
        private TextBox txtQuestionText;
        private Label lblPoints;
        private NumericUpDown numPoints;
        private Label lblDifficulty;
        private ComboBox cmbDifficulty;
        private Label lblCognitive;
        private ComboBox cmbCognitive;

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

        private Button btnSaveAndNext;
        private Button btnSaveAndClose;
        private Button btnCancel;

        public AddQuestionForm(int testId, int questionNumber = 1)
        {
            _testId = testId;
            _currentQuestionNumber = questionNumber;
            _questionManager = new QuestionBankManager();

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"Add Question #{_currentQuestionNumber}";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int yPos = 20;

            // Title
            lblTitle = new Label();
            lblTitle.Text = $"Question #{_currentQuestionNumber}";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, yPos);
            lblTitle.Size = new Size(650, 30);
            this.Controls.Add(lblTitle);
            yPos += 40;

            // Question Text
            lblQuestionText = new Label();
            lblQuestionText.Text = "Question Text:";
            lblQuestionText.Location = new Point(20, yPos);
            lblQuestionText.Size = new Size(150, 20);
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

            // Points and Difficulty
            lblPoints = new Label();
            lblPoints.Text = "Points:";
            lblPoints.Location = new Point(20, yPos);
            lblPoints.Size = new Size(60, 20);
            this.Controls.Add(lblPoints);

            numPoints = new NumericUpDown();
            numPoints.Location = new Point(90, yPos);
            numPoints.Size = new Size(60, 25);
            numPoints.Minimum = 1;
            numPoints.Maximum = 10;
            numPoints.Value = 1;
            this.Controls.Add(numPoints);

            lblDifficulty = new Label();
            lblDifficulty.Text = "Difficulty:";
            lblDifficulty.Location = new Point(200, yPos);
            lblDifficulty.Size = new Size(70, 20);
            this.Controls.Add(lblDifficulty);

            cmbDifficulty = new ComboBox();
            cmbDifficulty.Location = new Point(280, yPos);
            cmbDifficulty.Size = new Size(120, 25);
            cmbDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDifficulty.Items.AddRange(new string[] { "Easy", "Average", "Difficult" });
            cmbDifficulty.SelectedIndex = 1;
            this.Controls.Add(cmbDifficulty);

            lblCognitive = new Label();
            lblCognitive.Text = "Cognitive Level:";
            lblCognitive.Location = new Point(420, yPos);
            lblCognitive.Size = new Size(100, 20);
            this.Controls.Add(lblCognitive);

            cmbCognitive = new ComboBox();
            cmbCognitive.Location = new Point(530, yPos);
            cmbCognitive.Size = new Size(130, 25);
            cmbCognitive.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCognitive.Items.AddRange(new string[] { "Remember", "Understand", "Apply", "Analyze", "Evaluate", "Create" });
            cmbCognitive.SelectedIndex = 0;
            this.Controls.Add(cmbCognitive);
            yPos += 40;

            // Choices Label
            lblChoices = new Label();
            lblChoices.Text = "Answer Choices (Select the correct answer):";
            lblChoices.Font = new Font("Arial", 10, FontStyle.Bold);
            lblChoices.Location = new Point(20, yPos);
            lblChoices.Size = new Size(350, 20);
            this.Controls.Add(lblChoices);
            yPos += 30;

            // Choice A
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

            // Choice B
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

            // Choice C
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

            // Choice D
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

            // Buttons
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

        private void BtnSaveAndNext_Click(object sender, EventArgs e)
        {
            if (SaveQuestion())
            {
                // Clear form for next question
                ClearForm();
                _currentQuestionNumber++;
                lblTitle.Text = $"Question #{_currentQuestionNumber}";
                this.Text = $"Add Question #{_currentQuestionNumber}";
                txtQuestionText.Focus();
            }
        }

        private void BtnSaveAndClose_Click(object sender, EventArgs e)
        {
            if (SaveQuestion())
            {
                MessageBox.Show($"Question #{_currentQuestionNumber} saved successfully!\n\n" +
                    "You can now administer this test to students.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

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
                // Create question
                var question = new MultipleChoiceQuestion
                {
                    TestId = _testId,
                    QuestionText = txtQuestionText.Text.Trim(),
                    PointValue = (int)numPoints.Value,
                    DifficultyLevel = (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), cmbDifficulty.SelectedItem.ToString()),
                    CognitiveLevel = (CognitiveLevel)Enum.Parse(typeof(CognitiveLevel), cmbCognitive.SelectedItem.ToString()),
                    OrderNumber = _currentQuestionNumber
                };

                // Add choices
                int choiceOrder = 1;
                if (!string.IsNullOrWhiteSpace(txtChoiceA.Text))
                {
                    question.AddChoice(txtChoiceA.Text.Trim(), rdoCorrectA.Checked, choiceOrder++);
                }
                if (!string.IsNullOrWhiteSpace(txtChoiceB.Text))
                {
                    question.AddChoice(txtChoiceB.Text.Trim(), rdoCorrectB.Checked, choiceOrder++);
                }
                if (!string.IsNullOrWhiteSpace(txtChoiceC.Text))
                {
                    question.AddChoice(txtChoiceC.Text.Trim(), rdoCorrectC.Checked, choiceOrder++);
                }
                if (!string.IsNullOrWhiteSpace(txtChoiceD.Text))
                {
                    question.AddChoice(txtChoiceD.Text.Trim(), rdoCorrectD.Checked, choiceOrder++);
                }

                // Save to database
                int questionId = _questionManager.AddToQuestionBank(question, out string errorMessage);

                if (questionId > 0)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show($"Failed to save question:\n{errorMessage}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving question:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ClearForm()
        {
            txtQuestionText.Clear();
            numPoints.Value = 1;
            cmbDifficulty.SelectedIndex = 1;
            cmbCognitive.SelectedIndex = 0;
            txtChoiceA.Clear();
            txtChoiceB.Clear();
            txtChoiceC.Clear();
            txtChoiceD.Clear();
            rdoCorrectA.Checked = true;
        }
    }
}