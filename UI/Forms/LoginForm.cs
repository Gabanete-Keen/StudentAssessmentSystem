using System;
using System.Drawing;
using System.Windows.Forms;
using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.BusinessLogic.Services;
using StudentAssessmentSystem.Models.Users;
using StudentAssessmentSystem.Models.Enums;
using StudentAssessmentSystem.Utilities;

namespace StudentAssessmentSystem.UI.Forms
{
    public partial class LoginForm : Form
    {
        private readonly UserManager _userManager;
        private readonly AuthenticationService _authService;

        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnExit;

        public LoginForm()
        {
            _userManager = new UserManager();
            _authService = new AuthenticationService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Student Assessment System - Login";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Title Label
            lblTitle = new Label();
            lblTitle.Text = "Student Assessment System";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(50, 30);
            lblTitle.Size = new Size(300, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblTitle);

            // Username Label
            lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Location = new Point(50, 80);
            lblUsername.Size = new Size(80, 20);
            this.Controls.Add(lblUsername);

            // Username TextBox
            txtUsername = new TextBox();
            txtUsername.Location = new Point(50, 105);
            txtUsername.Size = new Size(300, 25);
            txtUsername.Font = new Font("Arial", 10);
            this.Controls.Add(txtUsername);

            // Password Label
            lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Location = new Point(50, 140);
            lblPassword.Size = new Size(80, 20);
            this.Controls.Add(lblPassword);

            // Password TextBox
            txtPassword = new TextBox();
            txtPassword.Location = new Point(50, 165);
            txtPassword.Size = new Size(300, 25);
            txtPassword.Font = new Font("Arial", 10);
            txtPassword.PasswordChar = '*';
            txtPassword.KeyPress += TxtPassword_KeyPress;
            this.Controls.Add(txtPassword);

            // Login Button
            btnLogin = new Button();
            btnLogin.Text = "Login";
            btnLogin.Location = new Point(50, 210);
            btnLogin.Size = new Size(140, 35);
            btnLogin.Font = new Font("Arial", 10, FontStyle.Bold);
            btnLogin.BackColor = Color.LightGreen;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);

            // Exit Button
            btnExit = new Button();
            btnExit.Text = "Exit";
            btnExit.Location = new Point(210, 210);
            btnExit.Size = new Size(140, 35);
            btnExit.Font = new Font("Arial", 10);
            btnExit.BackColor = Color.LightGray;
            btnExit.Cursor = Cursors.Hand;
            btnExit.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnExit);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter your username.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter your password.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            try
            {
                btnLogin.Enabled = false;
                btnLogin.Text = "Logging in...";
                this.Cursor = Cursors.WaitCursor;

                // Authenticate
                User authenticatedUser = _authService.Authenticate(
                    txtUsername.Text.Trim(),
                    txtPassword.Text
                );

                if (authenticatedUser != null)
                {
                    // SUCCESS
                    SessionManager.CurrentUser = authenticatedUser;

                    // Update last login
                    _userManager.UpdateLastLogin(authenticatedUser.UserId);

                    // Show welcome message
                    MessageBox.Show($"Welcome, {authenticatedUser.FullName}!",
                        "Login Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Open dashboard and handle form closure properly
                    Form dashboardForm = CreateDashboardForUser(authenticatedUser);

                    if (dashboardForm != null)
                    {
                        // Hide login form first
                        this.Hide();

                        // Show dashboard
                        dashboardForm.Show();

                        // Close login form when dashboard closes
                        dashboardForm.FormClosed += (s, args) => {
                            this.Close();
                            Application.Exit();
                        };
                    }
                    else
                    {
                        MessageBox.Show("Could not create dashboard!", "Error");
                        btnLogin.Enabled = true;
                        btnLogin.Text = "Login";
                        this.Cursor = Cursors.Default;
                    }
                }
                else
                {
                    // FAILURE
                    MessageBox.Show(
                        "Invalid username or password.\nPlease try again.",
                        "Login Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    txtPassword.Clear();
                    txtPassword.Focus();

                    btnLogin.Enabled = true;
                    btnLogin.Text = "Login";
                    this.Cursor = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred during login:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
                this.Cursor = Cursors.Default;
            }
        }

      
        private Form CreateDashboardForUser(User user)
        {
            try
            {
                Form dashboardForm = null;

                switch (user.Role)
                {
                    case UserRole.Admin:
                        dashboardForm = new Admin.AdminDashboardForm();
                        break;

                    case UserRole.Teacher:
                        dashboardForm = new Teacher.TeacherDashboardForm();
                        break;

                    case UserRole.Student:
                        dashboardForm = new Student.StudentDashboardForm();
                        break;

                    default:
                        MessageBox.Show("Unknown user role!", "Error");
                        return null;
                }

                return dashboardForm;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error creating dashboard:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Dashboard Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return null;
            }
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnLogin_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}