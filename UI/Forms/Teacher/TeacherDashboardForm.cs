using System;
using System.Drawing;
using System.Windows.Forms;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Utilities;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    public partial class TeacherDashboardForm : Form
    {
        private Models.Users.Teacher currentTeacher;
        private Label lblWelcome;
        private Label lblDept;
        private GroupBox grpQuickActions;
        private Button btnCreateTest;
        private Button btnMyTests;
        private Button btnQuestionBank;
        private Button btnManageQuestions; // NEW BUTTON
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
                MessageBox.Show($"Error initializing teacher dashboard: {ex.Message}\nTrace: {ex.StackTrace}",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTeacherData()
        {
            try
            {
                currentTeacher = SessionManager.CurrentUser as Models.Users.Teacher;
                if (currentTeacher != null)
                {
                    lblWelcome.Text = $"Welcome, {currentTeacher.FullName}!";
                    lblDept.Text = $"Department: {currentTeacher.Department ?? "Not Assigned"}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teacher data: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Teacher Dashboard";
            this.Size = new Size(600, 500); // Increased height
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Welcome Label
            lblWelcome = new Label
            {
                Text = "Welcome, Teacher!",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(550, 30)
            };
            this.Controls.Add(lblWelcome);

            // Department info
            lblDept = new Label
            {
                Text = "Department: Loading...",
                Location = new Point(20, 55),
                Size = new Size(400, 20),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(lblDept);

            // Quick Actions Group
            grpQuickActions = new GroupBox
            {
                Text = "Quick Actions",
                Location = new Point(20, 90),
                Size = new Size(550, 330), // Increased height
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(grpQuickActions);

            // ===== ROW 1: Create Test & My Tests =====
            // Create Test Button
            btnCreateTest = new Button
            {
                Text = "➕ Create New Test",
                Location = new Point(30, 40),
                Size = new Size(220, 50),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightGreen,
                Cursor = Cursors.Hand
            };
            btnCreateTest.Click += BtnCreateTest_Click;
            grpQuickActions.Controls.Add(btnCreateTest);

            // My Tests Button
            btnMyTests = new Button
            {
                Text = "📋 View My Tests",
                Location = new Point(280, 40),
                Size = new Size(220, 50),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightBlue,
                Cursor = Cursors.Hand
            };
            btnMyTests.Click += BtnMyTests_Click;
            grpQuickActions.Controls.Add(btnMyTests);

            // ===== ROW 2: Question Bank & Manage Questions (NEW!) =====
            // Question Bank Button
            btnQuestionBank = new Button
            {
                Text = "❓ Question Bank",
                Location = new Point(30, 110),
                Size = new Size(220, 50),
                Font = new Font("Arial", 10),
                BackColor = Color.LightYellow,
                Cursor = Cursors.Hand
            };
            btnQuestionBank.Click += BtnQuestionBank_Click;
            grpQuickActions.Controls.Add(btnQuestionBank);

            // ===== 🎯 NEW BUTTON: MANAGE QUESTIONS =====
            btnManageQuestions = new Button
            {
                Text = "📝 Manage Questions",
                Location = new Point(280, 110),
                Size = new Size(220, 50),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(173, 216, 230), // Light Blue
                Cursor = Cursors.Hand
            };
            btnManageQuestions.Click += BtnManageQuestions_Click;
            grpQuickActions.Controls.Add(btnManageQuestions);

            // ===== ROW 3: View Analysis (centered) =====
            // View Analysis Button
            btnViewAnalysis = new Button
            {
                Text = "📊 Item Analysis",
                Location = new Point(155, 180), // Centered
                Size = new Size(220, 50),
                Font = new Font("Arial", 10),
                BackColor = Color.LightCoral,
                Cursor = Cursors.Hand
            };
            btnViewAnalysis.Click += BtnViewAnalysis_Click;
            grpQuickActions.Controls.Add(btnViewAnalysis);

            // Info Label
            lblInfo = new Label
            {
                Text = "Click on any button above to get started!\n\n💡 New: Use 'Manage Questions' to edit or delete questions from your tests.",
                Location = new Point(30, 245),
                Size = new Size(490, 70),
                Font = new Font("Arial", 9),
                ForeColor = Color.Gray
            };
            grpQuickActions.Controls.Add(lblInfo);

            // Logout Button
            btnLogout = new Button
            {
                Text = "Logout",
                Location = new Point(470, 435),
                Size = new Size(100, 35),
                Font = new Font("Arial", 10),
                BackColor = Color.LightGray,
                Cursor = Cursors.Hand
            };
            btnLogout.Click += BtnLogout_Click;
            this.Controls.Add(btnLogout);
        }

        // ===== EVENT HANDLERS =====

        private void BtnCreateTest_Click(object sender, EventArgs e)
        {
            try
            {
                TestCreationForm form = new TestCreationForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening test creation form: {ex.Message}",
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
                MessageBox.Show($"Error opening my tests: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnQuestionBank_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Question Bank feature coming soon!", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 🎯 NEW: MANAGE QUESTIONS BUTTON CLICK
        /// Opens dialog to select test, then opens ManageQuestionsForm
        /// </summary>
        private void BtnManageQuestions_Click(object sender, EventArgs e)
        {
            try
            {
                // Get teacher ID
                int teacherId = SessionManager.GetCurrentUserId();

                // Load teacher's tests
                var testRepo = new TestRepository();
                var tests = testRepo.GetTestsByTeacher(teacherId);

                if (tests == null || tests.Count == 0)
                {
                    MessageBox.Show("You haven't created any tests yet.\nPlease create a test first before managing questions.",
                        "No Tests Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Create selection dialog
                Form selectDialog = new Form
                {
                    Text = "Select Test to Manage",
                    Size = new Size(500, 180),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.White
                };

                Label lblSelect = new Label
                {
                    Text = "Select a test to manage its questions:",
                    Location = new Point(20, 20),
                    Size = new Size(450, 20),
                    Font = new Font("Arial", 10, FontStyle.Bold)
                };
                selectDialog.Controls.Add(lblSelect);

                ComboBox cmbTests = new ComboBox
                {
                    Location = new Point(20, 50),
                    Size = new Size(440, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DisplayMember = "TestTitle",
                    ValueMember = "TestId",
                    Font = new Font("Arial", 10)
                };
                cmbTests.DataSource = tests;
                selectDialog.Controls.Add(cmbTests);

                Button btnOk = new Button
                {
                    Text = "Open",
                    Location = new Point(270, 95),
                    Size = new Size(90, 35),
                    DialogResult = DialogResult.OK,
                    BackColor = Color.LightGreen,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                selectDialog.Controls.Add(btnOk);

                Button btnCancelDialog = new Button
                {
                    Text = "Cancel",
                    Location = new Point(370, 95),
                    Size = new Size(90, 35),
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.LightGray,
                    Font = new Font("Arial", 10),
                    Cursor = Cursors.Hand
                };
                selectDialog.Controls.Add(btnCancelDialog);

                // Show dialog and open ManageQuestionsForm
                if (selectDialog.ShowDialog() == DialogResult.OK && cmbTests.SelectedItem != null)
                {
                    var selectedTest = (Test)cmbTests.SelectedItem;
                    ManageQuestionsForm manageForm = new ManageQuestionsForm(selectedTest.TestId, selectedTest.TestTitle);
                    manageForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening manage questions: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnViewAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                ItemAnalysisForm form = new ItemAnalysisForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening item analysis: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBoxIcon.Question);

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
                MessageBox.Show($"Error during logout: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
