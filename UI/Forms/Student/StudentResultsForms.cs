using MySql.Data.MySqlClient;
using StudentAssessmentSystem.DataAccess;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Results;
using StudentAssessmentSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Student
{
    public partial class StudentResultsForm : Form
    {
        private TestResultRepository _resultRepository;
        private int _currentStudentId;
        private Label lblTitle;
        private DataGridView dgvResults;
        private Button btnViewDetails;
        private Button btnClose;
        private Label lblSummary;

        public StudentResultsForm()
        {
            _resultRepository = new TestResultRepository();
            _currentStudentId = SessionManager.GetCurrentUserId();
            InitializeComponent();
            LoadTestResults();
            LoadSummaryStats();
        }

        private void InitializeComponent()
        {
            this.Text = "My Test Results";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int yPos = 20;

            lblTitle = new Label
            {
                Location = new Point(30, yPos),
                Size = new Size(400, 35),
                Text = "📊 My Test Results",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(lblTitle);
            yPos += 45;

            lblSummary = new Label
            {
                Location = new Point(30, yPos),
                Size = new Size(920, 60),
                Font = new Font("Arial", 10),
                BackColor = Color.LightCyan,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15),
                Text = "Loading statistics..."
            };
            this.Controls.Add(lblSummary);
            yPos += 70;

            dgvResults = new DataGridView
            {
                Location = new Point(30, yPos),
                Size = new Size(920, 380),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                Font = new Font("Arial", 10)
            };
            dgvResults.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
            this.Controls.Add(dgvResults);
            yPos += 390;

            btnViewDetails = new Button
            {
                Location = new Point(30, yPos),
                Size = new Size(200, 40),
                Text = "📄 View Details",
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.LightBlue,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnViewDetails.Click += BtnViewDetails_Click;
            this.Controls.Add(btnViewDetails);

            btnClose = new Button
            {
                Location = new Point(750, yPos),
                Size = new Size(200, 40),
                Text = "Close",
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.LightGray,
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            dgvResults.SelectionChanged += (s, e) =>
            {
                btnViewDetails.Enabled = dgvResults.SelectedRows.Count > 0;
            };
        }

        private void LoadTestResults()
        {
            try
            {
                var results = _resultRepository.GetResultsByStudent(_currentStudentId);

                if (results == null || results.Count == 0)
                {
                    MessageBox.Show(
                        "You haven't taken any tests yet.",
                        "No Results Available",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    dgvResults.DataSource = null;
                    btnViewDetails.Enabled = false;
                    return;
                }

                var displayData = new List<dynamic>();

                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();

                    foreach (var result in results)
                    {
                        string query = @"
                            SELECT t.TestTitle, s.SubjectName
                            FROM Tests t
                            INNER JOIN TestInstances ti ON t.TestId = ti.TestId
                            INNER JOIN Subjects s ON t.SubjectId = s.SubjectId
                            WHERE ti.InstanceId = @InstanceId";

                        using (var cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@InstanceId", result.InstanceId);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    displayData.Add(new
                                    {
                                        ResultId = result.ResultId,
                                        TestTitle = reader.GetString("TestTitle"),
                                        Subject = reader.GetString("SubjectName"),
                                        DateTaken = result.StartTime.ToString("MMM dd, yyyy"),
                                        Score = $"{result.RawScore} / {result.TotalPoints}",
                                        Percentage = $"{result.Percentage:F1}%",
                                        Grade = result.LetterGrade,
                                        Status = result.Passed ? "✓ PASSED" : "✗ FAILED"
                                    });
                                }
                                reader.Close();
                            }
                        }
                    }
                }

                // Bind to DataGridView
                dgvResults.DataSource = displayData;

                // ===== FORCE BINDING TO COMPLETE =====
                Application.DoEvents(); // Let UI thread process
                dgvResults.Update();
                dgvResults.Refresh();

                // ===== SAFE COLUMN CUSTOMIZATION (using try-catch for each column) =====
                try
                {
                    // Hide ResultId column (first column)
                    if (dgvResults.Columns.Count > 0)
                        dgvResults.Columns[0].Visible = false;

                    // Set ALL columns to auto-resize first
                    dgvResults.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                    // Then customize specific columns IF they exist
                    for (int i = 0; i < dgvResults.Columns.Count; i++)
                    {
                        string colName = dgvResults.Columns[i].Name;

                        switch (colName)
                        {
                            case "ResultId":
                                dgvResults.Columns[i].Visible = false;
                                break;
                            case "TestTitle":
                                dgvResults.Columns[i].HeaderText = "Test Name";
                                dgvResults.Columns[i].Width = 250;
                                break;
                            case "Subject":
                                dgvResults.Columns[i].HeaderText = "Subject";
                                dgvResults.Columns[i].Width = 150;
                                break;
                            case "DateTaken":
                                dgvResults.Columns[i].HeaderText = "Date Taken";
                                dgvResults.Columns[i].Width = 120;
                                break;
                            case "Score":
                                dgvResults.Columns[i].HeaderText = "Score";
                                dgvResults.Columns[i].Width = 100;
                                break;
                            case "Percentage":
                                dgvResults.Columns[i].HeaderText = "Percentage";
                                dgvResults.Columns[i].Width = 100;
                                break;
                            case "Grade":
                                dgvResults.Columns[i].HeaderText = "Grade";
                                dgvResults.Columns[i].Width = 80;
                                break;
                            case "Status":
                                dgvResults.Columns[i].HeaderText = "Status";
                                dgvResults.Columns[i].Width = 120;
                                break;
                        }
                    }

                    // Color code the Status column
                    dgvResults.CellFormatting += DgvResults_CellFormatting;
                }
                catch (Exception colEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Column error: {colEx.Message}");
                    // Don't crash - just log it
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading results:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSummaryStats()
        {
            try
            {
                var results = _resultRepository.GetResultsByStudent(_currentStudentId);

                if (results == null || results.Count == 0)
                {
                    lblSummary.Text = "📊 No tests taken yet.";
                    return;
                }

                int totalTests = results.Count;
                int passed = results.Count(r => r.Passed);
                int failed = totalTests - passed;
                decimal avgPercentage = results.Average(r => r.Percentage);

                lblSummary.Text = $"Total Tests: {totalTests}  |  " +
                                  $"Passed: {passed}  |  " +
                                  $"Failed: {failed}  |  " +
                                  $"Average Score: {avgPercentage:F1}%";
            }
            catch (Exception ex)
            {
                lblSummary.Text = $"Error loading statistics: {ex.Message}";
            }
        }

        private void BtnViewDetails_Click(object sender, EventArgs e)
        {
            if (dgvResults.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a test result to view details.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int resultId = (int)dgvResults.SelectedRows[0].Cells["ResultId"].Value;

                MessageBox.Show(
                    $"Detailed results view for Result ID: {resultId}\n\n" +
                    "This will show:\n" +
                    "• Question-by-question breakdown\n" +
                    "• Correct/incorrect answers\n" +
                    "• Explanations\n" +
                    "• Time spent per question",
                    "Feature Coming Soon",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error viewing details:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvResults_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                if (dgvResults.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
                {
                    string status = e.Value.ToString();

                    if (status.Contains("PASSED"))
                    {
                        e.CellStyle.ForeColor = Color.Green;
                        e.CellStyle.Font = new Font(dgvResults.Font, FontStyle.Bold);
                    }
                    else if (status.Contains("FAILED"))
                    {
                        e.CellStyle.ForeColor = Color.Red;
                        e.CellStyle.Font = new Font(dgvResults.Font, FontStyle.Bold);
                    }
                }
            }
            catch { }
        }
    }
}
