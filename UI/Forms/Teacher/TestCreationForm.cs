using StudentAssessmentSystem.BusinessLogic.Managers;
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

                // IMPORTANT: Your TestManager expects questions!
                // For now, we create an empty list (questions will be added later)
                _newTest.Questions = new List<Question>();

                // Save to database - WITH out parameter
                int testId = _testManager.CreateTest(_newTest, out string errorMessage);

                if (testId > 0)
                {
                    MessageBox.Show(
                        $"Test '{_newTest.TestTitle}' created successfully!\n\n" +
                        $"Test ID: {testId}\n" +
                        "Now you can add questions to this test.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // TODO: Open AddQuestionForm to add questions
                    // AddQuestionForm questionForm = new AddQuestionForm(testId);
                    // questionForm.ShowDialog();

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        $"Failed to create test.\n\n{errorMessage}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating test:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}