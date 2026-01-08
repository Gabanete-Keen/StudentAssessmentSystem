using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.BusinessLogic.Services;
using StudentAssessmentSystem.Models.Enums;
using StudentAssessmentSystem.Models.Users;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Admin
{
    public partial class AddEditUserForm : Form
    {
        private UserManager _userManager;
        private User _existingUser;
        private bool _isEditMode;

        // UI Controls
        private Label lblTitle;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private Label lblFirstName;
        private TextBox txtFirstName;
        private Label lblLastName;
        private TextBox txtLastName;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblRole;
        private ComboBox cboRole;
        private Panel pnlRoleSpecific;
        private Button btnSave;
        private Button btnCancel;

        // Role-specific controls
        private Label lblAccessLevel;
        private NumericUpDown numAccessLevel;
        private Label lblEmployeeNumber;
        private TextBox txtEmployeeNumber;
        private Label lblDepartment;
        private TextBox txtDepartment;
        private Label lblStudentNumber;
        private TextBox txtStudentNumber;
        private Label lblYearLevel;
        private NumericUpDown numYearLevel;

        public AddEditUserForm()
        {
            _userManager = new UserManager();
            _isEditMode = false;
            InitializeComponent();
        }

        public AddEditUserForm(User user)
        {
            _userManager = new UserManager();
            _existingUser = user;
            _isEditMode = true;
            InitializeComponent();
            LoadUserData();
        }

        private void InitializeComponent()
        {
            this.Text = _isEditMode ? "Edit User" : "Add New User";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int yPos = 20;

            // Title
            lblTitle = new Label
            {
                Text = _isEditMode ? "✏️ Edit User" : "➕ Add New User",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, yPos),
                Size = new Size(450, 30)
            };
            this.Controls.Add(lblTitle);
            yPos += 40;

            // Username
            lblUsername = new Label
            {
                Text = "Username: *",
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblUsername);

            txtUsername = new TextBox
            {
                Location = new Point(130, yPos - 2),
                Size = new Size(330, 25),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(txtUsername);
            yPos += 35;

            // Password
            lblPassword = new Label
            {
                Text = _isEditMode ? "New Password:" : "Password: *",
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblPassword);

            txtPassword = new TextBox
            {
                Location = new Point(130, yPos - 2),
                Size = new Size(330, 25),
                Font = new Font("Arial", 10),
                UseSystemPasswordChar = true
            };
            if (_isEditMode)
            {
                Label lblPasswordHint = new Label
                {
                    Text = "(Leave blank to keep current password)",
                    Location = new Point(130, yPos + 20),
                    Size = new Size(250, 15),
                    Font = new Font("Arial", 8, FontStyle.Italic),
                    ForeColor = Color.Gray
                };
                this.Controls.Add(lblPasswordHint);
                yPos += 20;
            }
            this.Controls.Add(txtPassword);
            yPos += 35;

            // First Name
            lblFirstName = new Label
            {
                Text = "First Name: *",
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblFirstName);

            txtFirstName = new TextBox
            {
                Location = new Point(130, yPos - 2),
                Size = new Size(150, 25),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(txtFirstName);

            // Last Name
            lblLastName = new Label
            {
                Text = "Last Name: *",
                Location = new Point(290, yPos),
                Size = new Size(80, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblLastName);

            txtLastName = new TextBox
            {
                Location = new Point(375, yPos - 2),
                Size = new Size(85, 25),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(txtLastName);
            yPos += 35;

            // Email
            lblEmail = new Label
            {
                Text = "Email: *",
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblEmail);

            txtEmail = new TextBox
            {
                Location = new Point(130, yPos - 2),
                Size = new Size(330, 25),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(txtEmail);
            yPos += 35;

            // Role
            lblRole = new Label
            {
                Text = "Role: *",
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblRole);

            cboRole = new ComboBox
            {
                Location = new Point(130, yPos - 2),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 10)
            };
            cboRole.Items.AddRange(new object[] { "Admin", "Teacher", "Student" });
            cboRole.SelectedIndex = 0;
            cboRole.SelectedIndexChanged += CboRole_SelectedIndexChanged;
            this.Controls.Add(cboRole);
            yPos += 40;

            // Role-specific panel
            pnlRoleSpecific = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(440, 120),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 248, 255)
            };
            this.Controls.Add(pnlRoleSpecific);

            CreateRoleSpecificControls();
            yPos += 130;

            // Buttons
            btnSave = new Button
            {
                Text = _isEditMode ? "💾 Update" : "💾 Save",
                Location = new Point(240, yPos),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(350, yPos),
                Size = new Size(100, 35),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 10),
                Cursor = Cursors.Hand
            };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);
        }

        private void CreateRoleSpecificControls()
        {
            // Admin Controls
            lblAccessLevel = new Label
            {
                Text = "Access Level:",
                Location = new Point(10, 15),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold),
                Visible = false
            };
            pnlRoleSpecific.Controls.Add(lblAccessLevel);

            numAccessLevel = new NumericUpDown
            {
                Location = new Point(120, 13),
                Size = new Size(60, 25),
                Minimum = 1,
                Maximum = 3,
                Value = 1,
                Font = new Font("Arial", 10),
                Visible = false
            };
            pnlRoleSpecific.Controls.Add(numAccessLevel);

            Label lblAccessNote = new Label
            {
                Text = "1=Basic, 2=Moderate, 3=Full",
                Location = new Point(190, 15),
                Size = new Size(200, 20),
                Font = new Font("Arial", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                Visible = false,
                Name = "lblAccessNote"
            };
            pnlRoleSpecific.Controls.Add(lblAccessNote);

            // Teacher Controls
            lblEmployeeNumber = new Label
            {
                Text = "Employee #:",
                Location = new Point(10, 15),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold),
                Visible = false
            };
            pnlRoleSpecific.Controls.Add(lblEmployeeNumber);

            txtEmployeeNumber = new TextBox
            {
                Location = new Point(120, 13),
                Size = new Size(200, 25),
                Font = new Font("Arial", 10),
                Visible = false
            };
            pnlRoleSpecific.Controls.Add(txtEmployeeNumber);

            lblDepartment = new Label
            {
                Text = "Department:",
                Location = new Point(10, 50),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold),
                Visible = false
            };
            pnlRoleSpecific.Controls.Add(lblDepartment);

            txtDepartment = new TextBox
            {
                Location = new Point(120, 48),
                Size = new Size(200, 25),
                Font = new Font("Arial", 10),
                Visible = false
            };
            pnlRoleSpecific.Controls.Add(txtDepartment);

            // Student Controls
            lblStudentNumber = new Label
            {
                Text = "Student #:",
                Location = new Point(10, 15),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold),
                Visible = false
            };
            pnlRoleSpecific.Controls.Add(lblStudentNumber);

            txtStudentNumber = new TextBox
            {
                Location = new Point(120, 13),
                Size = new Size(200, 25),
                Font = new Font("Arial", 10),
                Visible = false
            };
            pnlRoleSpecific.Controls.Add(txtStudentNumber);

            lblYearLevel = new Label
            {
                Text = "Year Level:",
                Location = new Point(10, 50),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold),
                Visible = false
            };
            pnlRoleSpecific.Controls.Add(lblYearLevel);

            numYearLevel = new NumericUpDown
            {
                Location = new Point(120, 48),
                Size = new Size(60, 25),
                Minimum = 1,
                Maximum = 6,
                Value = 1,
                Font = new Font("Arial", 10),
                Visible = false
            };
            pnlRoleSpecific.Controls.Add(numYearLevel);
        }

        private void CboRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hide all role-specific controls
            lblAccessLevel.Visible = false;
            numAccessLevel.Visible = false;
            pnlRoleSpecific.Controls.Find("lblAccessNote", true)[0].Visible = false;

            lblEmployeeNumber.Visible = false;
            txtEmployeeNumber.Visible = false;
            lblDepartment.Visible = false;
            txtDepartment.Visible = false;

            lblStudentNumber.Visible = false;
            txtStudentNumber.Visible = false;
            lblYearLevel.Visible = false;
            numYearLevel.Visible = false;

            // Show relevant controls based on role
            string role = cboRole.SelectedItem.ToString();

            if (role == "Admin")
            {
                lblAccessLevel.Visible = true;
                numAccessLevel.Visible = true;
                pnlRoleSpecific.Controls.Find("lblAccessNote", true)[0].Visible = true;
            }
            else if (role == "Teacher")
            {
                lblEmployeeNumber.Visible = true;
                txtEmployeeNumber.Visible = true;
                lblDepartment.Visible = true;
                txtDepartment.Visible = true;
            }
            else if (role == "Student")
            {
                lblStudentNumber.Visible = true;
                txtStudentNumber.Visible = true;
                lblYearLevel.Visible = true;
                numYearLevel.Visible = true;
            }
        }

        private void LoadUserData()
        {
            if (_existingUser == null) return;

            txtUsername.Text = _existingUser.Username;
            txtFirstName.Text = _existingUser.FirstName;
            txtLastName.Text = _existingUser.LastName;
            txtEmail.Text = _existingUser.Email;

            // Set role and load role-specific data
            cboRole.SelectedItem = _existingUser.Role.ToString();

            // ✅ FIXED: Use full namespace
            if (_existingUser is StudentAssessmentSystem.Models.Users.Admin admin)
            {
                numAccessLevel.Value = admin.AccessLevel;
            }
            else if (_existingUser is StudentAssessmentSystem.Models.Users.Teacher teacher)
            {
                txtEmployeeNumber.Text = teacher.EmployeeNumber ?? "";
                txtDepartment.Text = teacher.Department ?? "";
            }
            else if (_existingUser is StudentAssessmentSystem.Models.Users.Student student)
            {
                txtStudentNumber.Text = student.StudentNumber ?? "";
                numYearLevel.Value = student.YearLevel;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password is required for new users.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("First name is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Last name is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLastName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Valid email is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            return true;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                User user = null;
                string role = cboRole.SelectedItem.ToString();

                if (_isEditMode)
                {
                    // Update existing user
                    user = _existingUser;
                    user.Username = txtUsername.Text.Trim();
                    user.FirstName = txtFirstName.Text.Trim();
                    user.LastName = txtLastName.Text.Trim();
                    user.Email = txtEmail.Text.Trim();

                    // Update password if provided
                    if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        var authService = new AuthenticationService();
                        user.PasswordHash = authService.HashPassword(txtPassword.Text);
                    }

                    // ✅ FIXED: Update role-specific data with full namespace
                    if (user is StudentAssessmentSystem.Models.Users.Admin admin)
                    {
                        admin.AccessLevel = (int)numAccessLevel.Value;
                    }
                    else if (user is StudentAssessmentSystem.Models.Users.Teacher teacher)
                    {
                        teacher.EmployeeNumber = txtEmployeeNumber.Text.Trim();
                        teacher.Department = txtDepartment.Text.Trim();
                    }
                    else if (user is StudentAssessmentSystem.Models.Users.Student student)
                    {
                        student.StudentNumber = txtStudentNumber.Text.Trim();
                        student.YearLevel = (int)numYearLevel.Value;
                    }

                    bool success = _userManager.UpdateUser(user, out string error);

                    if (success)
                    {
                        MessageBox.Show("User updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show($"Failed to update user: {error}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Create new user
                    UserRole userRole = (UserRole)Enum.Parse(typeof(UserRole), role);

                    // ✅ FIXED: Create new user with full namespace
                    switch (userRole)
                    {
                        case UserRole.Admin:
                            user = new StudentAssessmentSystem.Models.Users.Admin
                            {
                                AccessLevel = (int)numAccessLevel.Value
                            };
                            break;
                        case UserRole.Teacher:
                            user = new StudentAssessmentSystem.Models.Users.Teacher
                            {
                                EmployeeNumber = txtEmployeeNumber.Text.Trim(),
                                Department = txtDepartment.Text.Trim()
                            };
                            break;
                        case UserRole.Student:
                            user = new StudentAssessmentSystem.Models.Users.Student
                            {
                                StudentNumber = txtStudentNumber.Text.Trim(),
                                YearLevel = (int)numYearLevel.Value
                            };
                            break;
                    }

                    user.Username = txtUsername.Text.Trim();
                    var authService = new AuthenticationService();
                    user.PasswordHash = authService.HashPassword(txtPassword.Text);
                    user.FirstName = txtFirstName.Text.Trim();
                    user.LastName = txtLastName.Text.Trim();
                    user.Email = txtEmail.Text.Trim();
                    user.Role = userRole;

                    bool success = _userManager.RegisterUser(user, out string error);

                    if (success)
                    {
                        MessageBox.Show("User created successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show($"Failed to create user: {error}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}