using System;
using System.Drawing;
using System.Windows.Forms;
using StudentAssessmentSystem.BusinessLogic.Analysis;
using StudentAssessmentSystem.Models.Results;

namespace StudentAssessmentSystem.UI.Forms.Student
{
    /// <summary>
    /// Shows detailed performance analysis for a student's test result
    /// Displays strengths, weaknesses, and personalized recommendations
    /// </summary>
    public partial class StudentPerformanceDetailForm : Form
    {
        private PerformanceAnalyzer _analyzer;
        private StudentPerformanceReport _report;
        private int _studentId;
        private int _testInstanceId;
        private string _testTitle;

        // UI Controls
        private Label lblTitle;
        private Label lblTestInfo;
        private Panel pnlOverall;
        private Panel pnlCognitive;
        private Panel pnlTopics;
        private Panel pnlStrengths;
        private Panel pnlWeaknesses;
        private Panel pnlRecommendations;
        private Button btnClose;
        private Button btnPrint;

        public StudentPerformanceDetailForm(int studentId, int testInstanceId, string testTitle)
        {
            _studentId = studentId;
            _testInstanceId = testInstanceId;
            _testTitle = testTitle;
            _analyzer = new PerformanceAnalyzer();

            InitializeComponent();
            LoadPerformanceData();
        }

        private void InitializeComponent()
        {
            this.Text = "Performance Analysis";
            this.Size = new Size(900, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Scrollable panel for content
            Panel scrollPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(884, 680),
                AutoScroll = true,
                BackColor = Color.WhiteSmoke
            };
            this.Controls.Add(scrollPanel);

            int yPos = 20;

            // Title
            lblTitle = new Label
            {
                Text = "📊 Performance Analysis Report",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, yPos),
                Size = new Size(840, 35),
                ForeColor = Color.DarkBlue
            };
            scrollPanel.Controls.Add(lblTitle);
            yPos += 45;

            // Test Info
            lblTestInfo = new Label
            {
                Text = $"Test: {_testTitle}",
                Font = new Font("Arial", 11, FontStyle.Italic),
                Location = new Point(20, yPos),
                Size = new Size(840, 25),
                ForeColor = Color.Gray
            };
            scrollPanel.Controls.Add(lblTestInfo);
            yPos += 35;

            // Overall Performance Panel
            pnlOverall = CreatePanel("Overall Performance", yPos, Color.LightCyan);
            scrollPanel.Controls.Add(pnlOverall);
            yPos += pnlOverall.Height + 15;

            // Cognitive Level Performance Panel
            pnlCognitive = CreatePanel("Performance by Cognitive Level (Bloom's Taxonomy)", yPos, Color.LightYellow);
            scrollPanel.Controls.Add(pnlCognitive);
            yPos += pnlCognitive.Height + 15;

            // Topics Performance Panel
            pnlTopics = CreatePanel("Performance by Topic", yPos, Color.LightGreen);
            scrollPanel.Controls.Add(pnlTopics);
            yPos += pnlTopics.Height + 15;

            // Strengths Panel
            pnlStrengths = CreatePanel("✓ Your Strengths", yPos, Color.FromArgb(144, 238, 144));
            scrollPanel.Controls.Add(pnlStrengths);
            yPos += pnlStrengths.Height + 15;

            // Weaknesses Panel
            pnlWeaknesses = CreatePanel("✗ Areas for Improvement", yPos, Color.FromArgb(255, 182, 193));
            scrollPanel.Controls.Add(pnlWeaknesses);
            yPos += pnlWeaknesses.Height + 15;

            // Recommendations Panel
            pnlRecommendations = CreatePanel("💡 Study Recommendations", yPos, Color.FromArgb(173, 216, 230));
            scrollPanel.Controls.Add(pnlRecommendations);

