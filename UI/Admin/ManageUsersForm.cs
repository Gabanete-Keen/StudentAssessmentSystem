using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.Models.Users;
using StudentAssessmentSystem.Models.Enums;

namespace StudentAssessmentSystem.UI.Forms.Admin
{
    public partial class ManageUsersForm : Form
    {
        private UserManager _userManager;
        private User _selectedUser;

        // UI Controls
        private Label lblTitle;
        private TextBox txtSearch;
        private ComboBox cboRoleFilter;
        private Button btnClearFilter;
        private DataGridView dgvUsers;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Label lblStats;
        private Label lblTotalUsers;
        private Label lblAdmins;
        private Label lblTeachers;
        private Label lblStudents;

        public ManageUsersForm()
        {
            _userManager = new UserManager();
            InitializeComponent();
            LoadUsers();
            LoadStatistics();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Users";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // Title
            lblTitle = new Label
            {
                Text = "👥 User Management",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(300, 30)
            };
            this.Controls.Add(lblTitle);

            // Search Box
            Label lblSearch = new Label
            {
                Text = "Search:",
                Location = new Point(20, 65),
                Size = new Size(60, 20),
                Font = new Font("Arial", 9)
            };
            this.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(85, 63),
                Size = new Size(200, 25),
                Font = new Font("Arial", 10)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            this.Controls.Add(txtSearch);

            // Role Filter
            Label lblRole = new Label
            {
                Text = "Role:",
                Location = new Point(310, 65),
                Size = new Size(40, 20),
                Font = new Font("Arial", 9)
            };
            this.Controls.Add(lblRole);

            cboRoleFilter = new ComboBox
            {
                Location = new Point(355, 63),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };
            cboRoleFilter.Items.AddRange(new object[] { "All Roles", "Admin", "Teacher", "Student" });
            cboRoleFilter.SelectedIndex = 0;
            cboRoleFilter.SelectedIndexChanged += CboRoleFilter_SelectedIndexChanged;
            this.Controls.Add(cboRoleFilter);

            // Clear Filter Button
            btnClearFilter = new Button
            {
                Text = "Clear",
                Location = new Point(490, 61),
                Size = new Size(70, 28),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 9),
                Cursor = Cursors.Hand
            };
            btnClearFilter.Click += BtnClearFilter_Click;
            this.Controls.Add(btnClearFilter);

            // DataGridView
            dgvUsers = new DataGridView
            {
                Location = new Point(20, 110),
                Size = new Size(750, 400),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };
            dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;
            this.Controls.Add(dgvUsers);

