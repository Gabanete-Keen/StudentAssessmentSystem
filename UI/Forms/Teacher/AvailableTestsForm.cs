using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    public partial class AvailableTestsForm : Form
    {
        private ListBox lstAvailableTests;
        private Button btnTakeTest;
        private Button btnClose;

        public AvailableTestsForm()
        {
            InitializeComponent();
            LoadAvailableTests();
        }

        private void InitializeComponent()
        {
            this.Text = "Available Tests";
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            Label lblTitle = new Label();
            lblTitle.Text = "Available Tests";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(400, 30);
            this.Controls.Add(lblTitle);

            lstAvailableTests = new ListBox();
            lstAvailableTests.Location = new Point(20, 60);
            lstAvailableTests.Size = new Size(540, 280);
            lstAvailableTests.Font = new Font("Arial", 10);
            this.Controls.Add(lstAvailableTests);

            btnTakeTest = new Button();
            btnTakeTest.Text = "Take Selected Test";
            btnTakeTest.Location = new Point(20, 360);
            btnTakeTest.Size = new Size(150, 35);
            btnTakeTest.BackColor = Color.LightGreen;
            btnTakeTest.Font = new Font("Arial", 10, FontStyle.Bold);
            btnTakeTest.Cursor = Cursors.Hand;
            btnTakeTest.Click += BtnTakeTest_Click;
            this.Controls.Add(btnTakeTest);

            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Location = new Point(480, 360);
            btnClose.Size = new Size(80, 35);
            btnClose.BackColor = Color.LightGray;
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadAvailableTests()
        {
            // TODO: Load tests available for current student's sections
            lstAvailableTests.Items.Add("Sample Midterm Exam - Data Structures");
            lstAvailableTests.Items.Add("Quiz 1 - Introduction to Programming");
            lstAvailableTests.Items.Add("Final Exam - Algorithms");
        }

        private void BtnTakeTest_Click(object sender, EventArgs e)
        {
            if (lstAvailableTests.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a test to take.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Taking test feature will be implemented next!", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // TODO: Open TakeTestForm
            // TakeTestForm form = new TakeTestForm(selectedTestId);
            // form.ShowDialog();
        }
    }
}
