using StudentAssessmentSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Student
{
    public partial class StudentDashboardForm : Form
    {
        private Student _currentStudent;

        private Label lblWelcome;
        private GroupBox grpActions;
        private Button btnTakeTest;
        private Button btnMyResults;
        private Button btnLogout;

        public StudentDashboardForm()
        {
            _currentStudent = SessionManager.CurrentUser as Student;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Student Dashboard";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // Welcome Label
            lblWelcome = new Label();
            lblWelcome.Text = $"Welcome, {_currentStudent.FullName}!";
            lblWelcome.Font = new Font("Arial", 16, FontStyle.Bold);
            lblWelcome.Location = new Point(20, 20);
            lblWelcome.Size = new Size(450, 30);
            this.Controls.Add(lblWelcome);

            // Student info
            Label lblInfo = new Label();
            lblInfo.Text = $"Student Number: {_currentStudent.StudentNumber} | Year Level: {_currentStudent.YearLevel}";
            lblInfo.Location = new Point(20, 55);
            lblInfo.Size = new Size(450, 20);
            lblInfo.Font = new Font("Arial", 10);
            this.Controls.Add(lblInfo);

            // Actions Group
            grpActions = new GroupBox();
            grpActions.Text = "What would you like to do?";
            grpActions.Location = new Point(20, 90);
            grpActions.Size = new Size(450, 230);
            grpActions.Font = new Font("Arial", 10, FontStyle.Bold);
            this.Controls.Add(grpActions);

            // Take Test Button
            btnTakeTest = new Button();
            btnTakeTest.Text = "Take a Test";
            btnTakeTest.Location = new Point(120, 50);
            btnTakeTest.Size = new Size(200, 60);
            btnTakeTest.Font = new Font("Arial", 12, FontStyle.Bold);
            btnTakeTest.BackColor = Color.LightGreen;
            btnTakeTest.Cursor = Cursors.Hand;
            btnTakeTest.Click += BtnTakeTest_Click;
            grpActions.Controls.Add(btnTakeTest);

            // My Results Button
            btnMyResults = new Button();
            btnMyResults.Text = "View My Results";
            btnMyResults.Location = new Point(120, 130);
            btnMyResults.Size = new Size(200, 60);
            btnMyResults.Font = new Font("Arial", 12, FontStyle.Bold);
            btnMyResults.BackColor = Color.LightBlue;
            btnMyResults.Cursor = Cursors.Hand;
            btnMyResults.Click += BtnMyResults_Click;
            grpActions.Controls.Add(btnMyResults);

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

        private void BtnTakeTest_Click(object sender, EventArgs e)
        {
            // Open Available Tests Form
            AvailableTestsForm form = new AvailableTestsForm();
            form.ShowDialog();
        }

        private void BtnMyResults_Click(object sender, EventArgs e)
        {
            MessageBox.Show("My Results feature coming soon!", "Info");
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