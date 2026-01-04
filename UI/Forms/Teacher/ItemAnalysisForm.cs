using StudentAssessmentSystem.BusinessLogic.Analysis;
using StudentAssessmentSystem.DataAccess;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Results;
using StudentAssessmentSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    public partial class ItemAnalysisForm : Form
    {
        private ItemAnalyzer _analyzer;
        private TestRepository _testRepository;
        private TestResultRepository _resultRepository;

        // UI Controls
        private Label lblTitle;
        private ComboBox cmbTests;
        private Button btnAnalyze;
        private DataGridView dgvAnalysis;
        private Label lblSummary;
        private Button btnClose;
        private Label lblInstructions;

        public ItemAnalysisForm()
        {
            _analyzer = new ItemAnalyzer();
            _testRepository = new TestRepository();
            _resultRepository = new TestResultRepository();

            InitializeComponent();
            LoadTests();
        }

        private void InitializeComponent()
        {
            this.Text = "Item Analysis Report";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            int yPos = 20;

            // Title
            lblTitle = new Label();
            lblTitle.Text = "Item Analysis - Test Quality Evaluation";
            lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            lblTitle.Location = new Point(20, yPos);
            lblTitle.Size = new Size(950, 30);
            this.Controls.Add(lblTitle);
            yPos += 40;

            // Instructions
            lblInstructions = new Label();
            lblInstructions.Text = "Select a test that has been completed by at least 30 students for valid statistical analysis.";
            lblInstructions.Location = new Point(20, yPos);
            lblInstructions.Size = new Size(950, 20);
            lblInstructions.ForeColor = Color.Gray;
            this.Controls.Add(lblInstructions);
            yPos += 30;

            // Test Selection
            Label lblTest = new Label();
            lblTest.Text = "Select Test:";
            lblTest.Location = new Point(20, yPos);
            lblTest.Size = new Size(100, 20);
            lblTest.Font = new Font("Arial", 10, FontStyle.Bold);
            this.Controls.Add(lblTest);

            cmbTests = new ComboBox();
            cmbTests.Location = new Point(130, yPos);
            cmbTests.Size = new Size(650, 25);
            cmbTests.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(cmbTests);

            btnAnalyze = new Button();
            btnAnalyze.Text = "Analyze Test";
            btnAnalyze.Location = new Point(800, yPos - 2);
            btnAnalyze.Size = new Size(150, 30);
            btnAnalyze.BackColor = Color.LightGreen;
            btnAnalyze.Font = new Font("Arial", 10, FontStyle.Bold);
            btnAnalyze.Cursor = Cursors.Hand;
            btnAnalyze.Click += BtnAnalyze_Click;
            this.Controls.Add(btnAnalyze);
            yPos += 50;

            // Data Grid View
            dgvAnalysis = new DataGridView();
            dgvAnalysis.Location = new Point(20, yPos);
            dgvAnalysis.Size = new Size(940, 350);
            dgvAnalysis.AllowUserToAddRows = false;
            dgvAnalysis.AllowUserToDeleteRows = false;
            dgvAnalysis.ReadOnly = true;
            dgvAnalysis.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAnalysis.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAnalysis.BackgroundColor = Color.White;
            dgvAnalysis.RowHeadersVisible = false;
            this.Controls.Add(dgvAnalysis);
            yPos += 360;

            // Summary Label
            lblSummary = new Label();
            lblSummary.Text = "Select a test and click 'Analyze Test' to view item analysis.";
            lblSummary.Location = new Point(20, yPos);
            lblSummary.Size = new Size(940, 60);
            lblSummary.Font = new Font("Arial", 10);
            lblSummary.ForeColor = Color.DarkBlue;
            this.Controls.Add(lblSummary);
            yPos += 70;

            // Close Button
            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Location = new Point(860, yPos);
            btnClose.Size = new Size(100, 35);
            btnClose.BackColor = Color.LightGray;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            // Setup DataGridView columns
            SetupDataGridColumns();
        }

        private void SetupDataGridColumns()
        {
            dgvAnalysis.Columns.Clear();

            dgvAnalysis.Columns.Add("QuestionNum", "Q#");
            dgvAnalysis.Columns[0].Width = 50;

            dgvAnalysis.Columns.Add("QuestionText", "Question");
            dgvAnalysis.Columns[1].Width = 300;

            dgvAnalysis.Columns.Add("TotalAttempts", "Total Students");
            dgvAnalysis.Columns[2].Width = 100;

            dgvAnalysis.Columns.Add("CorrectCount", "Correct Answers");
            dgvAnalysis.Columns[3].Width = 120;

            dgvAnalysis.Columns.Add("DifficultyIndex", "Difficulty (P)");
            dgvAnalysis.Columns[4].Width = 100;

            dgvAnalysis.Columns.Add("DifficultyCategory", "Difficulty Level");
            dgvAnalysis.Columns[5].Width = 120;

            dgvAnalysis.Columns.Add("DiscriminationIndex", "Discrimination (D)");
            dgvAnalysis.Columns[6].Width = 140;

            dgvAnalysis.Columns.Add("DiscriminationQuality", "Quality");
            dgvAnalysis.Columns[7].Width = 120;

            dgvAnalysis.Columns.Add("Status", "Status");
            dgvAnalysis.Columns[8].Width = 100;
        }

        private void LoadTests()
        {
            try
            {
                int teacherId = SessionManager.GetCurrentUserId();
                var tests = _testRepository.GetTestsByTeacher(teacherId);

                cmbTests.Items.Clear();
                cmbTests.DisplayMember = "TestTitle";
                cmbTests.ValueMember = "TestId";

                foreach (var test in tests)
                {
                    cmbTests.Items.Add(new { TestTitle = test.TestTitle, TestId = test.TestId });
                }

                if (cmbTests.Items.Count > 0)
                {
                    cmbTests.SelectedIndex = 0;
                }
                else
                {
                    lblSummary.Text = "No tests found. Please create a test first.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tests:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAnalyze_Click(object sender, EventArgs e)
        {
            if (cmbTests.SelectedItem == null)
            {
                MessageBox.Show("Please select a test to analyze.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnAnalyze.Enabled = false;
                btnAnalyze.Text = "Analyzing...";
                this.Cursor = Cursors.WaitCursor;

                // Get selected test ID
                dynamic selectedItem = cmbTests.SelectedItem;
                int testId = selectedItem.TestId;

                //  dummy test instance
                int testInstanceId = CreateDemoTestInstance(testId);

                if (testInstanceId == 0)
                {
                    MessageBox.Show("No test administrations found for this test.\n\n" +
                        "Note: Tests must be administered to students before analysis can be performed.",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Perform analysis
                PerformAnalysis(testInstanceId);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Not enough data"))
                {
                    MessageBox.Show(
                        "Not enough student responses for statistical analysis.\n\n" +
                        "Item analysis requires at least 30 students to have completed the test.\n\n" +
                        "This is a statistical requirement for valid difficulty and discrimination indices.",
                        "Insufficient Data",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else
                {
                    MessageBox.Show($"Error performing analysis:\n{ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnAnalyze.Enabled = true;
                btnAnalyze.Text = "Analyze Test";
                this.Cursor = Cursors.Default;
            }
        }

       
        /// Gets the first available test instance for the given test
        /// this should allow teachers to select which administration to analyze
        private int CreateDemoTestInstance(int testId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = @"
                SELECT InstanceId 
                FROM TestInstances 
                WHERE TestId = @TestId 
                  AND IsActive = 1
                ORDER BY StartDateTime DESC 
                LIMIT 1";

                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TestId", testId);
                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving test instance:\n{ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return 0; // No instance found
        }


        private void PerformAnalysis(int testInstanceId)
        {
            // Get all questions for this test
            var test = _testRepository.GetTestByInstanceId(testInstanceId);

            if (test == null || test.Questions == null || test.Questions.Count == 0)
            {
                MessageBox.Show("No questions found for this test.", "No Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Analyze all questions
            List<QuestionStatistics> statisticsList = _analyzer.AnalyzeAllQuestions(testInstanceId);

            if (statisticsList.Count == 0)
            {
                MessageBox.Show("No student responses found for this test.\n\n" +
                    "Students must complete the test before analysis can be performed.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Display results
            DisplayResults(test, statisticsList);
        }

        private void DisplayResults(Test test, List<QuestionStatistics> statisticsList)
        {
            dgvAnalysis.Rows.Clear();

            int questionNumber = 1;
            int needsReviewCount = 0;

            foreach (var stats in statisticsList)
            {
                // Find the question
                var question = test.Questions.Find(q => q.QuestionId == stats.QuestionId);
                string questionText = question != null ?
                    (question.QuestionText.Length > 50 ? question.QuestionText.Substring(0, 50) + "..." : question.QuestionText)
                    : "Unknown";

                // Difficulty category and interpretation
                string difficultyCategory = stats.GetDifficultyCategory();
                string discriminationQuality = stats.GetDiscriminationQuality();
                string status = stats.NeedsReview() ? "⚠ REVIEW" : "✓ Good";

                if (stats.NeedsReview())
                    needsReviewCount++;

                // Add row
                int rowIndex = dgvAnalysis.Rows.Add(
                    questionNumber,
                    questionText,
                    stats.TotalAttempts,
                    stats.CorrectCount,
                    stats.DifficultyIndex.ToString("F4"),
                    difficultyCategory,
                    stats.DiscriminationIndex.HasValue ? stats.DiscriminationIndex.Value.ToString("F4") : "N/A",
                    discriminationQuality,
                    status
                );

                // Color code the status
                if (stats.NeedsReview())
                {
                    dgvAnalysis.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                    dgvAnalysis.Rows[rowIndex].Cells["Status"].Style.ForeColor = Color.Red;
                    dgvAnalysis.Rows[rowIndex].Cells["Status"].Style.Font = new Font(dgvAnalysis.Font, FontStyle.Bold);
                }
                else
                {
                    dgvAnalysis.Rows[rowIndex].Cells["Status"].Style.ForeColor = Color.Green;
                }

                // Color code difficulty
                if (stats.DifficultyIndex > 0.75m)
                    dgvAnalysis.Rows[rowIndex].Cells["DifficultyCategory"].Style.BackColor = Color.LightGreen;
                else if (stats.DifficultyIndex < 0.25m)
                    dgvAnalysis.Rows[rowIndex].Cells["DifficultyCategory"].Style.BackColor = Color.LightCoral;

                // Color code discrimination
                if (stats.DiscriminationIndex.HasValue)
                {
                    if (stats.DiscriminationIndex.Value >= 0.40m)
                        dgvAnalysis.Rows[rowIndex].Cells["DiscriminationQuality"].Style.BackColor = Color.LightGreen;
                    else if (stats.DiscriminationIndex.Value < 0.20m)
                        dgvAnalysis.Rows[rowIndex].Cells["DiscriminationQuality"].Style.BackColor = Color.LightCoral;
                }

                questionNumber++;
            }

            // Update summary
            UpdateSummary(test.TestTitle, statisticsList.Count, needsReviewCount);
        }

        private void UpdateSummary(string testTitle, int totalQuestions, int needsReviewCount)
        {
            lblSummary.Text = $"📊 Analysis Complete for: {testTitle}\n\n" +
                $"Total Questions: {totalQuestions} | " +
                $"Good Questions: {totalQuestions - needsReviewCount} | " +
                $"⚠ Need Review: {needsReviewCount}";

            if (needsReviewCount > 0)
            {
                lblSummary.Text += $"\n\n Questions marked with '⚠ REVIEW' are either too easy/hard or don't discriminate well between high and low performers.";
                lblSummary.ForeColor = Color.DarkOrange;
            }
            else
            {
                lblSummary.Text += $"\n\n✓ All questions meet quality standards!";
                lblSummary.ForeColor = Color.Green;
            }
        }
    }
}