            // Buttons
            btnPrint = new Button
            {
                Text = "📄 Print Report",
                Location = new Point(20, 690),
                Size = new Size(150, 40),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightBlue,
                Cursor = Cursors.Hand
            };
            btnPrint.Click += BtnPrint_Click;
            this.Controls.Add(btnPrint);

            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(720, 690),
                Size = new Size(150, 40),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightGray,
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private Panel CreatePanel(string title, int yPosition, Color backColor)
        {
            Panel panel = new Panel
            {
                Location = new Point(20, yPosition),
                Size = new Size(840, 100), // Initial height, will be adjusted
                BackColor = backColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblPanelTitle = new Label
            {
                Text = title,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(820, 25),
                ForeColor = Color.DarkSlateGray
            };
            panel.Controls.Add(lblPanelTitle);

            return panel;
        }

        private void LoadPerformanceData()
        {
            try
            {
                // Show loading message
                lblTitle.Text = "📊 Loading Performance Data...";
                Application.DoEvents();

                // Run analysis
                _report = _analyzer.AnalyzeStudentPerformance(_studentId, _testInstanceId);

                // Update title
                lblTitle.Text = "📊 Performance Analysis Report";

                // Populate panels
                PopulateOverallPanel();
                PopulateCognitivePanel();
                PopulateTopicsPanel();
                PopulateStrengthsPanel();
                PopulateWeaknessesPanel();
                PopulateRecommendationsPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading performance data:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
            }
        }

        private void PopulateOverallPanel()
        {
            int yPos = 45;

            Label lblScore = new Label
            {
                Text = $"{_report.OverallAccuracy:P0}",
                Font = new Font("Arial", 36, FontStyle.Bold),
                Location = new Point(350, yPos),
                Size = new Size(150, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = GetColorForAccuracy(_report.OverallAccuracy)
            };
            pnlOverall.Controls.Add(lblScore);

            yPos += 65;

            // SIMPLE TEXT - NO METHOD NEEDED
            string levelText = _report.OverallAccuracy >= 0.70m ? "Good" :
                               _report.OverallAccuracy >= 0.60m ? "Average" : "Needs Work";
            Label lblLevel = new Label
            {
                Text = levelText,
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(300, yPos),
                Size = new Size(250, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = GetColorForAccuracy(_report.OverallAccuracy)
            };
            pnlOverall.Controls.Add(lblLevel);

            yPos += 40;

            // CRITICAL: ACTUAL COUNTS FROM REPORT
            Label lblDetails = new Label
            {
                Text = $"Correct: {_report.CorrectAnswers}/{_report.TotalQuestions} | Wrong: {_report.WrongAnswers}",
                Font = new Font("Arial", 11),
                Location = new Point(250, yPos),
                Size = new Size(350, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlOverall.Controls.Add(lblDetails);

            pnlOverall.Height = yPos + 40;
        }



        private void PopulateCognitivePanel()
        {
            int yPos = 45;

            if (_report.PerformanceByCognitiveLevel.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "No cognitive level data available",
                    Font = new Font("Arial", 10, FontStyle.Italic),
                    Location = new Point(20, yPos),
                    Size = new Size(800, 25),
                    ForeColor = Color.Gray
                };
                pnlCognitive.Controls.Add(lblEmpty);
                pnlCognitive.Height = yPos + 40;
                return;
            }

            foreach (var kvp in _report.PerformanceByCognitiveLevel)
            {
                AddPerformanceBar(pnlCognitive, kvp.Key, kvp.Value, ref yPos);
            }

            pnlCognitive.Height = yPos + 15;
        }

        private void PopulateTopicsPanel()
        {
            int yPos = 45;

            if (_report.PerformanceByTopic.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "No topic data available",
                    Font = new Font("Arial", 10, FontStyle.Italic),
                    Location = new Point(20, yPos),
                    Size = new Size(800, 25),
                    ForeColor = Color.Gray
                };
                pnlTopics.Controls.Add(lblEmpty);
                pnlTopics.Height = yPos + 40;
                return;
            }

            foreach (var kvp in _report.PerformanceByTopic)
            {
                AddPerformanceBar(pnlTopics, kvp.Key, kvp.Value, ref yPos);
            }

            pnlTopics.Height = yPos + 15;
        }

        private void AddPerformanceBar(Panel panel, string label, decimal accuracy, ref int yPos)
        {
            // Label
            Label lblName = new Label
            {
                Text = label,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(20, yPos),
                Size = new Size(200, 20)
            };
            panel.Controls.Add(lblName);

            // Percentage
            Label lblPercent = new Label
            {
                Text = $"{accuracy:P0}",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(770, yPos),
                Size = new Size(50, 20),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = GetColorForAccuracy(accuracy)
            };
            panel.Controls.Add(lblPercent);

            // Progress bar background
            Panel barBg = new Panel
            {
                Location = new Point(230, yPos + 2),
                Size = new Size(530, 16),
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(barBg);

            // Progress bar fill
            int fillWidth = (int)(530 * accuracy);
            Panel barFill = new Panel
            {
                Location = new Point(1, 1),
                Size = new Size(fillWidth, 14),
                BackColor = GetColorForAccuracy(accuracy)
            };
            barBg.Controls.Add(barFill);

            yPos += 30;
        }

        private void PopulateStrengthsPanel()
        {
            int yPos = 45;

            if (_report.Strengths.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "Keep working hard to identify your strengths!",
                    Font = new Font("Arial", 10, FontStyle.Italic),
                    Location = new Point(20, yPos),
                    Size = new Size(800, 25),
                    ForeColor = Color.Gray
                };
                pnlStrengths.Controls.Add(lblEmpty);
                pnlStrengths.Height = yPos + 40;
                return;
            }

            foreach (string strength in _report.Strengths)
            {
                Label lbl = new Label
                {
                    Text = "✓ " + strength,
                    Font = new Font("Arial", 10),
                    Location = new Point(20, yPos),
                    Size = new Size(800, 25),
                    ForeColor = Color.DarkGreen
                };
                pnlStrengths.Controls.Add(lbl);
                yPos += 30;
            }

            pnlStrengths.Height = yPos + 15;
        }

        private void PopulateWeaknessesPanel()
        {
            int yPos = 45;

            if (_report.Weaknesses.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "Great! No significant weaknesses identified.",
                    Font = new Font("Arial", 10, FontStyle.Italic),
                    Location = new Point(20, yPos),
                    Size = new Size(800, 25),
                    ForeColor = Color.Green
                };
                pnlWeaknesses.Controls.Add(lblEmpty);
                pnlWeaknesses.Height = yPos + 40;
                return;
            }

            foreach (string weakness in _report.Weaknesses)
            {
                Label lbl = new Label
                {
                    Text = "✗ " + weakness,
                    Font = new Font("Arial", 10),
                    Location = new Point(20, yPos),
                    Size = new Size(800, 25),
                    ForeColor = Color.DarkRed
                };
                pnlWeaknesses.Controls.Add(lbl);
                yPos += 30;
            }

            pnlWeaknesses.Height = yPos + 15;
        }

        private void PopulateRecommendationsPanel()
        {
            int yPos = 45;

            foreach (string recommendation in _report.Recommendations)
            {
                Label lbl = new Label
                {
                    Text = recommendation,
                    Font = new Font("Arial", 10),
                    Location = new Point(20, yPos),
                    Size = new Size(800, 25),
                    ForeColor = Color.DarkBlue
                };
                pnlRecommendations.Controls.Add(lbl);
                yPos += 30;
            }

            pnlRecommendations.Height = yPos + 15;
        }

        private Color GetColorForAccuracy(decimal accuracy)
        {
            if (accuracy >= 0.90m) return Color.Green;
            if (accuracy >= 0.80m) return Color.LimeGreen;
            if (accuracy >= 0.70m) return Color.Orange;
            if (accuracy >= 0.60m) return Color.OrangeRed;
            return Color.Red;
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            string report = _analyzer.GenerateTextSummary(_report);

            MessageBox.Show(
                "Print functionality coming soon!\n\n" +
                "For now, here's a text version:\n\n" +
                report.Substring(0, Math.Min(300, report.Length)) + "...",
                "Print Preview",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}
