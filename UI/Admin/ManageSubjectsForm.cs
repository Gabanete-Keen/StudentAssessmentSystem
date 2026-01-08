using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.Models.Academic;

namespace StudentAssessmentSystem.UI.Forms.Admin
{
    public partial class ManageSubjectsForm : Form
    {
        private SubjectManager _subjectManager;
        private Subject _selectedSubject;

        // UI Controls
        private Label lblTitle;
        private TextBox txtSearch;
        private Button btnClearSearch;
        private DataGridView dgvSubjects;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Label lblStats;
        private Label lblTotalSubjects;
        private Label lblTotalUnits;
        private Label lblAverageUnits;

        public ManageSubjectsForm()
        {
            _subjectManager = new SubjectManager();
            InitializeComponent();
            LoadSubjects();
            LoadStatistics();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Subjects";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // Title
            lblTitle = new Label
            {
                Text = "📚 Subject Management",
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
                Size = new Size(250, 25),
                Font = new Font("Arial", 10)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            this.Controls.Add(txtSearch);

            // Clear Search Button
            btnClearSearch = new Button
            {
                Text = "Clear",
                Location = new Point(345, 61),
                Size = new Size(70, 28),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 9),
                Cursor = Cursors.Hand
            };
            btnClearSearch.Click += BtnClearSearch_Click;
            this.Controls.Add(btnClearSearch);

            // DataGridView
            dgvSubjects = new DataGridView
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
            dgvSubjects.SelectionChanged += DgvSubjects_SelectionChanged;
            this.Controls.Add(dgvSubjects);

            // Action Buttons
            btnAdd = new Button
            {
                Text = "➕ Add Subject",
                Location = new Point(20, 520),
                Size = new Size(130, 35),
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
                Location = new Point(160, 520),
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
                Location = new Point(270, 520),
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
                Location = new Point(380, 520),
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
                Size = new Size(180, 150),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(grpStats);

            lblStats = new Label
            {
                Text = "📊 Subject Stats",
                Location = new Point(10, 25),
                Size = new Size(160, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            grpStats.Controls.Add(lblStats);

            lblTotalSubjects = new Label
            {
                Text = "Total Subjects: 0",
                Location = new Point(15, 55),
                Size = new Size(150, 20),
                Font = new Font("Arial", 9)
            };
            grpStats.Controls.Add(lblTotalSubjects);

            lblTotalUnits = new Label
            {
                Text = "Total Units: 0",
                Location = new Point(15, 85),
                Size = new Size(150, 20),
                Font = new Font("Arial", 9)
            };
            grpStats.Controls.Add(lblTotalUnits);

            lblAverageUnits = new Label
            {
                Text = "Avg Units: 0",
                Location = new Point(15, 115),
                Size = new Size(150, 20),
                Font = new Font("Arial", 9)
            };
            grpStats.Controls.Add(lblAverageUnits);

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

        private void LoadSubjects()
        {
            try
            {
                List<Subject> subjects = _subjectManager.GetAllSubjects();
                DisplaySubjects(subjects);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplaySubjects(List<Subject> subjects)
        {
            dgvSubjects.DataSource = null;
            dgvSubjects.Rows.Clear();
            dgvSubjects.Columns.Clear();

            // Setup columns
            dgvSubjects.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SubjectId",
                HeaderText = "ID",
                Width = 50,
                Visible = false
            });

            dgvSubjects.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SubjectCode",
                HeaderText = "Code",
                Width = 100
            });

            dgvSubjects.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SubjectName",
                HeaderText = "Subject Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvSubjects.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Units",
                HeaderText = "Units",
                Width = 70
            });

            dgvSubjects.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "Description",
                Width = 200
            });

            // Populate rows
            foreach (Subject subject in subjects)
            {
                string description = subject.Description ?? "N/A";
                if (description.Length > 50)
                    description = description.Substring(0, 47) + "...";

                dgvSubjects.Rows.Add(
                    subject.SubjectId,
                    subject.SubjectCode,
                    subject.SubjectName,
                    subject.Units,
                    description
                );
            }

            UpdateButtonStates();
        }

        private void LoadStatistics()
        {
            try
            {
                SubjectStatistics stats = _subjectManager.GetStatistics();

                lblTotalSubjects.Text = $"Total Subjects: {stats.TotalSubjects}";
                lblTotalUnits.Text = $"Total Units: {stats.TotalUnits}";
                lblAverageUnits.Text = $"Avg Units: {stats.AverageUnits:F1}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtSearch.Text.Trim();
                List<Subject> filtered = _subjectManager.SearchSubjects(searchText);
                DisplaySubjects(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadSubjects();
        }

        private void DgvSubjects_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSubjects.SelectedRows.Count > 0)
            {
                int subjectId = Convert.ToInt32(dgvSubjects.SelectedRows[0].Cells["SubjectId"].Value);
                _selectedSubject = _subjectManager.GetSubjectById(subjectId);
            }
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = dgvSubjects.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            AddEditSubjectForm form = new AddEditSubjectForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadSubjects();
                LoadStatistics();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedSubject == null) return;

            AddEditSubjectForm form = new AddEditSubjectForm(_selectedSubject);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadSubjects();
                LoadStatistics();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedSubject == null) return;

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete subject '{_selectedSubject.SubjectCode} - {_selectedSubject.SubjectName}'?\n\nThis action cannot be undone!",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool success = _subjectManager.DeleteSubject(_selectedSubject.SubjectId, out string error);

                if (success)
                {
                    MessageBox.Show("Subject deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSubjects();
                    LoadStatistics();
                }
                else
                {
                    MessageBox.Show($"Failed to delete subject: {error}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadSubjects();
            LoadStatistics();
            MessageBox.Show("Subject list refreshed!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}