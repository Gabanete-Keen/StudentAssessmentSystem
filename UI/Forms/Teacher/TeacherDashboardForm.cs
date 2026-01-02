using System;
using System.Drawing;
using System.Windows.Forms;
using StudentAssessmentSystem.Utilities;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    public partial class TeacherDashboardForm : Form
    {
        private Models.Users.Teacher _currentTeacher;

        private Label lblWelcome;
        private Label lblDept;
        private GroupBox grpQuickActions;
        private Button btnCreateTest;
        private Button btnMyTests;
        private Button btnQuestionBank;
        private Button btnViewAnalysis;
        private Button btnLogout;
        private Label lblInfo;

        public TeacherDashboardForm()
        {
            try
            {
                InitializeComponent();
                LoadTeacherData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing teacher dashboard:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTeacherData()
        {
            try
            {
                _currentTeacher = SessionManager.CurrentUser as Models.Users.Teacher;

                if (_currentTeacher != null)
                {
                    lblWelcome.Text = $"Welcome, {_currentTeacher.FullName}!";
                    lblDept.Text = $"Department: {_currentTeacher.Department ?? "Not Assigned"}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teacher data:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Teacher Dashboard";
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Welcome Label
            lblWelcome = new Label();
            lblWelcome.Text = "Welcome, Teacher!";
            lblWelcome.Font = new Font("Arial", 16, FontStyle.Bold);
            lblWelcome.Location = new Point(20, 20);
            lblWelcome.Size = new Size(550, 30);
            this.Controls.Add(lblWelcome);

            // Department info
            lblDept = new Label();
            lblDept.Text = "Department: Loading...";
            lblDept.Location = new Point(20, 55);
            lblDept.Size = new Size(400, 20);
            lblDept.Font = new Font("Arial", 10);
            this.Controls.Add(lblDept);

            // Quick Actions Group
            grpQuickActions = new GroupBox();
            grpQuickActions.Text = "Quick Actions";
            grpQuickActions.Location = new Point(20, 90);
            grpQuickActions.Size = new Size(550, 270);
            grpQuickActions.Font = new Font("Arial", 10, FontStyle.Bold);
            this.Controls.Add(grpQuickActions);

            // Create Test Button
            btnCreateTest = new Button();
            btnCreateTest.Text = "Create New Test";
            btnCreateTest.Location = new Point(30, 40);
            btnCreateTest.Size = new Size(220, 50);
            btnCreateTest.Font = new Font("Arial", 10, FontStyle.Bold);
            btnCreateTest.BackColor = Color.LightGreen;
            btnCreateTest.Cursor = Cursors.Hand;
            btnCreateTest.Click += BtnCreateTest_Click;
            grpQuickActions.Controls.Add(btnCreateTest);

            // My Tests Button
            btnMyTests = new Button();
            btnMyTests.Text = "View My Tests";
            btnMyTests.Location = new Point(280, 40);
            btnMyTests.Size = new Size(220, 50);
            btnMyTests.Font = new Font("Arial", 10, FontStyle.Bold);
            btnMyTests.BackColor = Color.LightBlue;
            btnMyTests.Cursor = Cursors.Hand;
            btnMyTests.Click += BtnMyTests_Click;
            grpQuickActions.Controls.Add(btnMyTests);

            // Question Bank Button
            btnQuestionBank = new Button();
            btnQuestionBank.Text = "Question Bank";
            btnQuestionBank.Location = new Point(30, 110);
            btnQuestionBank.Size = new Size(220, 50);
            btnQuestionBank.Font = new Font("Arial", 10);
            btnQuestionBank.BackColor = Color.LightYellow;
            btnQuestionBank.Cursor = Cursors.Hand;
            btnQuestionBank.Click += BtnQuestionBank_Click;
            grpQuickActions.Controls.Add(btnQuestionBank);

            // View Analysis Button
            btnViewAnalysis = new Button();
            btnViewAnalysis.Text = "Item Analysis";
            btnViewAnalysis.Location = new Point(280, 110);
            btnViewAnalysis.Size = new Size(220, 50);
            btnViewAnalysis.Font = new Font("Arial", 10);
            btnViewAnalysis.BackColor = Color.LightCoral;
            btnViewAnalysis.Cursor = Cursors.Hand;
            btnViewAnalysis.Click += BtnViewAnalysis_Click;
            grpQuickActions.Controls.Add(btnViewAnalysis);

            // Info Label
            lblInfo = new Label();
            lblInfo.Text = "Click on any button above to get started!";
            lblInfo.Location = new Point(30, 180);
            lblInfo.Size = new Size(470, 60);
            lblInfo.Font = new Font("Arial", 9);
            lblInfo.ForeColor = Color.Gray;
            grpQuickActions.Controls.Add(lblInfo);

            // Logout Button
            btnLogout = new Button();
            btnLogout.Text = "Logout";
            btnLogout.Location = new Point(470, 375);
            btnLogout.Size = new Size(100, 35);
            btnLogout.Font = new Font("Arial", 10);
            btnLogout.BackColor = Color.LightGray;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += BtnLogout_Click;
            this.Controls.Add(btnLogout);
        }

        private void BtnCreateTest_Click(object sender, EventArgs e)
        {
            try
            {
                TestCreationForm form = new TestCreationForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening test creation form:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnMyTests_Click(object sender, EventArgs e)
        {
            try
            {
                MyTestsForm form = new MyTestsForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening my tests:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnQuestionBank_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Question Bank feature coming soon!", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnViewAnalysis_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Item Analysis feature coming soon!", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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