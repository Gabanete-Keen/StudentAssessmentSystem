using System;
using System.Drawing;
using System.Windows.Forms;
using StudentAssessmentSystem.BusinessLogic.Analysis;
using StudentAssessmentSystem.Models.Results;
using StudentAssessmentSystem.DataAccess.Repositories;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    /// <summary>
    /// Simple form to test ItemAnalyzer functionality
    /// Use this to verify your item analysis calculations work
    /// </summary>
    public partial class ItemAnalyzerTestForm : Form
    {
        private ItemAnalyzer _analyzer;
        private TestResultRepository _resultRepo;

        // UI Controls
        private NumericUpDown numQuestionId;
        private NumericUpDown numInstanceId;
        private Button btnAnalyze;
        private Button btnCheckData;
        private TextBox txtOutput;

        public ItemAnalyzerTestForm()
        {
            _analyzer = new ItemAnalyzer();
            _resultRepo = new TestResultRepository();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Item Analyzer Test Tool - DEBUG MODE";
            this.Size = new Size(750, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "🧪 Test Item Analysis Calculations";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(450, 30);
            lblTitle.ForeColor = Color.DarkBlue;
            this.Controls.Add(lblTitle);

            // Info label
            Label lblInfo = new Label();
            lblInfo.Text = "This tool helps you verify that ItemAnalyzer works correctly with your database.";
            lblInfo.Location = new Point(20, 55);
            lblInfo.Size = new Size(680, 20);
            lblInfo.ForeColor = Color.Gray;
            this.Controls.Add(lblInfo);

            // Question ID Input
            Label lblQuestion = new Label();
            lblQuestion.Text = "Question ID:";
            lblQuestion.Location = new Point(20, 90);
            lblQuestion.Size = new Size(100, 20);
            lblQuestion.Font = new Font("Arial", 9, FontStyle.Bold);
            this.Controls.Add(lblQuestion);

            numQuestionId = new NumericUpDown();
            numQuestionId.Location = new Point(130, 87);
            numQuestionId.Size = new Size(100, 25);
            numQuestionId.Minimum = 1;
            numQuestionId.Maximum = 10000;
            numQuestionId.Value = 1;
            this.Controls.Add(numQuestionId);

            // Test Instance ID Input
            Label lblInstance = new Label();
            lblInstance.Text = "Test Instance ID:";
            lblInstance.Location = new Point(260, 90);
            lblInstance.Size = new Size(120, 20);
            lblInstance.Font = new Font("Arial", 9, FontStyle.Bold);
            this.Controls.Add(lblInstance);

            numInstanceId = new NumericUpDown();
            numInstanceId.Location = new Point(390, 87);
            numInstanceId.Size = new Size(100, 25);
            numInstanceId.Minimum = 1;
            numInstanceId.Maximum = 10000;
            numInstanceId.Value = 1;
            this.Controls.Add(numInstanceId);

            // Check Data Button (NEW - helps debug)
            btnCheckData = new Button();
            btnCheckData.Text = "1️⃣ Check Data";
            btnCheckData.Location = new Point(510, 85);
            btnCheckData.Size = new Size(100, 30);
            btnCheckData.BackColor = Color.LightBlue;
            btnCheckData.Font = new Font("Arial", 9, FontStyle.Bold);
            btnCheckData.Click += BtnCheckData_Click;
            this.Controls.Add(btnCheckData);

            // Analyze Button
            btnAnalyze = new Button();
            btnAnalyze.Text = "2️⃣ Run Analysis";
            btnAnalyze.Location = new Point(620, 85);
            btnAnalyze.Size = new Size(110, 30);
            btnAnalyze.BackColor = Color.LightGreen;
            btnAnalyze.Font = new Font("Arial", 9, FontStyle.Bold);
            btnAnalyze.Click += BtnAnalyze_Click;
            this.Controls.Add(btnAnalyze);

            // Output TextBox
            Label lblOutput = new Label();
            lblOutput.Text = "Output Console:";
            lblOutput.Location = new Point(20, 130);
            lblOutput.Size = new Size(200, 20);
            lblOutput.Font = new Font("Arial", 10, FontStyle.Bold);
            this.Controls.Add(lblOutput);

            txtOutput = new TextBox();
            txtOutput.Location = new Point(20, 155);
            txtOutput.Size = new Size(710, 320);
            txtOutput.Multiline = true;
            txtOutput.ScrollBars = ScrollBars.Vertical;
            txtOutput.Font = new Font("Consolas", 9);
            txtOutput.ReadOnly = true;
            txtOutput.BackColor = Color.Black;
            txtOutput.ForeColor = Color.Lime;
            this.Controls.Add(txtOutput);

            // Close Button
            Button btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Location = new Point(630, 485);
            btnClose.Size = new Size(100, 30);
            btnClose.BackColor = Color.LightGray;
            btnClose.Font = new Font("Arial", 9, FontStyle.Bold);
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        // NEW: Check what data exists before running analysis
        private void BtnCheckData_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();
            Log("════════════════════════════════════════");
            Log("  DATA VALIDATION CHECK");
            Log("════════════════════════════════════════\n");

            int instanceId = (int)numInstanceId.Value;
            int questionId = (int)numQuestionId.Value;

            try
            {
                // Check 1: Test Instance exists
                Log($"Checking Test Instance ID: {instanceId}");
                var results = _resultRepo.GetResultsByInstance(instanceId);

                if (results.Count == 0)
                {
                    Log("❌ ERROR: No test results found for this instance!");
                    Log("   This test instance might not exist or has no completed results.\n");
                    Log("💡 SOLUTION: Run this SQL to see available instances:");
                    Log("   SELECT InstanceId, TestId FROM TestInstances;");
                    return;
                }

                Log($"✓ Found {results.Count} completed test results\n");

                // Check 2: Sample size validation
                if (results.Count < 30)
                {
                    Log($"⚠ WARNING: Only {results.Count} students (need 30 minimum)");
                    Log($"   Item analysis will still run but results may not be statistically valid.\n");
                }
                else
                {
                    Log($"✓ Sample size is adequate ({results.Count} ≥ 30)\n");
                }

                // Check 3: Show score distribution
                Log("--- Score Distribution ---");
                var scoreGroups = new System.Collections.Generic.Dictionary<string, int>
                {
                    {"A (90-100%)", 0},
                    {"B (80-89%)", 0},
                    {"C (70-79%)", 0},
                    {"D (60-69%)", 0},
                    {"F (<60%)", 0}
                };

                foreach (var result in results)
                {
                    if (result.Percentage >= 90) scoreGroups["A (90-100%)"]++;
                    else if (result.Percentage >= 80) scoreGroups["B (80-89%)"]++;
                    else if (result.Percentage >= 70) scoreGroups["C (70-79%)"]++;
                    else if (result.Percentage >= 60) scoreGroups["D (60-69%)"]++;
                    else scoreGroups["F (<60%)"]++;
                }

                foreach (var group in scoreGroups)
                {
                    Log($"  {group.Key}: {group.Value} students");
                }

                Log("\n--- Ready to Analyze ---");
                Log($"✓ Question ID: {questionId}");
                Log($"✓ Instance ID: {instanceId}");
                Log($"✓ Total Students: {results.Count}");
                Log("\n👉 Click '2️⃣ Run Analysis' to proceed!");

            }
            catch (Exception ex)
            {
                Log($"\n❌ ERROR: {ex.Message}");
                Log($"\nStack Trace:\n{ex.StackTrace}");
            }
        }

        private void BtnAnalyze_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();
            Log("════════════════════════════════════════");
            Log("  ITEM ANALYSIS TEST");
            Log("════════════════════════════════════════\n");

            int questionId = (int)numQuestionId.Value;
            int instanceId = (int)numInstanceId.Value;

            Log($"Question ID: {questionId}");
            Log($"Test Instance ID: {instanceId}\n");

            try
            {
                // Step 1: Check data availability
                var results = _resultRepo.GetResultsByInstance(instanceId);
                Log($"✓ Found {results.Count} completed test results\n");

                if (results.Count < 30)
                {
                    Log($"⚠ WARNING: Only {results.Count}/30 students");
                    Log($"   Analysis will proceed but may not be statistically valid.\n");
                }

                // Step 2: Run item analysis
                Log("Running item analysis calculations...\n");
                QuestionStatistics stats = _analyzer.AnalyzeQuestion(questionId, instanceId);

                // Step 3: Display results
                Log("════════════════════════════════════════");
                Log("  ANALYSIS RESULTS");
                Log("════════════════════════════════════════\n");

                Log($"Total Attempts: {stats.TotalAttempts}");
                Log($"Correct Answers: {stats.CorrectCount}");
                Log($"Wrong Answers: {stats.TotalAttempts - stats.CorrectCount}\n");

                Log("─── DIFFICULTY INDEX (P-Value) ───");
                Log($"Formula: P = Correct / Total");
                Log($"P-Value: {stats.DifficultyIndex:F4} ({stats.DifficultyIndex * 100:F2}%)");
                Log($"Category: {stats.GetDifficultyCategory()}");
                Log($"Interpretation: {_analyzer.GetDifficultyInterpretation(stats.DifficultyIndex)}\n");

                if (stats.DiscriminationIndex.HasValue)
                {
                    Log("─── DISCRIMINATION INDEX (D-Value) ───");
                    Log($"Formula: D = P(upper 27%) - P(lower 27%)");
                    Log($"D-Value: {stats.DiscriminationIndex.Value:F4}");
                    Log($"Quality: {stats.GetDiscriminationQuality()}");
                    Log($"Interpretation: {_analyzer.GetDiscriminationInterpretation(stats.DiscriminationIndex.Value)}\n");
                }

                Log("─── RECOMMENDATION ───");
                if (stats.NeedsReview())
                {
                    Log("⚠ This question NEEDS REVIEW");
                    Log("  Consider revising or replacing this question.");
                }
                else
                {
                    Log("✓ This question is acceptable.");
                    Log("  Quality is within normal range.");
                }

                Log("\n════════════════════════════════════════");
                Log("✓ Analysis completed successfully!");
                Log($"  Calculated at: {stats.LastCalculated:yyyy-MM-dd HH:mm:ss}");

            }
            catch (Exception ex)
            {
                Log("\n❌ ERROR OCCURRED:\n");
                Log($"Message: {ex.Message}\n");
                Log($"Stack Trace:\n{ex.StackTrace}");
            }
        }

        // Helper method for logging
        private void Log(string message)
        {
            txtOutput.AppendText(message + "\r\n");
        }
    }
}
