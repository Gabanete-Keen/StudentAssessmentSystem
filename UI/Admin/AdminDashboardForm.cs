using StudentAssessmentSystem.UI.Forms;
using StudentAssessmentSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Admin
{
    using StudentAssessmentSystem.Models.Users;
    public partial class AdminDashboardForm : Form
    {
        private Admin _currentAdmin;

        private Label lblWelcome;
        private GroupBox grpActions;
        private Button btnManageUsers;
        private Button btnManageSubjects;
        private Button btnViewReports;
        private Button btnLogout;

        public AdminDashboardForm()
        {
            _currentAdmin = SessionManager.CurrentUser as Admin;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Admin Dashboard";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // Welcome Label
            lblWelcome = new Label();
            lblWelcome.Text = $"Welcome, Administrator!";
            lblWelcome.Font = new Font("Arial", 16, FontStyle.Bold);
            lblWelcome.Location = new Point(20, 20);
            lblWelcome.Size = new Size(450, 30);
            this.Controls.Add(lblWelcome);

            // Info
            Label lblInfo = new Label();
            lblInfo.Text = $"Access Level: {_currentAdmin.AccessLevel}";
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

            // Manage Users Button
            btnManageUsers = new Button();
            btnManageUsers.Text = "Manage Users";
            btnManageUsers.Location = new Point(40, 40);
            btnManageUsers.Size = new Size(160, 50);
            btnManageUsers.Font = new Font("Arial", 10, FontStyle.Bold);
            btnManageUsers.BackColor = Color.LightBlue;
            btnManageUsers.Cursor = Cursors.Hand;
            btnManageUsers.Click += (s, e) => MessageBox.Show("Manage Users coming soon!");
            grpActions.Controls.Add(btnManageUsers);

            // Manage Subjects Button
            btnManageSubjects = new Button();
            btnManageSubjects.Text = "Manage Subjects";
            btnManageSubjects.Location = new Point(240, 40);
            btnManageSubjects.Size = new Size(160, 50);
            btnManageSubjects.Font = new Font("Arial", 10, FontStyle.Bold);
            btnManageSubjects.BackColor = Color.LightGreen;
            btnManageSubjects.Cursor = Cursors.Hand;
            btnManageSubjects.Click += (s, e) => MessageBox.Show("Manage Subjects coming soon!");
            grpActions.Controls.Add(btnManageSubjects);

            // View Reports Button
            btnViewReports = new Button();
            btnViewReports.Text = "View Reports";
            btnViewReports.Location = new Point(40, 110);
            btnViewReports.Size = new Size(360, 50);
            btnViewReports.Font = new Font("Arial", 10, FontStyle.Bold);
            btnViewReports.BackColor = Color.LightYellow;
            btnViewReports.Cursor = Cursors.Hand;
            btnViewReports.Click += (s, e) => MessageBox.Show("View Reports coming soon!");
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

        private void BtnLogout_Click(object sender, EventArgs e)
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
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }
    }
}

