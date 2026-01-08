using System;
using System.Drawing;
using System.Windows.Forms;
using StudentAssessmentSystem.Utilities;
using StudentAssessmentSystem.Models.Users;

namespace StudentAssessmentSystem.UI.Forms.Admin
{
    public partial class AdminDashboardForm : Form
    {
        private Models.Users.Admin _currentAdmin;

        private Label lblWelcome;
        private Label lblInfo;
        private GroupBox grpActions;
        private Button btnManageUsers;
        private Button btnManageSubjects;
        private Button btnViewReports;
        private Button btnLogout;

        public AdminDashboardForm()
        {
            try
            {
                InitializeComponent();
                LoadAdminData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing admin dashboard:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAdminData()
        {
            try
            {
                _currentAdmin = SessionManager.CurrentUser as Models.Users.Admin;

                if (_currentAdmin != null)
                {
                    lblWelcome.Text = $"Welcome, {_currentAdmin.FullName}!";
                    lblInfo.Text = $"Access Level: {_currentAdmin.AccessLevel}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading admin data:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Admin Dashboard";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Welcome Label
            lblWelcome = new Label();
            lblWelcome.Text = "Welcome, System Administrator!";
            lblWelcome.Font = new Font("Arial", 16, FontStyle.Bold);
            lblWelcome.Location = new Point(20, 20);
            lblWelcome.Size = new Size(450, 30);
            this.Controls.Add(lblWelcome);

            // Info
            lblInfo = new Label();
            lblInfo.Text = "Access Level: 3";
            lblInfo.Location = new Point(20, 55);
            lblInfo.Size = new Size(450, 20);
            lblInfo.Font = new Font("Arial", 10);
            this.Controls.Add(lblInfo);

            // Actions Group
            grpActions = new GroupBox();
            grpActions.Text = "Administration";
            grpActions.Location = new Point(20, 90);
            grpActions.Size = new Size(450, 230);
            grpActions.Font = new Font("Arial", 10, FontStyle.Bold);
            this.Controls.Add(grpActions);

            // Manage Users Button - FIXED
            btnManageUsers = new Button();
            btnManageUsers.Text = "Manage Users";
            btnManageUsers.Location = new Point(40, 40);
            btnManageUsers.Size = new Size(160, 50);
            btnManageUsers.Font = new Font("Arial", 10, FontStyle.Bold);
            btnManageUsers.BackColor = Color.LightBlue;
            btnManageUsers.Cursor = Cursors.Hand;
            btnManageUsers.Click += BtnManageUsers_Click; 
            grpActions.Controls.Add(btnManageUsers);

            // Manage Subjects Button
            btnManageSubjects = new Button();
            btnManageSubjects.Text = "Manage Subjects";
            btnManageSubjects.Location = new Point(240, 40);
            btnManageSubjects.Size = new Size(160, 50);
            btnManageSubjects.Font = new Font("Arial", 10, FontStyle.Bold);
            btnManageSubjects.BackColor = Color.LightGreen;
            btnManageSubjects.Cursor = Cursors.Hand;
            btnManageSubjects.Click += BtnManageSubjects_Click;
            grpActions.Controls.Add(btnManageSubjects);

            // View Reports Button
            btnViewReports = new Button();
            btnViewReports.Text = "View Reports";
            btnViewReports.Location = new Point(40, 110);
            btnViewReports.Size = new Size(360, 50);
            btnViewReports.Font = new Font("Arial", 10, FontStyle.Bold);
            btnViewReports.BackColor = Color.LightYellow;
            btnViewReports.Cursor = Cursors.Hand;
            btnViewReports.Click += (s, e) => MessageBox.Show("View Reports coming soon!", "Info");
            grpActions.Controls.Add(btnViewReports);

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

        //  Opens Manage Users Form
        private void BtnManageUsers_Click(object sender, EventArgs e)
        {
            try
            {
                ManageUsersForm form = new ManageUsersForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening user management: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                MessageBox.Show($"Error during logout:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtnManageSubjects_Click(object sender, EventArgs e)
        {
            try
            {
                ManageSubjectsForm form = new ManageSubjectsForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening subject management: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}