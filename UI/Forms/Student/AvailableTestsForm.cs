using MySql.Data.MySqlClient;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using StudentModel = StudentAssessmentSystem.Models.Users.Student;

namespace StudentAssessmentSystem.UI.Forms.Student
{
    public partial class AvailableTestsForm : Form
    {
        private StudentModel _currentStudent;
        private DataGridView dgvTests;
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
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Title Label
            Label lblTitle = new Label
            {
                Text = "📝 Available Tests",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(30, 20),
                Size = new Size(300, 30)
            };
            this.Controls.Add(lblTitle);

            // DataGridView for tests
            dgvTests = new DataGridView
            {
                Location = new Point(30, 60),
                Size = new Size(830, 350),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // ✅ CRITICAL: Add columns properly with DataPropertyName
            DataGridViewTextBoxColumn colInstanceId = new DataGridViewTextBoxColumn
            {
                Name = "InstanceId",
                HeaderText = "Instance ID",
                DataPropertyName = "InstanceId",
                Visible = false  // Hidden but accessible
            };

            DataGridViewTextBoxColumn colTestTitle = new DataGridViewTextBoxColumn
            {
                Name = "TestTitle",
                HeaderText = "Test Title",
                DataPropertyName = "TestTitle"
            };

            DataGridViewTextBoxColumn colSubject = new DataGridViewTextBoxColumn
            {
                Name = "Subject",
                HeaderText = "Subject",
                DataPropertyName = "Subject"
            };

            DataGridViewTextBoxColumn colQuestions = new DataGridViewTextBoxColumn
            {
                Name = "Questions",
                HeaderText = "Questions",
                DataPropertyName = "Questions"
            };

            DataGridViewTextBoxColumn colDuration = new DataGridViewTextBoxColumn
            {
                Name = "Duration",
                HeaderText = "Duration",
                DataPropertyName = "Duration"
            };

            DataGridViewTextBoxColumn colStatus = new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                DataPropertyName = "Status"
            };

            dgvTests.Columns.AddRange(new DataGridViewColumn[] {
                colInstanceId,
                colTestTitle,
                colSubject,
                colQuestions,
                colDuration,
                colStatus
            });

            this.Controls.Add(dgvTests);

            // Take Test Button
            btnTakeTest = new Button
            {
                Text = "Take Test",
                Location = new Point(620, 425),
                Size = new Size(120, 35),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTakeTest.Click += BtnTakeTest_Click;
            this.Controls.Add(btnTakeTest);

            // Close Button
            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(750, 425),
                Size = new Size(110, 35),
                BackColor = Color.LightGray,
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadAvailableTests()
        {
            try
            {
                // Get student's enrolled section IDs
                List<int> sectionIds = _currentStudent.EnrolledSectionIds ?? new List<int>();

                if (sectionIds.Count == 0)
                {
                    MessageBox.Show("You are not enrolled in any sections.",
                        "No Sections", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Load test instances for student's sections
                TestInstanceRepository instanceRepo = new TestInstanceRepository();
                List<TestInstance> instances = instanceRepo.GetActiveTestInstancesByStudentSections(
                    _currentStudent.UserId, sectionIds);

                if (instances.Count == 0)
                {
                    MessageBox.Show("No tests available for you at this time.\n\n" +
                                   "You may have already completed all active tests assigned to your sections.",
                                   "No Tests",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                    return;
                }

                // Clear and populate the DataGridView
                dgvTests.Rows.Clear();

                foreach (var instance in instances)
                {
                    // ✅ PROPER: Add all values in correct order
                    int rowIndex = dgvTests.Rows.Add();
                    DataGridViewRow row = dgvTests.Rows[rowIndex];

                    row.Cells["InstanceId"].Value = instance.InstanceId;
                    row.Cells["TestTitle"].Value = instance.InstanceTitle ?? "Untitled";
                    row.Cells["Subject"].Value = instance.SubjectName ?? "N/A";
                    row.Cells["Questions"].Value = "N/A";  // Can load from question count later
                    row.Cells["Duration"].Value = instance.DurationMinutes + " min";
                    row.Cells["Status"].Value = "Available";
                }

                MessageBox.Show($"Loaded {instances.Count} test(s) successfully!",
                    "Tests Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tests: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTakeTest_Click(object sender, EventArgs e)
        {
            try
            {
                // ✅ Validation
                if (dgvTests.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a test to take.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ✅ Get InstanceId safely with NULL check
                DataGridViewRow selectedRow = dgvTests.SelectedRows[0];
                object instanceIdValue = selectedRow.Cells["InstanceId"].Value;

                if (instanceIdValue == null || instanceIdValue == DBNull.Value)
                {
                    MessageBox.Show("Invalid test instance. Please refresh and try again.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ✅ Parse instance ID
                int instanceId;
                if (!int.TryParse(instanceIdValue.ToString(), out instanceId))
                {
                    MessageBox.Show($"Invalid instance ID format: {instanceIdValue}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ✅ Debug: Show what we got
                string testTitle = selectedRow.Cells["TestTitle"].Value?.ToString() ?? "Unknown";
                DialogResult confirm = MessageBox.Show(
                    $"You are about to take:\n\n{testTitle}\n\nInstance ID: {instanceId}\n\nProceed?",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.No)
                    return;

                // ✅ Open TestTakingForm
                var testForm = new TestTakingForm(_currentStudent, instanceId);
                this.Hide();

                DialogResult result = testForm.ShowDialog();

                this.Show();

                if (result == DialogResult.OK)
                {
                    LoadAvailableTests();  // Refresh list
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