            // Action Buttons
            btnAdd = new Button
            {
                Text = "➕ Add User",
                Location = new Point(20, 520),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnEdit = new Button
            {
                Text = "✏️ Edit",
                Location = new Point(150, 520),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnDelete = new Button
            {
                Text = "🗑️ Delete",
                Location = new Point(260, 520),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(370, 520),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.Click += BtnRefresh_Click;
            this.Controls.Add(btnRefresh);

            // Statistics Panel
            GroupBox grpStats = new GroupBox
            {
                Text = "Statistics",
                Location = new Point(790, 110),
                Size = new Size(180, 180),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(grpStats);

            lblStats = new Label
            {
                Text = "📊 User Stats",
                Location = new Point(10, 25),
                Size = new Size(160, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            grpStats.Controls.Add(lblStats);

            lblTotalUsers = new Label
            {
                Text = "Total Users: 0",
                Location = new Point(15, 55),
                Size = new Size(150, 20),
                Font = new Font("Arial", 9)
            };
            grpStats.Controls.Add(lblTotalUsers);

            lblAdmins = new Label
            {
                Text = "Admins: 0",
                Location = new Point(15, 85),
                Size = new Size(150, 20),
                Font = new Font("Arial", 9)
            };
            grpStats.Controls.Add(lblAdmins);

            lblTeachers = new Label
            {
                Text = "Teachers: 0",
                Location = new Point(15, 115),
                Size = new Size(150, 20),
                Font = new Font("Arial", 9)
            };
            grpStats.Controls.Add(lblTeachers);

            lblStudents = new Label
            {
                Text = "Students: 0",
                Location = new Point(15, 145),
                Size = new Size(150, 20),
                Font = new Font("Arial", 9)
            };
            grpStats.Controls.Add(lblStudents);

            // Close Button
            Button btnClose = new Button
            {
                Text = "Close",
                Location = new Point(870, 520),
                Size = new Size(100, 35),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 9),
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadUsers()
        {
            try
            {
                List<User> users = _userManager.GetAllUsers();
                DisplayUsers(users);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayUsers(List<User> users)
        {
            dgvUsers.DataSource = null;
            dgvUsers.Rows.Clear();
            dgvUsers.Columns.Clear();

            // Setup columns
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UserId",
                HeaderText = "ID",
                Width = 50,
                Visible = false
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Username",
                HeaderText = "Username",
                Width = 120
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FullName",
                HeaderText = "Full Name",
                Width = 150
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email",
                Width = 180
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Role",
                HeaderText = "Role",
                Width = 80
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "RoleDetails",
                HeaderText = "Details",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // Populate rows
            foreach (User user in users)
            {
                string roleDetails = GetRoleDetails(user);

                dgvUsers.Rows.Add(
                    user.UserId,
                    user.Username,
                    user.FullName,
                    user.Email,
                    user.Role.ToString(),
                    roleDetails
                );
            }

            UpdateButtonStates();
        }

        private string GetRoleDetails(User user)
        {
            if (user is StudentAssessmentSystem.Models.Users.Admin admin)
                return $"Access Level: {admin.AccessLevel}";
            else if (user is StudentAssessmentSystem.Models.Users.Teacher teacher)
                return $"Dept: {teacher.Department ?? "N/A"}";
            else if (user is StudentAssessmentSystem.Models.Users.Student student)
                return $"Year: {student.YearLevel}";
            return "";
        }

        private void LoadStatistics()
        {
            try
            {
                UserStatistics stats = _userManager.GetUserStatistics();

                lblTotalUsers.Text = $"Total Users: {stats.TotalUsers}";
                lblAdmins.Text = $"Admins: {stats.TotalAdmins} ({stats.AdminPercentage:F1}%)";
                lblTeachers.Text = $"Teachers: {stats.TotalTeachers} ({stats.TeacherPercentage:F1}%)";
                lblStudents.Text = $"Students: {stats.TotalStudents} ({stats.StudentPercentage:F1}%)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void CboRoleFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cboRoleFilter.SelectedIndex = 0;
            LoadUsers();
        }

        private void ApplyFilters()
        {
            try
            {
                string searchText = txtSearch.Text.Trim();
                UserRole? role = null;

                if (cboRoleFilter.SelectedIndex > 0)
                {
                    role = (UserRole)Enum.Parse(typeof(UserRole), cboRoleFilter.SelectedItem.ToString());
                }

                List<User> filtered = _userManager.SearchUsers(searchText, role);
                DisplayUsers(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserId"].Value);
                _selectedUser = _userManager.GetUserById(userId);
            }
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = dgvUsers.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            AddEditUserForm form = new AddEditUserForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
                LoadStatistics();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedUser == null) return;

            AddEditUserForm form = new AddEditUserForm(_selectedUser);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
                LoadStatistics();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedUser == null) return;

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete user '{_selectedUser.FullName}'?\n\nThis action cannot be undone!",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool success = _userManager.DeactivateUser(_selectedUser.UserId, out string error);

                if (success)
                {
                    MessageBox.Show("User deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                    LoadStatistics();
                }
                else
                {
                    MessageBox.Show($"Failed to delete user: {error}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadUsers();
            LoadStatistics();
            MessageBox.Show("User list refreshed!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}