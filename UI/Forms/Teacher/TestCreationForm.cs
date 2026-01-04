using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.DataAccess;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Enums;
using StudentAssessmentSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    public partial class TestCreationForm : Form
    {
        private TestManager _testManager;
        private Test _newTest;

        // UI Controls
        private Label lblTitle;
        private TextBox txtTestTitle;
        private Label lblDescription;
        private TextBox txtDescription;
        private Label lblTestType;
        private ComboBox cmbTestType;
        private Label lblDuration;
        private NumericUpDown numDuration;
        private Label lblPassingScore;
        private NumericUpDown numPassingScore;
        private Button btnSave;
        private Button btnCancel;

        public TestCreationForm()
        {
            _testManager = new TestManager();
            _newTest = new Test();

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Create New Test";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int yPos = 20;

            // Test Title
            lblTitle = new Label();
            lblTitle.Text = "Test Title:";
            lblTitle.Location = new Point(20, yPos);
            lblTitle.Size = new Size(100, 20);
            this.Controls.Add(lblTitle);

            txtTestTitle = new TextBox();
            txtTestTitle.Location = new Point(20, yPos + 25);
            txtTestTitle.Size = new Size(440, 25);
            txtTestTitle.Font = new Font("Arial", 10);
            this.Controls.Add(txtTestTitle);

            yPos += 60;

            // Description
            lblDescription = new Label();
            lblDescription.Text = "Description:";
            lblDescription.Location = new Point(20, yPos);
            lblDescription.Size = new Size(100, 20);
            this.Controls.Add(lblDescription);

            txtDescription = new TextBox();
            txtDescription.Location = new Point(20, yPos + 25);
            txtDescription.Size = new Size(440, 60);
            txtDescription.Multiline = true;
            txtDescription.ScrollBars = ScrollBars.Vertical;
            txtDescription.Font = new Font("Arial", 10);
            this.Controls.Add(txtDescription);

            yPos += 100;

            // Test Type
            lblTestType = new Label();
            lblTestType.Text = "Test Type:";
            lblTestType.Location = new Point(20, yPos);
            lblTestType.Size = new Size(100, 20);
            this.Controls.Add(lblTestType);

            cmbTestType = new ComboBox();
            cmbTestType.Location = new Point(20, yPos + 25);
            cmbTestType.Size = new Size(200, 25);
            cmbTestType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTestType.Items.AddRange(new string[] { "Quiz", "Exam", "Diagnostic", "Practice" });
            cmbTestType.SelectedIndex = 0;
            this.Controls.Add(cmbTestType);

            yPos += 60;

            // Duration
            lblDuration = new Label();
            lblDuration.Text = "Duration (minutes):";
            lblDuration.Location = new Point(20, yPos);
            lblDuration.Size = new Size(150, 20);
            this.Controls.Add(lblDuration);

            numDuration = new NumericUpDown();
            numDuration.Location = new Point(20, yPos + 25);
            numDuration.Size = new Size(100, 25);
            numDuration.Minimum = 5;
            numDuration.Maximum = 300;
            numDuration.Value = 60;
            this.Controls.Add(numDuration);

            // Passing Score
            lblPassingScore = new Label();
            lblPassingScore.Text = "Passing Score (%):";
            lblPassingScore.Location = new Point(250, yPos);
            lblPassingScore.Size = new Size(150, 20);
            this.Controls.Add(lblPassingScore);

            numPassingScore = new NumericUpDown();
            numPassingScore.Location = new Point(250, yPos + 25);
            numPassingScore.Size = new Size(100, 25);
            numPassingScore.Minimum = 50;
            numPassingScore.Maximum = 100;
            numPassingScore.Value = 60;
            this.Controls.Add(numPassingScore);

            yPos += 70;

            // Save Button
            btnSave = new Button();
            btnSave.Text = "Save Test";
            btnSave.Location = new Point(260, yPos);
            btnSave.Size = new Size(100, 35);
            btnSave.BackColor = Color.LightGreen;
            btnSave.Font = new Font("Arial", 10, FontStyle.Bold);
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            // Cancel Button
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(370, yPos);
            btnCancel.Size = new Size(90, 35);
            btnCancel.BackColor = Color.LightGray;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTestTitle.Text))
            {
                MessageBox.Show("Please enter a test title.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTestTitle.Focus();
                return;
            }

            try
            {
                // Fill test object
                _newTest.TestTitle = txtTestTitle.Text.Trim();
                _newTest.Description = txtDescription.Text.Trim();
                _newTest.TestType = (TestType)Enum.Parse(typeof(TestType), cmbTestType.SelectedItem.ToString());
                _newTest.DurationMinutes = (int)numDuration.Value;
                _newTest.PassingScore = numPassingScore.Value;
                _newTest.TeacherId = SessionManager.GetCurrentUserId();
                _newTest.SubjectId = 1; // TODO: Let teacher select subject
                _newTest.TotalPoints = 0; // Will be calculated when questions are added
                _newTest.CreatedDate = DateTime.Now;
                _newTest.IsActive = true;

                // ✅ Create empty list (questions will be added next)
                _newTest.Questions = new List<Question>();

                // ✅ BYPASS validation temporarily - save test WITHOUT questions first
                // We'll add questions in the next step
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO Tests 
                           (SubjectId, TeacherId, TestTitle, Description, TestType, TotalPoints, 
                            PassingScore, DurationMinutes, Instructions, RandomizeQuestions, 
                            RandomizeChoices, ShowCorrectAnswers, AllowReview, CreatedDate, IsActive)
                           VALUES 
                           (@SubjectId, @TeacherId, @TestTitle, @Description, @TestType, @TotalPoints,
                            @PassingScore, @DurationMinutes, @Instructions, @RandomizeQuestions,
                            @RandomizeChoices, @ShowCorrectAnswers, @AllowReview, @CreatedDate, @IsActive);
                           SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SubjectId", _newTest.SubjectId);
                        cmd.Parameters.AddWithValue("@TeacherId", _newTest.TeacherId);
                        cmd.Parameters.AddWithValue("@TestTitle", _newTest.TestTitle);
                        cmd.Parameters.AddWithValue("@Description", _newTest.Description ?? "");
                        cmd.Parameters.AddWithValue("@TestType", _newTest.TestType.ToString());
                        cmd.Parameters.AddWithValue("@TotalPoints", 0); // Will be updated later
                        cmd.Parameters.AddWithValue("@PassingScore", _newTest.PassingScore);
                        cmd.Parameters.AddWithValue("@DurationMinutes", _newTest.DurationMinutes);
                        cmd.Parameters.AddWithValue("@Instructions", _newTest.Instructions ?? "");
                        cmd.Parameters.AddWithValue("@RandomizeQuestions", _newTest.RandomizeQuestions);
                        cmd.Parameters.AddWithValue("@RandomizeChoices", _newTest.RandomizeChoices);
                        cmd.Parameters.AddWithValue("@ShowCorrectAnswers", _newTest.ShowCorrectAnswers);
                        cmd.Parameters.AddWithValue("@AllowReview", _newTest.AllowReview);
                        cmd.Parameters.AddWithValue("@CreatedDate", _newTest.CreatedDate);
                        cmd.Parameters.AddWithValue("@IsActive", _newTest.IsActive);

                        int testId = Convert.ToInt32(cmd.ExecuteScalar());

                        if (testId > 0)
                        {
                            // ✅ Now open AddQuestionForm to add questions
                            DialogResult result = MessageBox.Show(
                                $"Test '{_newTest.TestTitle}' created successfully!\n\n" +
                                $"Test ID: {testId}\n\n" +
                                "Would you like to add questions now?",
                                "Success",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question
                            );

                            if (result == DialogResult.Yes)
                            {
                                // Open AddQuestionForm
                                AddQuestionForm questionForm = new AddQuestionForm(testId, 1);
                                questionForm.ShowDialog();
                            }

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Failed to create test.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating test:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}