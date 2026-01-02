using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.BusinessLogic.Services;
using StudentAssessmentSystem.Models.Users;
using StudentAssessmentSystem.Models.Enums;

namespace StudentAssessmentSystem.UI.Forms
{
    public partial class LoginForm : Form
    {
        // Managers for authentication
        private readonly UserManager _userManager;
        private readonly AuthenticationService _authService;

        // UI Controls
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnExit;

        public LoginForm()
        {
            // Initialize managers
            _userManager = new UserManager();
            _authService = new AuthenticationService();

            // Setup the form
            InitializeComponent();
        }

       
        /// Sets up all the UI controls - Simple design!
        private void InitializeComponent()
        {
            // Form properties
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
            txtPassword.KeyPress += TxtPassword_KeyPress; // Allow Enter key to login
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

        /// Handles the Login button click
        /// This is where authentication happens
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Validation: Check if fields are empty
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
                // Disable button to prevent double-click
                btnLogin.Enabled = false;
                btnLogin.Text = "Logging in...";
                this.Cursor = Cursors.WaitCursor;

                // AUTHENTICATION: Try to login
                User authenticatedUser = _authService.Authenticate(
                    txtUsername.Text.Trim(),
                    txtPassword.Text
                );

                if (authenticatedUser != null)
                {
                    // SUCCESS: User found and password correct

                    // Store user in session
                    SessionManager.CurrentUser = authenticatedUser;

                    // Update last login time in database
                    _userManager.UpdateLastLogin(authenticatedUser.UserId);

                    // Show welcome message
                    MessageBox.Show($"Welcome, {authenticatedUser.FullName}!",
                        "Login Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Open appropriate dashboard based on user role
                    this.Hide(); // Hide login form
                    OpenDashboardForUser(authenticatedUser);

                    // Close login form after dashboard is shown
                    this.Close();
                }
                else
                {
                    // FAILURE: Wrong username or password
                    MessageBox.Show(
                        "Invalid username or password.\nPlease try again.",
                        "Login Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    // Clear password field for security
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                // ERROR: Something went wrong
                MessageBox.Show(
                    $"An error occurred during login:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                // Re-enable button and restore cursor
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
                this.Cursor = Cursors.Default;
            }
        }

        /// Opens the correct dashboard based on user role
        /// Admin → AdminDashboardForm
        /// Teacher → TeacherDashboardForm
        /// Student → StudentDashboardForm
        private void OpenDashboardForUser(User user)
        {
            Form dashboardForm = null;

            // POLYMORPHISM: Different forms for different user types
            switch (user.Role)
            {
                case UserRole.Admin:
                    dashboardForm = new AdminDashboardForm();
                    break;

                case UserRole.Teacher:
                    dashboardForm = new TeacherDashboardForm();
                    break;

                case UserRole.Student:
                    dashboardForm = new StudentDashboardForm();
                    break;

                default:
                    MessageBox.Show("Unknown user role!", "Error");
                    return;
            }

            // When dashboard closes, close the entire application
            dashboardForm.FormClosed += (s, args) => Application.Exit();
            dashboardForm.Show();
        }

      
        /// Allows pressing Enter key to login
        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnLogin_Click(sender, e);
                e.Handled = true; // Prevent beep sound
            }
        }
    }
}

