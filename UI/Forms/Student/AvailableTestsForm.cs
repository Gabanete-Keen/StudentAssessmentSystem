using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Utilities;

namespace StudentAssessmentSystem.UI.Forms.Student
{
    /// <summary>
    /// Shows all available tests that student can take
    /// </summary>
    public partial class AvailableTestsForm : Form
    {
        private TestRepository _testRepository;
        private TestResultRepository _resultRepository;
        private ListView lvAvailableTests;
        private Button btnTakeTest;
        private Button btnClose;

        public AvailableTestsForm()
        {
            _testRepository = new TestRepository();
            _resultRepository = new TestResultRepository();

            InitializeComponent();
            LoadAvailableTests();
        }

        private void InitializeComponent()
        {
            this.Text = "Available Tests";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // Header Label
            Label lblHeader = new Label
            {
                Location = new Point(30, 20),
                Size = new Size(840, 40),
                Text = "📝 Available Tests",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            this.Controls.Add(lblHeader);

            // ListView to display tests
            lvAvailableTests = new ListView
            {
                Location = new Point(30, 80),
                Size = new Size(840, 400),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Arial", 10)
            };

            // Add columns
            lvAvailableTests.Columns.Add("Instance ID", 80);
            lvAvailableTests.Columns.Add("Test Title", 300);
            lvAvailableTests.Columns.Add("Subject", 150);
            lvAvailableTests.Columns.Add("Questions", 100);
            lvAvailableTests.Columns.Add("Duration", 100);
            lvAvailableTests.Columns.Add("Status", 100);

            lvAvailableTests.SelectedIndexChanged += LvAvailableTests_SelectedIndexChanged;
            this.Controls.Add(lvAvailableTests);

            // Take Test Button
            btnTakeTest = new Button
            {
                Location = new Point(620, 500),
                Size = new Size(120, 40),
                Text = "Take Test",
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnTakeTest.Click += BtnTakeTest_Click;
            this.Controls.Add(btnTakeTest);

            // Close Button
            btnClose = new Button
            {
                Location = new Point(750, 500),
                Size = new Size(120, 40),
                Text = "Close",
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadAvailableTests()
        {
            try
            {
                lvAvailableTests.Items.Clear();

                // Get all active test instances
                List<TestInstance> testInstances = null;

                try
                {
                    testInstances = _testRepository.GetActiveTestInstances();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error getting test instances:\n{ex.Message}\n\nTry creating a test first.",
                        "No Tests Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (testInstances == null || testInstances.Count == 0)
                {
                    MessageBox.Show("No active tests available at this time.\n\nPlease contact your teacher.",
                        "No Tests", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int studentId = SessionManager.GetCurrentUserId();

                foreach (var instance in testInstances)
                {
                    try
                    {
                        // Check if student already took this test
                        var existingResult = _resultRepository.GetResultByStudentAndInstance(
                            studentId, instance.InstanceId);

                        // Skip if already taken and completed
                        if (existingResult != null && existingResult.IsCompleted)
                            continue;

                        // Get test details
                        var test = _testRepository.GetTestById(instance.TestId);
                        if (test == null)
                        {
                            Console.WriteLine($"Test ID {instance.TestId} not found. Skipping...");
                            continue;
                        }

                        // Create list item
                        ListViewItem item = new ListViewItem(instance.InstanceId.ToString());
                        item.SubItems.Add(test.Title ?? "Untitled Test");
                        item.SubItems.Add(test.SubjectName ?? "N/A");

                        // Handle null Questions list
                        int questionCount = (test.Questions != null) ? test.Questions.Count : 0;
                        item.SubItems.Add(questionCount.ToString());

                        item.SubItems.Add($"{test.DurationMinutes} mins");
                        item.SubItems.Add("Available");
                        item.Tag = instance; // Store the instance object
                        item.BackColor = Color.LightGreen;

                        lvAvailableTests.Items.Add(item);
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue with other tests
                        Console.WriteLine($"Error loading test instance {instance.InstanceId}: {ex.Message}");
                        continue;
                    }
                }

                if (lvAvailableTests.Items.Count == 0)
                {
                    MessageBox.Show("No tests available for you at this time.\n\nYou may have already completed all active tests.",
                        "No Tests", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tests:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LvAvailableTests_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnTakeTest.Enabled = lvAvailableTests.SelectedItems.Count > 0;
        }

        private void BtnTakeTest_Click(object sender, EventArgs e)
        {
            if (lvAvailableTests.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a test to take.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get selected test instance
                TestInstance selectedInstance = lvAvailableTests.SelectedItems[0].Tag as TestInstance;
                if (selectedInstance == null) return;

                // Get test details for confirmation
                var test = _testRepository.GetTestById(selectedInstance.TestId);

                DialogResult confirmResult = MessageBox.Show(
                    $"You are about to start:\n\n" +
                    $"Test: {test.Title}\n" +
                    $"Duration: {test.DurationMinutes} minutes\n" +
                    $"Questions: {test.Questions?.Count ?? 0}\n\n" +
                    $"⚠️ Once started, the timer cannot be paused!\n\n" +
                    $"Are you ready to begin?",
                    "Start Test Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmResult != DialogResult.Yes)
                    return;

                // Launch TakeTestForm
                TakeTestForm takeTestForm = new TakeTestForm(
                    selectedInstance.TestId,
                    selectedInstance.InstanceId
                );

                this.Hide();
                DialogResult result = takeTestForm.ShowDialog();
                this.Show();

                // Refresh list after test completion
                if (result == DialogResult.OK)
                {
                    LoadAvailableTests();
                    MessageBox.Show("Test completed successfully! Check your results.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                this.Show();
                MessageBox.Show($"Error starting test:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
