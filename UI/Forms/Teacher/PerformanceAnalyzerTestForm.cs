using StudentAssessmentSystem.BusinessLogic.Analysis;
using StudentAssessmentSystem.DataAccess.Repositories;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    /// Test tool for PerformanceAnalyzer
    /// Helps verify that student performance analysis works correctly
    public partial class PerformanceAnalyzerTestForm : Form
    {
        private PerformanceAnalyzer _analyzer;
        private TestResultRepository _resultRepo;
        private TestRepository _testRepo;

        // UI Controls
        private NumericUpDown numStudentId;
        private NumericUpDown numInstanceId;
        private Button btnAnalyze;
        private Button btnCheckData;
        private TextBox txtOutput;
        private Button btnClose;

        public PerformanceAnalyzerTestForm()
        {
            _analyzer = new PerformanceAnalyzer();
            _resultRepo = new TestResultRepository();
            _testRepo = new TestRepository();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Performance Analyzer Test Tool - DEBUG MODE";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Title
            Label lblTitle = new Label
            {
                Text = "Test Student Performance Analysis",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(850, 30),
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(lblTitle);

            // Info label
            Label lblInfo = new Label
            {
                Text = "This tool verifies that PerformanceAnalyzer correctly identifies strengths and weaknesses.",
                Location = new Point(20, 55),
                Size = new Size(850, 20),
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblInfo);

            // Student ID Input
            Label lblStudent = new Label
            {
                Text = "Student ID:",
                Location = new Point(20, 90),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblStudent);

            numStudentId = new NumericUpDown
            {
                Location = new Point(130, 87),
                Size = new Size(100, 25),
                Minimum = 1,
                Maximum = 10000,
                Value = 1
            };
            this.Controls.Add(numStudentId);

            // Test Instance ID Input
            Label lblInstance = new Label
            {
                Text = "Test Instance ID:",
                Location = new Point(250, 90),
                Size = new Size(120, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblInstance);

            numInstanceId = new NumericUpDown
            {
                Location = new Point(380, 87),
                Size = new Size(100, 25),
                Minimum = 1,
                Maximum = 10000,
                Value = 1
            };
            this.Controls.Add(numInstanceId);

            // Check Data Button
            btnCheckData = new Button
            {
                Text = "1. Check Data",
                Location = new Point(500, 85),
                Size = new Size(120, 30),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCheckData.Click += BtnCheckDataClick;
            this.Controls.Add(btnCheckData);

            // Analyze Button
            btnAnalyze = new Button
            {
                Text = "2. Run Analysis",
                Location = new Point(630, 85),
                Size = new Size(130, 30),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAnalyze.Click += BtnAnalyzeClick;
            this.Controls.Add(btnAnalyze);

            // Output Label
            Label lblOutput = new Label
            {
                Text = "Output Console:",
                Location = new Point(20, 130),
                Size = new Size(200, 20),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblOutput);

            // Output TextBox
            txtOutput = new TextBox
            {
                Location = new Point(20, 155),
                Size = new Size(850, 450),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.Lime
            };
            this.Controls.Add(txtOutput);

            // Close Button
            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(770, 615),
                Size = new Size(100, 35),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        /// Step 1: Check if data exists in database
        private void BtnCheckDataClick(object sender, EventArgs e)
        {
            txtOutput.Clear();
            Log("═══════════════════════════════════════════════");
            Log("   DATA VALIDATION CHECK");
            Log("═══════════════════════════════════════════════");
            Log("");

            int studentId = (int)numStudentId.Value;
            int instanceId = (int)numInstanceId.Value;

            try
            {
                // Check 1: Test Instance exists
                Log($"[CHECK 1] Test Instance ID: {instanceId}");
                var test = _testRepo.GetTestByInstanceId(instanceId);
                if (test == null)
                {
                    Log("  ✗ ERROR: Test instance not found!");
                    Log("  SOLUTION: Use a valid InstanceId from TestInstances table");
                    return;
                }
                Log($"  ✓ Found test: {test.TestTitle}");
                Log($"  ✓ Questions in test: {test.Questions?.Count ?? 0}");
                Log("");

                // Check 2: Student has completed this test
                Log($"[CHECK 2] Student ID: {studentId}");
                var result = _resultRepo.GetResultByStudentAndInstance(studentId, instanceId);
                if (result == null)
                {
                    Log("  ✗ ERROR: Student has not taken this test!");
                    Log("  SOLUTION: Student must complete the test first");

                    // Show available students who took this test
                    var allResults = _resultRepo.GetResultsByInstance(instanceId);
                    if (allResults.Count > 0)
                    {
                        Log("");
                        Log("  Students who completed this test:");
                        foreach (var r in allResults)
                        {
                            Log($"    - Student ID: {r.StudentId} (Score: {r.Percentage:F1}%)");
                        }
                    }
                    return;
                }
                Log($"  ✓ Found test result: ResultId {result.ResultId}");
                Log($"  ✓ Score: {result.RawScore}/{result.TotalPoints} ({result.Percentage:F1}%)");
                Log($"  ✓ Status: {(result.Passed ? "PASSED" : "FAILED")}");
                Log("");

                // Check 3: Questions have CognitiveLevel and Topic
                Log("[CHECK 3] Question Data Quality");
                int withCogLevel = test.Questions.Count(q => q.CognitiveLevel != 0);
                int withTopic = test.Questions.Count(q => !string.IsNullOrEmpty(q.Topic));

                Log($"  Questions with CognitiveLevel: {withCogLevel}/{test.Questions.Count}");
                Log($"  Questions with Topic: {withTopic}/{test.Questions.Count}");

                if (withCogLevel == 0)
                {
                    Log("  ⚠ WARNING: No questions have CognitiveLevel set!");
                    Log("  Analysis by cognitive level will be empty.");
                }

                if (withTopic == 0)
                {
                    Log("  ⚠ WARNING: No questions have Topic set!");
                    Log("  Analysis by topic will be empty.");
                }
                Log("");

                // Check 4: Show cognitive level distribution
                if (withCogLevel > 0)
                {
                    Log("[CHECK 4] Cognitive Level Distribution:");
                    var cogLevels = test.Questions
                        .GroupBy(q => q.CognitiveLevel.ToString())
                        .OrderBy(g => g.Key);

                    foreach (var group in cogLevels)
                    {
                        Log($"  {group.Key}: {group.Count()} questions");
                    }
                    Log("");
                }

                // Check 5: Show topic distribution
                if (withTopic > 0)
                {
                    Log("[CHECK 5] Topic Distribution:");
                    var topics = test.Questions
                        .Where(q => !string.IsNullOrEmpty(q.Topic))
                        .GroupBy(q => q.Topic)
                        .OrderBy(g => g.Key);

                    foreach (var group in topics)
                    {
                        Log($"  {group.Key}: {group.Count()} questions");
                    }
                    Log("");
                }

                Log("═══════════════════════════════════════════════");
                Log("✓ ALL CHECKS PASSED!");
                Log("Ready to run performance analysis.");
                Log("Click '2. Run Analysis' to proceed.");
                Log("═══════════════════════════════════════════════");
            }
            catch (Exception ex)
            {
                Log("");
                Log("✗ ERROR OCCURRED:");
                Log($"  {ex.Message}");
                Log("");
                Log("STACK TRACE:");
                Log(ex.StackTrace);
            }
        }

        /// Step 2: Run the performance analysis
        private void BtnAnalyzeClick(object sender, EventArgs e)
        {
            txtOutput.Clear();
            Log("═══════════════════════════════════════════════");
            Log("   PERFORMANCE ANALYSIS");
            Log("═══════════════════════════════════════════════");
            Log("");

            int studentId = (int)numStudentId.Value;
            int instanceId = (int)numInstanceId.Value;

            try
            {
                Log($"Analyzing performance for Student ID {studentId} on Test Instance {instanceId}...");
                Log("");

                // Run the analyzer
                var report = _analyzer.AnalyzeStudentPerformance(studentId, instanceId);

                // Display the full report
                string summary = _analyzer.GenerateTextSummary(report);
                Log(summary);

                Log("═══════════════════════════════════════════════");
                Log("✓ ANALYSIS COMPLETED SUCCESSFULLY!");
                Log("═══════════════════════════════════════════════");
            }
            catch (Exception ex)
            {
                Log("");
                Log("✗ ERROR OCCURRED:");
                Log($"  {ex.Message}");
                Log("");
                Log("STACK TRACE:");
                Log(ex.StackTrace);
            }
        }

        /// Helper method to log output
        private void Log(string message)
        {
            txtOutput.AppendText(message + Environment.NewLine);
        }
    }
}
