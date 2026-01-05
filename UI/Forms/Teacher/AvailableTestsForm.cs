using System;
using System.Drawing;
using System.Windows.Forms;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.DTOs;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    /// <summary>
    /// Form for students to view and select available test sessions
    /// Shows only active test instances within their time window
    /// </summary>
    public partial class AvailableTestsForm : Form
    {
        private TestRepository testRepository;
        private Label lblTitle;
        private Label lblInstructions;
        private DataGridView dgvAvailableTests;
        private Button btnTakeTest;
        private Button btnRefresh;
        private Button btnClose;
        private Label lblStatus;

        public AvailableTestsForm()
        {
            testRepository = new TestRepository();
            InitializeComponent();
            LoadAvailableTests();
        }

        private void InitializeComponent()
        {
            this.Text = "Available Tests";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int yPos = 20;

            // Title
            lblTitle = new Label
            {
                Text = "📝 Available Test Sessions",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, yPos),
                Size = new Size(950, 30),
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(lblTitle);
            yPos += 40;

            // Instructions
            lblInstructions = new Label
            {
                Text = "Select a test session below to view details and start taking the test. " +
                       "Only tests that are currently active and within their scheduled time window are shown.",
                Location = new Point(20, yPos),
                Size = new Size(950, 30),
                Font = new Font("Arial", 9),
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblInstructions);
            yPos += 40;

            // DataGridView for available tests
            dgvAvailableTests = new DataGridView
            {
                Location = new Point(20, yPos),
                Size = new Size(940, 350),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                Font = new Font("Arial", 9)
            };
            // ❌ REMOVED: SetupDataGridColumns() - columns will be set in LoadAvailableTests()
            this.Controls.Add(dgvAvailableTests);
            yPos += 360;

            // Status Label
            lblStatus = new Label
            {
                Text = "Loading available tests...",
                Location = new Point(20, yPos),
                Size = new Size(600, 20),
                Font = new Font("Arial", 9),
                ForeColor = Color.DarkGreen
            };
            this.Controls.Add(lblStatus);

            // Buttons
            btnTakeTest = new Button
            {
                Text = "📝 View Test",
                Location = new Point(650, yPos - 5),
                Size = new Size(150, 35),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTakeTest.Click += BtnTakeTest_Click;
            this.Controls.Add(btnTakeTest);

            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(810, yPos - 5),
                Size = new Size(80, 35),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 9),
                Cursor = Cursors.Hand
            };
            btnRefresh.Click += BtnRefresh_Click;
            this.Controls.Add(btnRefresh);

            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(900, yPos - 5),
                Size = new Size(60, 35),
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
                lblStatus.Text = "Loading available tests...";
                lblStatus.ForeColor = Color.DarkGreen;
                dgvAvailableTests.Columns.Clear();

                var availableTests = testRepository.GetAvailableTestInstancesForStudents();

                if (availableTests == null || availableTests.Count == 0)
                {
                    lblStatus.Text = "No test sessions are currently available. Check back later!";
                    lblStatus.ForeColor = Color.DarkOrange;
                    return;
                }

                // Configure DataGridView columns
                dgvAvailableTests.AutoGenerateColumns = false;

                //  Hidden InstanceId Column (PRIMARY KEY)
                dgvAvailableTests.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "InstanceId",
                    HeaderText = "InstanceId",
                    Name = "InstanceId",
                    Visible = false  // Hidden but accessible
                });

                //  Test Name Column (InstanceTitle from DTO)
                dgvAvailableTests.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "InstanceTitle",
                    HeaderText = "Test Name",
                    Name = "InstanceTitle",
                    Width = 200,
                    ReadOnly = true
                });

                // Subject Column
                dgvAvailableTests.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "SubjectName",
                    HeaderText = "Subject",
                    Name = "SubjectName",
                    Width = 150,
                    ReadOnly = true
                });

                //  Teacher Column
                dgvAvailableTests.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "TeacherName",
                    HeaderText = "Teacher",
                    Name = "TeacherName",
                    Width = 150,
                    ReadOnly = true
                });

                //  Start Date Column
                dgvAvailableTests.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "StartDate",
                    HeaderText = "Start Date",
                    Name = "StartDate",
                    Width = 120,
                    ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MMM dd, yyyy HH:mm" }
                });

                //  End Date Column
                dgvAvailableTests.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "EndDate",
                    HeaderText = "End Date",
                    Name = "EndDate",
                    Width = 120,
                    ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MMM dd, yyyy HH:mm" }
                });

                //  Duration Column
                dgvAvailableTests.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "DurationMinutes",
                    HeaderText = "Duration (min)",
                    Name = "DurationMinutes",
                    Width = 100,
                    ReadOnly = true
                });

                //  Total Points Column
                dgvAvailableTests.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "TotalPoints",
                    HeaderText = "Total Points",
                    Name = "TotalPoints",
                    Width = 100,
                    ReadOnly = true
                });

                // Bind data to DataGridView
                dgvAvailableTests.DataSource = availableTests;

                // Update status
                lblStatus.Text = $"Found {availableTests.Count} available test(s)";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading tests";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"ERROR: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Error Details", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTakeTest_Click(object sender, EventArgs e)
        {
            if (dgvAvailableTests.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a test session to view.",
                    "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                //  Get values using CORRECT column names
                int instanceId = Convert.ToInt32(dgvAvailableTests.SelectedRows[0].Cells["InstanceId"].Value);
                string instanceTitle = dgvAvailableTests.SelectedRows[0].Cells["InstanceTitle"].Value.ToString();
                string subjectName = dgvAvailableTests.SelectedRows[0].Cells["SubjectName"].Value.ToString();
                string teacherName = dgvAvailableTests.SelectedRows[0].Cells["TeacherName"].Value.ToString();
                int duration = Convert.ToInt32(dgvAvailableTests.SelectedRows[0].Cells["DurationMinutes"].Value);
                int totalPoints = Convert.ToInt32(dgvAvailableTests.SelectedRows[0].Cells["TotalPoints"].Value);

                // Show confirmation dialog with test details
                DialogResult result = MessageBox.Show(
                    $"You are about to start:\n\n" +
                    $"Test: {instanceTitle}\n" +
                    $"Subject: {subjectName}\n" +
                    $"Teacher: {teacherName}\n" +
                    $"Duration: {duration} minutes\n" +
                    $"Total Points: {totalPoints}\n\n" +
                    $"Are you ready to begin?",
                    "Confirm Test Start",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // TODO: Open TakeTestForm with instanceId
                    MessageBox.Show(
                        $"Test Taking feature will be implemented next!\n\n" +
                        $"Instance ID: {instanceId}\n" +
                        $"Test: {instanceTitle}\n" +
                        $"Subject: {subjectName}",
                        "Coming Soon",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting test: {ex.Message}\n\nStack: {ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadAvailableTests();
        }
    }
}
