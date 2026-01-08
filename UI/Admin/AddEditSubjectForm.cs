using System;
using System.Drawing;
using System.Windows.Forms;
using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.Models.Academic;

namespace StudentAssessmentSystem.UI.Forms.Admin
{
    public partial class AddEditSubjectForm : Form
    {
        private SubjectManager _subjectManager;
        private Subject _existingSubject;
        private bool _isEditMode;

        // UI Controls
        private Label lblTitle;
        private Label lblSubjectCode;
        private TextBox txtSubjectCode;
        private Label lblSubjectName;
        private TextBox txtSubjectName;
        private Label lblUnits;
        private NumericUpDown numUnits;
        private Label lblDescription;
        private TextBox txtDescription;
        private Button btnSave;
        private Button btnCancel;

        public AddEditSubjectForm()
        {
            _subjectManager = new SubjectManager();
            _isEditMode = false;
            InitializeComponent();
        }

        public AddEditSubjectForm(Subject subject)
        {
            _subjectManager = new SubjectManager();
            _existingSubject = subject;
            _isEditMode = true;
            InitializeComponent();
            LoadSubjectData();
        }

        private void InitializeComponent()
        {
            this.Text = _isEditMode ? "Edit Subject" : "Add New Subject";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int yPos = 20;

            // Title
            lblTitle = new Label
            {
                Text = _isEditMode ? "✏️ Edit Subject" : "➕ Add New Subject",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, yPos),
                Size = new Size(450, 30)
            };
            this.Controls.Add(lblTitle);
            yPos += 45;

            // Subject Code
            lblSubjectCode = new Label
            {
                Text = "Subject Code: *",
                Location = new Point(20, yPos),
                Size = new Size(120, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblSubjectCode);

            txtSubjectCode = new TextBox
            {
                Location = new Point(150, yPos - 2),
                Size = new Size(310, 25),
                Font = new Font("Arial", 10),
                MaxLength = 20
            };
            this.Controls.Add(txtSubjectCode);
            yPos += 35;

            // Subject Name
            lblSubjectName = new Label
            {
                Text = "Subject Name: *",
                Location = new Point(20, yPos),
                Size = new Size(120, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblSubjectName);

            txtSubjectName = new TextBox
            {
                Location = new Point(150, yPos - 2),
                Size = new Size(310, 25),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(txtSubjectName);
            yPos += 35;

            // Units
            lblUnits = new Label
            {
                Text = "Units: *",
                Location = new Point(20, yPos),
                Size = new Size(120, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblUnits);

            numUnits = new NumericUpDown
            {
                Location = new Point(150, yPos - 2),
                Size = new Size(80, 25),
                Font = new Font("Arial", 10),
                Minimum = 1,
                Maximum = 10,
                Value = 3
            };
            this.Controls.Add(numUnits);
            yPos += 35;

            // Description
            lblDescription = new Label
            {
                Text = "Description:",
                Location = new Point(20, yPos),
                Size = new Size(120, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblDescription);

            txtDescription = new TextBox
            {
                Location = new Point(150, yPos - 2),
                Size = new Size(310, 80),
                Font = new Font("Arial", 10),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.Add(txtDescription);
            yPos += 90;

            Label lblNote = new Label
            {
                Text = "* Required fields",
                Location = new Point(20, yPos),
                Size = new Size(150, 15),
                Font = new Font("Arial", 8, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblNote);
            yPos += 25;

            // Buttons
            btnSave = new Button
            {
                Text = _isEditMode ? "💾 Update" : "💾 Save",
                Location = new Point(250, yPos),
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
                Location = new Point(360, yPos),
                Size = new Size(100, 35),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 10),
                Cursor = Cursors.Hand
            };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);
        }

        private void LoadSubjectData()
        {
            if (_existingSubject == null) return;

            txtSubjectCode.Text = _existingSubject.SubjectCode;
            txtSubjectName.Text = _existingSubject.SubjectName;
            numUnits.Value = _existingSubject.Units;
            txtDescription.Text = _existingSubject.Description ?? "";
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtSubjectCode.Text))
            {
                MessageBox.Show("Subject code is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubjectCode.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                MessageBox.Show("Subject name is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubjectName.Focus();
                return false;
            }

            if (numUnits.Value <= 0)
            {
                MessageBox.Show("Units must be greater than 0.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numUnits.Focus();
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
                if (_isEditMode)
                {
                    // Update existing subject
                    _existingSubject.SubjectCode = txtSubjectCode.Text.Trim().ToUpper();
                    _existingSubject.SubjectName = txtSubjectName.Text.Trim();
                    _existingSubject.Units = (int)numUnits.Value;
                    _existingSubject.Description = txtDescription.Text.Trim();

                    bool success = _subjectManager.UpdateSubject(_existingSubject, out string error);

                    if (success)
                    {
                        MessageBox.Show("Subject updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show($"Failed to update subject: {error}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Create new subject
                    Subject newSubject = new Subject
                    {
                        SubjectCode = txtSubjectCode.Text.Trim().ToUpper(),
                        SubjectName = txtSubjectName.Text.Trim(),
                        Units = (int)numUnits.Value,
                        Description = txtDescription.Text.Trim(),
                        IsActive = true
                    };

                    int subjectId = _subjectManager.AddSubject(newSubject, out string error);

                    if (subjectId > 0)
                    {
                        MessageBox.Show("Subject created successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show($"Failed to create subject: {error}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving subject: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}