using StudentAssessmentSystem.UI.Forms.Teacher;
using StudentAssessmentSystem.Utilities;
using System;
using System.Drawing;
using System.Windows.Forms;



namespace StudentAssessmentSystem.UI.Forms.Student
{
    public partial class StudentDashboardForm : Form
    {
        private Models.Users.Student _currentStudent;

        private Label lblWelcome;
        private Label lblInfo;
        private GroupBox grpActions;
        private Button btnTakeTest;
        private Button btnMyResults;
        private Button btnLogout;

        public StudentDashboardForm()
        {
            try
            {
                InitializeComponent();
                LoadStudentData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing student dashboard:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStudentData()
        {
            try
            {
                _currentStudent = SessionManager.CurrentUser as Models.Users.Student;

                if (_currentStudent != null)
                {
                    lblWelcome.Text = $"Welcome, {_currentStudent.FullName}!";
                    lblInfo.Text = $"Student Number: {_currentStudent.StudentNumber ?? "N/A"} | Year Level: {_currentStudent.YearLevel}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading student data:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Student Dashboard";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Welcome Label
            lblWelcome = new Label();
            lblWelcome.Text = "Welcome, Student!";
            lblWelcome.Font = new Font("Arial", 16, FontStyle.Bold);
            lblWelcome.Location = new Point(20, 20);
            lblWelcome.Size = new Size(450, 30);
            this.Controls.Add(lblWelcome);

            // Student info
            lblInfo = new Label();
            lblInfo.Text = "Loading student information...";
            lblInfo.Location = new Point(20, 55);
            lblInfo.Size = new Size(450, 20);
            lblInfo.Font = new Font("Arial", 10);
            this.Controls.Add(lblInfo);

            // Actions Group
            grpActions = new GroupBox();
            grpActions.Text = "What would you like to do?";
            grpActions.Location = new Point(20, 90);
            grpActions.Size = new Size(450, 230);
            grpActions.Font = new Font("Arial", 10, FontStyle.Bold);
            this.Controls.Add(grpActions);

            // Take Test Button
            btnTakeTest = new Button();
            btnTakeTest.Text = "Take a Test";
            btnTakeTest.Location = new Point(120, 50);
            btnTakeTest.Size = new Size(200, 60);
            btnTakeTest.Font = new Font("Arial", 12, FontStyle.Bold);
            btnTakeTest.BackColor = Color.LightGreen;
            btnTakeTest.Cursor = Cursors.Hand;
            btnTakeTest.Click += BtnTakeTest_Click;
            grpActions.Controls.Add(btnTakeTest);

            // My Results Button
            btnMyResults = new Button();
            btnMyResults.Text = "View My Results";
            btnMyResults.Location = new Point(120, 130);
            btnMyResults.Size = new Size(200, 60);
            btnMyResults.Font = new Font("Arial", 12, FontStyle.Bold);
            btnMyResults.BackColor = Color.LightBlue;
            btnMyResults.Cursor = Cursors.Hand;
            btnMyResults.Click += BtnMyResults_Click;
            grpActions.Controls.Add(btnMyResults);

            // Logout Button
            btnLogout = new Button();
            btnLogout.Text = "Logout";
            btnLogout.Location = new Point(370, 330);
            btnLogout.Size = new Size(100, 30);
            btnLogout.BackColor = Color.LightGray;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += BtnLogout_Click;
            this.Controls.Add(btnLogout);

        }

        private void BtnTakeTest_Click(object sender, EventArgs e)
        {
            try
            {
                //  Use FULL namespace to avoid ambiguity
                var availableTestsForm = new StudentAssessmentSystem.UI.Forms.Student.AvailableTestsForm(_currentStudent);
                this.Hide();

                DialogResult result = availableTestsForm.ShowDialog();

                this.Show();

                if (result == DialogResult.OK)
                {
                    LoadStudentData();
                }
            }
            catch (Exception ex)
            {
                this.Show();
                MessageBox.Show($"Error: {ex.Message}\n\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnMyResults_Click(object sender, EventArgs e)
        {
            StudentResultsForm resultsForm = new StudentResultsForm();
            resultsForm.ShowDialog();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to logout?",
                    "Confirm Logout",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    SessionManager.Logout();

                    var loginForm = new StudentAssessmentSystem.UI.Forms.LoginForm();
                    loginForm.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during logout:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}