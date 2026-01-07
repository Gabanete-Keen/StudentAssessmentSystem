using StudentAssessmentSystem.Models.Assessment;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    /// <summary>
    /// Form to view detailed information about a question
    /// </summary>
    public partial class ViewQuestionDetailsForm : Form
    {
        private readonly MultipleChoiceQuestion _question;

        public ViewQuestionDetailsForm(MultipleChoiceQuestion question)
        {
            _question = question;
            InitializeComponent();
            LoadQuestionDetails();
        }

        private void InitializeComponent()
        {
            this.Text = "Question Details";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main Panel
            Panel mainPanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(640, 500),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            int yPosition = 20;

            // Title
            Label lblTitle = new Label
            {
                Text = "📄 Question Details",
                Location = new Point(20, yPosition),
                Size = new Size(600, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185)
            };
            mainPanel.Controls.Add(lblTitle);
            yPosition += 40;

            // Question ID
            AddInfoLabel("Question ID:", _question.QuestionId.ToString(), ref yPosition, mainPanel);

            // Question Text
            Label lblQuestionLabel = CreateLabel("Question Text:", new Point(20, yPosition), true);
            mainPanel.Controls.Add(lblQuestionLabel);
            yPosition += 25;

            TextBox txtQuestion = new TextBox
            {
                Location = new Point(20, yPosition),
                Size = new Size(580, 80),
                Text = _question.QuestionText,
                Multiline = true,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(236, 240, 241),
                ScrollBars = ScrollBars.Vertical
            };
            mainPanel.Controls.Add(txtQuestion);
            yPosition += 90;

            // Metadata Section
            Label lblMetadata = CreateLabel("Metadata:", new Point(20, yPosition), true);
            mainPanel.Controls.Add(lblMetadata);
            yPosition += 25;

            AddInfoLabel("Topic:", _question.Topic ?? "N/A", ref yPosition, mainPanel);
            AddInfoLabel("Difficulty Level:", _question.DifficultyLevel.ToString(), ref yPosition, mainPanel);
            AddInfoLabel("Cognitive Level:", _question.CognitiveLevel.ToString(), ref yPosition, mainPanel);
            AddInfoLabel("Point Value:", _question.PointValue.ToString(), ref yPosition, mainPanel);

            // Choices Section
            Label lblChoices = CreateLabel("Answer Choices:", new Point(20, yPosition), true);
            mainPanel.Controls.Add(lblChoices);
            yPosition += 25;

            if (_question.Choices != null && _question.Choices.Count > 0)
            {
                foreach (var choice in _question.Choices)
                {
                    Panel choicePanel = new Panel
                    {
                        Location = new Point(30, yPosition),
                        Size = new Size(570, 40),
                        BackColor = choice.IsCorrect ? Color.FromArgb(46, 204, 113) : Color.FromArgb(236, 240, 241),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    string prefix = choice.IsCorrect ? "✓" : "○";
                    Label lblChoice = new Label
                    {
                        Text = $"{prefix} {choice.ChoiceText}",
                        Location = new Point(10, 10),
                        Size = new Size(550, 20),
                        Font = new Font("Segoe UI", 9),
                        ForeColor = choice.IsCorrect ? Color.White : Color.Black
                    };

                    choicePanel.Controls.Add(lblChoice);
                    mainPanel.Controls.Add(choicePanel);
                    yPosition += 45;
                }
            }

            // Explanation (if exists)
            if (!string.IsNullOrWhiteSpace(_question.Explanation))
            {
                Label lblExplanationLabel = CreateLabel("Explanation:", new Point(20, yPosition), true);
                mainPanel.Controls.Add(lblExplanationLabel);
                yPosition += 25;

                TextBox txtExplanation = new TextBox
                {
                    Location = new Point(20, yPosition),
                    Size = new Size(580, 60),
                    Text = _question.Explanation,
                    Multiline = true,
                    ReadOnly = true,
                    Font = new Font("Segoe UI", 9),
                    BackColor = Color.FromArgb(255, 243, 205),
                    ScrollBars = ScrollBars.Vertical
                };
                mainPanel.Controls.Add(txtExplanation);
                yPosition += 70;
            }

            // Close Button
            Button btnClose = new Button
            {
                Text = "Close",
                Location = new Point(270, 530),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(mainPanel);
            this.Controls.Add(btnClose);
        }

        private void LoadQuestionDetails()
        {
            // Details are loaded in InitializeComponent
        }

        private Label CreateLabel(string text, Point location, bool isBold = false)
        {
            return new Label
            {
                Text = text,
                Location = location,
                AutoSize = true,
                Font = new Font("Segoe UI", 10, isBold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
        }

        private void AddInfoLabel(string label, string value, ref int yPosition, Panel parent)
        {
            Label lblLabel = CreateLabel(label, new Point(40, yPosition), true);
            parent.Controls.Add(lblLabel);

            Label lblValue = CreateLabel(value, new Point(200, yPosition));
            parent.Controls.Add(lblValue);

            yPosition += 30;
        }
    }
}