using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    /// <summary>
    /// Form for teachers to create and manage test instances (test sessions)
    /// </summary>
    public partial class TestAdministrationForm : Form
    {
        private TestRepository _testRepository;
        private TestInstanceRepository _instanceRepository;
        private int _teacherId;

        // UI Controls
        private Label lblTitle;
        private Label lblSelectTest;
        private ComboBox cmbTests;
        private Label lblInstanceTitle;
        private TextBox txtInstanceTitle;
        private Label lblStartDate;
        private DateTimePicker dtpStartDate;
        private Label lblStartTime;
        private DateTimePicker dtpStartTime;
        private Label lblEndDate;
        private DateTimePicker dtpEndDate;
        private Label lblEndTime;
        private DateTimePicker dtpEndTime;
        private DataGridView dgvInstances;
        private Button btnCreateInstance;
        private Button btnDeactivate;
        private Button btnClose;

        public TestAdministrationForm()
        {
            _teacherId = SessionManager.CurrentUserId;
            _testRepository = new TestRepository();
            _instanceRepository = new TestInstanceRepository();

            InitializeComponent();
            LoadTests();
            LoadTestInstances();
        }

        private void InitializeComponent()
        {
            this.Text = "Test Administration";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            int yPos = 20;

            // Title
            lblTitle = new Label
            {
                Text = "📋 Test Administration - Create Test Sessions",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, yPos),
                Size = new Size(900, 30),
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(lblTitle);
            yPos += 50;

            // Select Test
            lblSelectTest = new Label
            {
                Text = "Select Test:",
                Location = new Point(20, yPos),
                Size = new Size(100, 20)
            };
            this.Controls.Add(lblSelectTest);

            cmbTests = new ComboBox
            {
                Location = new Point(130, yPos),
                Size = new Size(400, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbTests);
            yPos += 40;

            // Instance Title
            lblInstanceTitle = new Label
            {
                Text = "Session Title:",
                Location = new Point(20, yPos),
                Size = new Size(100, 20)
            };
            this.Controls.Add(lblInstanceTitle);

            txtInstanceTitle = new TextBox
            {
                Location = new Point(130, yPos),
                Size = new Size(400, 25),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(txtInstanceTitle);
            yPos += 40;

            // Start Date/Time
            lblStartDate = new Label
            {
                Text = "Start Date:",
                Location = new Point(20, yPos),
                Size = new Size(100, 20)
            };
            this.Controls.Add(lblStartDate);

            dtpStartDate = new DateTimePicker
            {
                Location = new Point(130, yPos),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short
            };
            this.Controls.Add(dtpStartDate);

            lblStartTime = new Label
            {
                Text = "Time:",
                Location = new Point(290, yPos),
                Size = new Size(40, 20)
            };
            this.Controls.Add(lblStartTime);

            dtpStartTime = new DateTimePicker
            {
                Location = new Point(340, yPos),
                Size = new Size(100, 25),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true
            };
            this.Controls.Add(dtpStartTime);
            yPos += 40;

            // End Date/Time
            lblEndDate = new Label
            {
                Text = "End Date:",
                Location = new Point(20, yPos),
                Size = new Size(100, 20)
            };
            this.Controls.Add(lblEndDate);

            dtpEndDate = new DateTimePicker
            {
                Location = new Point(130, yPos),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short
            };
            this.Controls.Add(dtpEndDate);

            lblEndTime = new Label
            {
                Text = "Time:",
                Location = new Point(290, yPos),
                Size = new Size(40, 20)
            };
            this.Controls.Add(lblEndTime);

            dtpEndTime = new DateTimePicker
            {
                Location = new Point(340, yPos),
                Size = new Size(100, 25),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true
            };
            this.Controls.Add(dtpEndTime);
            yPos += 50;

            // Create Button
            btnCreateInstance = new Button
            {
                Text = "✅ Create Test Session",
                Location = new Point(130, yPos),
                Size = new Size(200, 40),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCreateInstance.Click += BtnCreateInstance_Click;
            this.Controls.Add(btnCreateInstance);
            yPos += 60;

            // Test Instances Grid
            Label lblInstances = new Label
            {
                Text = "📊 My Test Sessions:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(20, yPos),
                Size = new Size(500, 25)
            };
            this.Controls.Add(lblInstances);
            yPos += 35;

            dgvInstances = new DataGridView
            {
                Location = new Point(20, yPos),
                Size = new Size(940, 300),
                BackgroundColor = Color.White,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            SetupInstanceGridColumns();
            this.Controls.Add(dgvInstances);
            yPos += 310;

            // Buttons
            btnDeactivate = new Button
            {
                Text = "🚫 Deactivate Selected",
                Location = new Point(20, yPos),
                Size = new Size(180, 35),
                BackColor = Color.LightCoral,
                Font = new Font("Arial", 10),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnDeactivate.Click += BtnDeactivate_Click;
            this.Controls.Add(btnDeactivate);

            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(860, yPos),
                Size = new Size(100, 35),
                BackColor = Color.LightGray,
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            dgvInstances.SelectionChanged += (s, e) =>
            {
                btnDeactivate.Enabled = dgvInstances.SelectedRows.Count > 0;
            };
        }

        private void SetupInstanceGridColumns()
        {
            dgvInstances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "InstanceId",
                DataPropertyName = "InstanceId",
                Visible = false
            });

            dgvInstances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SessionTitle",
                DataPropertyName = "SessionTitle",
                HeaderText = "Session Title",
                Width = 250
            });

            dgvInstances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TestTitle",
                DataPropertyName = "TestTitle",
                HeaderText = "Test",
                Width = 200
            });

            dgvInstances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StartDate",
                DataPropertyName = "StartDate",
                HeaderText = "Start Date",
                Width = 150
            });

            dgvInstances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EndDate",
                DataPropertyName = "EndDate",
                HeaderText = "End Date",
                Width = 150
            });

            dgvInstances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                DataPropertyName = "Status",
                HeaderText = "Status",
                Width = 100
            });
        }

        private void LoadTests()
        {
            try
            {
                var tests = _testRepository.GetTestsByTeacher(_teacherId);

                cmbTests.Items.Clear();
                cmbTests.DisplayMember = "TestTitle";
                cmbTests.ValueMember = "TestId";

                foreach (var test in tests)
                {
                    cmbTests.Items.Add(test);
                }

                if (cmbTests.Items.Count > 0)
                    cmbTests.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tests: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTestInstances()
        {
            try
            {
                var instances = _instanceRepository.GetInstancesByTeacher(_teacherId);
                var testRepo = new TestRepository();

                var displayData = new List<object>();

                foreach (var instance in instances)
                {
                    var test = testRepo.GetById(instance.TestId);
                    string status = instance.IsActive ? "Active" : "Inactive";

                    displayData.Add(new
                    {
                        InstanceId = instance.InstanceId,
                        SessionTitle = instance.InstanceTitle,
                        TestTitle = test?.TestTitle ?? "Unknown",
                        StartDate = instance.StartDate.ToString("yyyy-MM-dd HH:mm"),
                        EndDate = instance.EndDate.ToString("yyyy-MM-dd HH:mm"),
                        Status = status
                    });
                }

                dgvInstances.DataSource = displayData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading test instances: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCreateInstance_Click(object sender, EventArgs e)
        {
            if (cmbTests.SelectedItem == null)
            {
                MessageBox.Show("Please select a test.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtInstanceTitle.Text))
            {
                MessageBox.Show("Please enter a session title.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedTest = (Test)cmbTests.SelectedItem;

                // Combine date and time
                DateTime startDateTime = dtpStartDate.Value.Date + dtpStartTime.Value.TimeOfDay;
                DateTime endDateTime = dtpEndDate.Value.Date + dtpEndTime.Value.TimeOfDay;

                if (endDateTime <= startDateTime)
                {
                    MessageBox.Show("End date/time must be after start date/time.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var instance = new TestInstance
                {
                    TestId = selectedTest.TestId,
                    TeacherId = _teacherId,
                    InstanceTitle = txtInstanceTitle.Text.Trim(),
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                int instanceId = _instanceRepository.CreateTestInstance(instance);

                if (instanceId > 0)
                {
                    MessageBox.Show("Test session created successfully! Students can now take this test.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtInstanceTitle.Clear();
                    LoadTestInstances();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating test session: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeactivate_Click(object sender, EventArgs e)
        {
            if (dgvInstances.SelectedRows.Count == 0)
                return;

            try
            {
                int instanceId = Convert.ToInt32(dgvInstances.SelectedRows[0].Cells["InstanceId"].Value);

                var result = MessageBox.Show("Are you sure you want to deactivate this test session?",
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _instanceRepository.UpdateInstanceStatus(instanceId, false);

                    if (success)
                    {
                        MessageBox.Show("Test session deactivated.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadTestInstances();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deactivating test session: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
