using MySql.Data.MySqlClient;
using StudentAssessmentSystem.DataAccess;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using StudentModel = StudentAssessmentSystem.Models.Users.Student;

namespace StudentAssessmentSystem.UI.Forms.Student
{
    public partial class AvailableTestsForm : Form
    {
        private StudentModel _currentStudent;
        private DataGridView dgvTests;
        private Label lblStatus;  // ✅ ADDED - This was missing
        private Button btnTakeTest;
        private Button btnClose;

        public AvailableTestsForm(StudentModel currentStudent)
        {
            _currentStudent = currentStudent;
            InitializeComponent();
            LoadAvailableTests();
        }

        private void InitializeComponent()
        {
            this.Text = "Available Tests";
            this.Size = new Size(1000, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // Title Label
            Label lblTitle = new Label
            {
                Text = "📝 Available Tests",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(30, 20),
                Size = new Size(400, 30),
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(lblTitle);

            // ✅ Status Label (was missing)
            lblStatus = new Label
            {
                Text = "Loading...",
                Font = new Font("Arial", 10),
                Location = new Point(30, 55),
                Size = new Size(900, 25),
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblStatus);

            // ✅ DataGridView for tests (matching variable name)
            dgvTests = new DataGridView
            {
                Location = new Point(30, 90),
                Size = new Size(930, 350),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                AutoGenerateColumns = false  // ✅ IMPORTANT
            };

            // ✅ Setup columns properly
            dgvTests.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "InstanceId",
                DataPropertyName = "InstanceId",
                HeaderText = "Instance ID",
                Visible = false
            });

            dgvTests.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TestTitle",
                DataPropertyName = "TestTitle",
                HeaderText = "Test Title",
                Width = 250
            });

            dgvTests.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Subject",
                DataPropertyName = "Subject",
                HeaderText = "Subject",
                Width = 150
            });

            dgvTests.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Questions",
                DataPropertyName = "Questions",
                HeaderText = "Questions",
                Width = 100
            });

            dgvTests.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Duration",
                DataPropertyName = "Duration",
                HeaderText = "Duration",
                Width = 100
            });

            dgvTests.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StartDate",
                DataPropertyName = "StartDate",
                HeaderText = "Available From",
                Width = 150
            });

            dgvTests.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EndDate",
                DataPropertyName = "EndDate",
                HeaderText = "Available Until",
                Width = 150
            });

            this.Controls.Add(dgvTests);

            // Take Test Button
            btnTakeTest = new Button
            {
                Text = "✅ Take Selected Test",
                Location = new Point(690, 460),
                Size = new Size(180, 40),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnTakeTest.Click += BtnTakeTest_Click;
            this.Controls.Add(btnTakeTest);

            // Close Button
            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(880, 460),
                Size = new Size(80, 40),
                BackColor = Color.LightGray,
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            // Enable button when row selected
            dgvTests.SelectionChanged += (s, e) =>
            {
                btnTakeTest.Enabled = dgvTests.SelectedRows.Count > 0;
            };
        }

        private void LoadAvailableTests()
        {
            try
            {
                lblStatus.Text = "Loading available tests...";
                lblStatus.ForeColor = Color.Blue;

                // ✅ Get student's enrolled sections
                var studentRepo = new StudentRepository();
                var enrolledSections = studentRepo.GetEnrolledSectionIds(_currentStudent.StudentId);

                if (enrolledSections == null || enrolledSections.Count == 0)
                {
                    lblStatus.Text = "⚠️ You are not enrolled in any sections. Please contact your administrator.";
                    lblStatus.ForeColor = Color.Orange;
                    dgvTests.DataSource = null;
                    btnTakeTest.Enabled = false;
                    return;
                }

                // ✅ Get test instances for student's sections
                var instanceRepo = new TestInstanceRepository();
                var instances = instanceRepo.GetActiveTestInstancesByStudentSections(
                    _currentStudent.StudentId,
                    enrolledSections
                );

                if (instances == null || instances.Count == 0)
                {
                    lblStatus.Text = "📭 No tests available right now for your sections.";
                    lblStatus.ForeColor = Color.Gray;
                    dgvTests.DataSource = null;
                    btnTakeTest.Enabled = false;
                    return;
                }

                // ✅ Get question counts for each test
                var questionRepo = new QuestionRepository();
                var displayData = new List<object>();

                foreach (var instance in instances)
                {
                    var questions = questionRepo.GetQuestionsByTest(instance.TestId);
                    int questionCount = questions?.Count ?? 0;

                    // Only show tests with questions
                    if (questionCount > 0)
                    {
                        displayData.Add(new
                        {
                            InstanceId = instance.InstanceId,
                            TestTitle = instance.TestTitle ?? instance.InstanceTitle,
                            Subject = instance.SubjectName ?? "N/A",
                            Questions = $"{questionCount} questions",
                            Duration = $"{instance.DurationMinutes} min",
                            StartDate = instance.StartDate.ToString("MMM dd, yyyy hh:mm tt"),
                            EndDate = instance.EndDate.ToString("MMM dd, yyyy hh:mm tt")
                        });
                    }
                }

                if (displayData.Count == 0)
                {
                    lblStatus.Text = "⚠️ Available tests have no questions yet. Please check back later.";
                    lblStatus.ForeColor = Color.Orange;
                    dgvTests.DataSource = null;
                    btnTakeTest.Enabled = false;
                    return;
                }

                dgvTests.DataSource = displayData;
                lblStatus.Text = $"✅ {displayData.Count} test(s) available for you!";
                lblStatus.ForeColor = Color.Green;
                btnTakeTest.Enabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Error loading tests: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                dgvTests.DataSource = null;
                btnTakeTest.Enabled = false;
            }
        }

        private void BtnTakeTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvTests.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a test to take.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = dgvTests.SelectedRows[0];
                object instanceIdValue = selectedRow.Cells["InstanceId"].Value;

                if (instanceIdValue == null || instanceIdValue == DBNull.Value)
                {
                    MessageBox.Show("Invalid test instance. Please refresh and try again.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(instanceIdValue.ToString(), out int instanceId))
                {
                    MessageBox.Show($"Invalid instance ID format: {instanceIdValue}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string testTitle = selectedRow.Cells["TestTitle"].Value?.ToString() ?? "Unknown";
                string duration = selectedRow.Cells["Duration"].Value?.ToString() ?? "N/A";
                string questions = selectedRow.Cells["Questions"].Value?.ToString() ?? "N/A";

                DialogResult confirm = MessageBox.Show(
                    $"You are about to take:\n\n" +
                    $"📝 {testTitle}\n" +
                    $"⏱️ Duration: {duration}\n" +
                    $"❓ {questions}\n\n" +
                    $"Once you start, the timer will begin.\n\n" +
                    $"Are you ready to proceed?",
                    "Confirm Test",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.No)
                    return;

                // Open test taking form
                var testForm = new TestTakingForm(_currentStudent, instanceId);
                this.Hide();

                DialogResult result = testForm.ShowDialog();

                this.Show();

                if (result == DialogResult.OK)
                {
                    MessageBox.Show("Test submitted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAvailableTests();
                }
            }
            catch (Exception ex)
            {
                this.Show();
                MessageBox.Show($"Error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}