using MySql.Data.MySqlClient;
using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.DataAccess;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    public partial class ManageQuestionsForm : Form
    {
        private QuestionBankManager _questionManager;
        private QuestionRepository _questionRepository;
        private TestRepository _testRepository;
        private int _testId;
        private string _testTitle;

        private Label lblTitle;
        private DataGridView dgvQuestions;
        private Button btnAddNew;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnClose;
        private Label lblInfo;

        public ManageQuestionsForm(int testId, string testTitle)
        {
            _testId = testId;
            _testTitle = testTitle;
            _questionManager = new QuestionBankManager();
            _questionRepository = new QuestionRepository();
            _testRepository = new TestRepository();

            InitializeComponent();
            SetupDataGridColumns(); // ✅ SETUP COLUMNS FIRST
            LoadQuestions();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Questions";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int yPos = 20;

            lblTitle = new Label
            {
                Location = new Point(30, yPos),
                Size = new Size(1000, 35),
                Text = $"📝 Manage Questions - {_testTitle}",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(lblTitle);
            yPos += 45;

            lblInfo = new Label
            {
                Location = new Point(30, yPos),
                Size = new Size(1000, 20),
                Text = "Select a question to Edit or Delete. Questions answered by students cannot be deleted.",
                Font = new Font("Arial", 9),
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblInfo);
            yPos += 30;

            dgvQuestions = new DataGridView
            {
                Location = new Point(30, yPos),
                Size = new Size(1020, 480),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false, 
                RowHeadersVisible = false,
                Font = new Font("Arial", 10)
            };
            dgvQuestions.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
            dgvQuestions.SelectionChanged += DgvQuestions_SelectionChanged;
            this.Controls.Add(dgvQuestions);
            yPos += 490;

            btnAddNew = new Button
            {
                Location = new Point(30, yPos),
                Size = new Size(180, 40),
                Text = "➕ Add New Question",
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightGreen,
                Cursor = Cursors.Hand
            };
            btnAddNew.Click += BtnAddNew_Click;
            this.Controls.Add(btnAddNew);

            btnEdit = new Button
            {
                Location = new Point(230, yPos),
                Size = new Size(150, 40),
                Text = "✏️ Edit Question",
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightBlue,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnDelete = new Button
            {
                Location = new Point(400, yPos),
                Size = new Size(150, 40),
                Text = "🗑️ Delete Question",
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightCoral,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            btnClose = new Button
            {
                Location = new Point(900, yPos),
                Size = new Size(150, 40),
                Text = "Close",
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightGray,
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

       
        /// ✅ MANUALLY CREATE COLUMNS - This prevents the error
        private void SetupDataGridColumns()
        {
            dgvQuestions.Columns.Clear();

            // QuestionId (hidden)
            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QuestionId",
                DataPropertyName = "QuestionId",
                Visible = false
            });

            // Number
            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "No",
                DataPropertyName = "No",
                HeaderText = "#",
                Width = 50
            });

            // Question Text
            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QuestionText",
                DataPropertyName = "QuestionText",
                HeaderText = "Question",
                Width = 400
            });

            // Points
            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Points",
                DataPropertyName = "Points",
                HeaderText = "Points",
                Width = 80
            });

            // Difficulty
            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Difficulty",
                DataPropertyName = "Difficulty",
                HeaderText = "Difficulty",
                Width = 120
            });

            // Cognitive Level
            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CognitiveLevel",
                DataPropertyName = "CognitiveLevel",
                HeaderText = "Cognitive Level",
                Width = 140
            });

            // Correct Answer
            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CorrectAnswer",
                DataPropertyName = "CorrectAnswer",
                HeaderText = "Correct Answer",
                Width = 200
            });
        }

        
        /// Load questions without column manipulation
        private void LoadQuestions()
        {
            try
            {
                var questions = _questionRepository.GetQuestionsByTest(_testId);

                if (questions == null || questions.Count == 0)
                {
                    dgvQuestions.DataSource = null;
                    dgvQuestions.Rows.Clear();
                    lblInfo.Text = "No questions found. Click 'Add New Question' to create one.";
                    lblInfo.ForeColor = Color.OrangeRed;
                    return;
                }

                // Create list of display objects
                var displayData = new List<QuestionDisplayItem>();
                int questionNumber = 1;

                foreach (var question in questions)
                {
                    string correctAnswers = "";
                    if (question.Choices != null && question.Choices.Count > 0)
                    {
                        var correctChoices = question.Choices.Where(c => c.IsCorrect).ToList();
                        if (correctChoices.Count > 0)
                        {
                            correctAnswers = string.Join(", ", correctChoices.Select(c => c.ChoiceText));
                        }
                    }

                    displayData.Add(new QuestionDisplayItem
                    {
                        QuestionId = question.QuestionId,
                        No = questionNumber++,
                        QuestionText = question.QuestionText != null && question.QuestionText.Length > 80
                            ? question.QuestionText.Substring(0, 77) + "..."
                            : question.QuestionText ?? "",
                        Points = question.PointValue,
                        Difficulty = question.DifficultyLevel.ToString(),
                        CognitiveLevel = question.CognitiveLevel.ToString(),
                        CorrectAnswer = correctAnswers.Length > 50
                            ? correctAnswers.Substring(0, 47) + "..."
                            : correctAnswers
                    });
                }

                dgvQuestions.DataSource = displayData;

                lblInfo.Text = $"Total Questions: {questions.Count} | Select a question to Edit or Delete";
                lblInfo.ForeColor = Color.Gray;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading questions:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvQuestions_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = dgvQuestions.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void BtnAddNew_Click(object sender, EventArgs e)
        {
            AddQuestionForm addForm = new AddQuestionForm(_testId, GetNextQuestionNumber());
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadQuestions();
                MessageBox.Show("Question added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvQuestions.SelectedRows.Count == 0)
                return;

            try
            {
                int questionId = Convert.ToInt32(dgvQuestions.SelectedRows[0].Cells["QuestionId"].Value);

                EditQuestionForm editForm = new EditQuestionForm(questionId);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadQuestions();
                    MessageBox.Show("Question updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing question:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvQuestions.SelectedRows.Count == 0)
                return;

            try
            {
                int questionId = Convert.ToInt32(dgvQuestions.SelectedRows[0].Cells["QuestionId"].Value);
                string questionText = dgvQuestions.SelectedRows[0].Cells["QuestionText"].Value.ToString();

                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to delete this question?\n\n\"{questionText}\"\n\n" +
                    "This action cannot be undone.",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    string errorMessage;
                    bool success = _questionManager.DeleteQuestion(questionId, out errorMessage);

                    if (success)
                    {
                        LoadQuestions();
                        MessageBox.Show("Question deleted successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(errorMessage, "Cannot Delete",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting question:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetNextQuestionNumber()
        {
            return dgvQuestions.Rows.Count + 1;
        }
    }

    
    /// ✅ HELPER CLASS: Data transfer object for display
    public class QuestionDisplayItem
    {
        public int QuestionId { get; set; }
        public int No { get; set; }
        public string QuestionText { get; set; }
        public int Points { get; set; }
        public string Difficulty { get; set; }
        public string CognitiveLevel { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
