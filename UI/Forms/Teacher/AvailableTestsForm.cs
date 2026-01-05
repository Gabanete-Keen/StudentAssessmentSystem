using MySql.Data.MySqlClient;
using StudentAssessmentSystem.DataAccess;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.UI.Forms.Student;
using StudentAssessmentSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    public partial class AvailableTestsForm : Form
    {
        private ListBox lstAvailableTests;
        private Button btnTakeTest;
        private Button btnClose;

        public AvailableTestsForm()
        {
            InitializeComponent();
            LoadAvailableTests();
        }

        private void InitializeComponent()
        {
            this.Text = "Available Tests";
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            Label lblTitle = new Label();
            lblTitle.Text = "Available Tests";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(400, 30);
            this.Controls.Add(lblTitle);

            lstAvailableTests = new ListBox();
            lstAvailableTests.Location = new Point(20, 60);
            lstAvailableTests.Size = new Size(540, 280);
            lstAvailableTests.Font = new Font("Arial", 10);
            this.Controls.Add(lstAvailableTests);

            btnTakeTest = new Button();
            btnTakeTest.Text = "Take Selected Test";
            btnTakeTest.Location = new Point(20, 360);
            btnTakeTest.Size = new Size(150, 35);
            btnTakeTest.BackColor = Color.LightGreen;
            btnTakeTest.Font = new Font("Arial", 10, FontStyle.Bold);
            btnTakeTest.Cursor = Cursors.Hand;
            btnTakeTest.Click += BtnTakeTest_Click;
            this.Controls.Add(btnTakeTest);

            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Location = new Point(480, 360);
            btnClose.Size = new Size(80, 35);
            btnClose.BackColor = Color.LightGray;
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

       
        /// Loads available tests for the student
        private void LoadAvailableTests()
        {
            try
            {
                lstAvailableTests.Items.Clear();  // Use your actual ListBox name

                int studentId = SessionManager.GetCurrentUserId();

                // Get tests available to this student
                var testRepository = new TestRepository();
                var allTests = testRepository.GetAll();

                foreach (var test in allTests)
                {
                    // Create an anonymous object with TestId AND display text
                    var displayItem = new
                    {
                        TestId = test.TestId,           // Include this!
                        TestTitle = test.TestTitle,
                        DisplayText = $"{test.TestTitle}"
                    };

                    lstAvailableTests.Items.Add(displayItem);
                }

                // Set the display member
                lstAvailableTests.DisplayMember = "DisplayText";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tests:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void BtnTakeTest_Click(object sender, EventArgs e)
        {
            // Check if a test is selected
            if (lstAvailableTests.SelectedItem == null)
            {
                MessageBox.Show("Please select a test first.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get the selected test - cast to dynamic to access properties
                dynamic selectedTest = lstAvailableTests.SelectedItem;

                // Now you can access TestId
                int testId = (int)selectedTest.TestId;  // ✅ Cast to int

                // Get the first active instance of this test
                int instanceId = GetTestInstanceId(testId);

                if (instanceId == 0)
                {
                    MessageBox.Show("No active test instance found.\n\n" +
                        "Please contact your teacher.", "Not Available",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Open the TakeTestForm
                TakeTestForm takeTestForm = new TakeTestForm(testId, instanceId);
                takeTestForm.ShowDialog();

                // Refresh the list after test is completed
                LoadAvailableTests();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting test:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gets the active test instance ID for a test
        /// </summary>
        private int GetTestInstanceId(int testId)
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

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TestId", testId);
                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting test instance:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
    }
}

