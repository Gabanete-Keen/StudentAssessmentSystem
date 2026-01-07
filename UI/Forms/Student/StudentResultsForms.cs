using MySql.Data.MySqlClient;
using StudentAssessmentSystem.DataAccess;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Utilities;
using StudentAssessmentSystem.Models.Results;  // ADDED for TestResult
using StudentAssessmentSystem.Models.Assessment;
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
            try
            {
                _resultRepository = new TestResultRepository();
                _currentStudentId = SessionManager.GetCurrentUserId();  // Ensure SessionManager exists
                if (_currentStudentId == 0)
                {
                    MessageBox.Show("No student session found. Please login again.", "Session Error");
                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization error: {ex.Message}", "Error");
                this.Close();
                return;
            }
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
                    MessageBox.Show("You haven't taken any tests yet.", "No Results Available",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                                        InstanceId = result.InstanceId,
                                        TestTitle = reader.GetString("TestTitle"),
                                        Subject = reader.GetString("SubjectName"),
                                        DateTaken = result.StartTime.ToString("MMM dd, yyyy"),
                                        Score = $"{result.RawScore}/{result.TotalPoints}",
                                        Percentage = $"{result.Percentage:F1}%",
                                        Grade = result.LetterGrade,
                                        Status = result.Passed ? "PASSED" : "FAILED"
                                    });
                                }
                            }
                        }
                    }
                }

                dgvResults.DataSource = displayData;
                dgvResults.Refresh();

                // Hide ID columns
                if (dgvResults.Columns["ResultId"] != null) dgvResults.Columns["ResultId"].Visible = false;
                if (dgvResults.Columns["InstanceId"] != null) dgvResults.Columns["InstanceId"].Visible = false;

                // Resize headers
                dgvResults.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading results: {ex.Message}", "Error",
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
                DataGridViewRow row = dgvResults.SelectedRows[0];

                int resultId = (int)row.Cells["ResultId"].Value;
                int instanceId = (int)row.Cells["InstanceId"].Value;   // ✅ get instanceId directly
                string testTitle = row.Cells["TestTitle"].Value.ToString();

                // (Optional sanity check)
                // MessageBox.Show($"Debug:\nResultId={resultId}\nInstanceId={instanceId}");

                StudentPerformanceDetailForm detailForm = new StudentPerformanceDetailForm(
                    _currentStudentId,
                    instanceId,
                    testTitle
                );
                detailForm.ShowDialog();
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
