using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    public partial class MyTestsForm : Form
    {
        private TestManager _testManager;
        private ListBox lstTests;
        private Button btnClose;

        public MyTestsForm()
        {
            _testManager = new TestManager();
            InitializeComponent();
            LoadTests();
        }

        private void InitializeComponent()
        {
            this.Text = "My Tests";
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            Label lblTitle = new Label();
            lblTitle.Text = "My Created Tests";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(400, 30);
            this.Controls.Add(lblTitle);

            lstTests = new ListBox();
            lstTests.Location = new Point(20, 60);
            lstTests.Size = new Size(540, 300);
            lstTests.Font = new Font("Arial", 10);
            this.Controls.Add(lstTests);

            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Location = new Point(480, 375);
            btnClose.Size = new Size(80, 30);
            btnClose.BackColor = Color.LightGray;
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadTests()
        {
            try
            {
                int teacherId = SessionManager.GetCurrentUserId();
                var tests = _testManager.GetTestsByTeacher(teacherId);

                lstTests.Items.Clear();

                if (tests.Count == 0)
                {
                    lstTests.Items.Add("No tests created yet.");
                }
                else
                {
                    foreach (var test in tests)
                    {
                        lstTests.Items.Add($"{test.TestTitle} - {test.TestType} ({test.DurationMinutes} mins)");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tests:\n{ex.Message}", "Error");
            }
        }
    }
}