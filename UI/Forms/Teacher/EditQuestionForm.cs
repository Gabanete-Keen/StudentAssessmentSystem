using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
   
    /// FORM PURPOSE: Edit an existing question
    public partial class EditQuestionForm : Form
    {
        private QuestionBankManager _questionManager;
        private MultipleChoiceQuestion _question;
        private List<TextBox> _choiceTextBoxes;
        private List<RadioButton> _correctRadioButtons;

        // UI Controls
        private Label lblTitle;
        private TextBox txtQuestionText;
        private NumericUpDown numPoints;
        private ComboBox cmbDifficulty;
        private ComboBox cmbCognitiveLevel;
        private Panel pnlChoices;
        private Button btnSave;
        private Button btnCancel;

        public EditQuestionForm(int questionId)
        {
            _questionManager = new QuestionBankManager();
            _choiceTextBoxes = new List<TextBox>();
            _correctRadioButtons = new List<RadioButton>();

            // Load question
            var questionRepo = new QuestionRepository();
            _question = questionRepo.GetQuestionById(questionId);

            if (_question == null)
            {
                MessageBox.Show("Question not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            InitializeComponent();
            LoadQuestionData();
        }

        private void InitializeComponent()
        {
            this.Text = "Edit Question";
            this.Size = new Size(700, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int yPos = 20;

            // Title
            lblTitle = new Label
            {
                Location = new Point(20, yPos),
                Size = new Size(640, 30),
                Text = "✏️ Edit Question",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(lblTitle);
            yPos += 40;

            // Question Text
            Label lblQuestion = new Label
            {
                Location = new Point(20, yPos),
                Size = new Size(150, 20),
                Text = "Question Text:",
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblQuestion);

            txtQuestionText = new TextBox
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(640, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(txtQuestionText);
            yPos += 120;

            // Points and Difficulty
            Label lblPoints = new Label
            {
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                Text = "Points:",
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblPoints);

            numPoints = new NumericUpDown
            {
                Location = new Point(120, yPos),
                Size = new Size(80, 25),
                Minimum = 1,
                Maximum = 10,
                Value = 1
            };
            this.Controls.Add(numPoints);

            Label lblDifficulty = new Label
            {
                Location = new Point(230, yPos),
                Size = new Size(100, 20),
                Text = "Difficulty:",
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblDifficulty);

            cmbDifficulty = new ComboBox
            {
                Location = new Point(330, yPos),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDifficulty.Items.AddRange(Enum.GetNames(typeof(DifficultyLevel)));
            this.Controls.Add(cmbDifficulty);

            Label lblCognitive = new Label
            {
                Location = new Point(470, yPos),
                Size = new Size(80, 20),
                Text = "Cognitive:",
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblCognitive);

            cmbCognitiveLevel = new ComboBox
            {
                Location = new Point(555, yPos),
                Size = new Size(105, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCognitiveLevel.Items.AddRange(Enum.GetNames(typeof(CognitiveLevel)));
            this.Controls.Add(cmbCognitiveLevel);
            yPos += 40;

            // Choices Panel
            pnlChoices = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(640, 300),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                BackColor = Color.WhiteSmoke
            };
            this.Controls.Add(pnlChoices);
            yPos += 310;

            // Buttons
            btnSave = new Button
            {
                Location = new Point(460, yPos),
                Size = new Size(100, 35),
                Text = "Save",
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightGreen,
                Cursor = Cursors.Hand
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Location = new Point(570, yPos),
                Size = new Size(90, 35),
                Text = "Cancel",
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightGray,
                Cursor = Cursors.Hand
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void LoadQuestionData()
        {
            txtQuestionText.Text = _question.QuestionText;
            numPoints.Value = _question.PointValue;
            cmbDifficulty.SelectedItem = _question.DifficultyLevel.ToString();
            cmbCognitiveLevel.SelectedItem = _question.CognitiveLevel.ToString();

            // Load choices
            int choiceYPos = 10;
            for (int i = 0; i < _question.Choices.Count; i++)
            {
                var choice = _question.Choices[i];

                RadioButton radio = new RadioButton
                {
                    Location = new Point(10, choiceYPos + 5),
                    Size = new Size(20, 20),
                    Checked = choice.IsCorrect
                };
                pnlChoices.Controls.Add(radio);
                _correctRadioButtons.Add(radio);

                TextBox txtChoice = new TextBox
                {
                    Location = new Point(40, choiceYPos),
                    Size = new Size(560, 25),
                    Text = choice.ChoiceText,
                    Font = new Font("Arial", 10)
                };
                pnlChoices.Controls.Add(txtChoice);
                _choiceTextBoxes.Add(txtChoice);

                choiceYPos += 35;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtQuestionText.Text))
                {
                    MessageBox.Show("Question text is required.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool hasCorrectAnswer = _correctRadioButtons.Any(r => r.Checked);
                if (!hasCorrectAnswer)
                {
                    MessageBox.Show("Please select at least one correct answer.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update question object
                _question.QuestionText = txtQuestionText.Text.Trim();
                _question.PointValue = (int)numPoints.Value;
                _question.DifficultyLevel = (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), cmbDifficulty.SelectedItem.ToString());
                _question.CognitiveLevel = (CognitiveLevel)Enum.Parse(typeof(CognitiveLevel), cmbCognitiveLevel.SelectedItem.ToString());

                // Update choices
                for (int i = 0; i < _question.Choices.Count && i < _choiceTextBoxes.Count; i++)
                {
                    _question.Choices[i].ChoiceText = _choiceTextBoxes[i].Text.Trim();
                    _question.Choices[i].IsCorrect = _correctRadioButtons[i].Checked;
                }

                // Save to database
                string errorMessage;
                bool success = _questionManager.UpdateQuestion(_question, out errorMessage);

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving question:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
