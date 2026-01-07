using StudentAssessmentSystem.BusinessLogic.Managers;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Enums;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StudentAssessmentSystem.UI.Forms.Teacher
{
    public partial class QuestionBankForm : Form
    {
        private readonly QuestionBankManager _questionBankManager;
        private MultipleChoiceQuestion _selectedQuestion;

        public QuestionBankForm()
        {
            InitializeComponent();
            _questionBankManager = new QuestionBankManager();
            LoadQuestions();
            LoadFilters();
            LoadStatistics();
        }

        #region Form Controls Initialization

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Question Bank Manager";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // Header Panel
            Panel headerPanel = CreateHeaderPanel();
            this.Controls.Add(headerPanel);

            // Search and Filter Panel
            Panel filterPanel = CreateFilterPanel();
            this.Controls.Add(filterPanel);

            // Main Content Panel
            Panel mainPanel = CreateMainPanel();
            this.Controls.Add(mainPanel);

            // Statistics Panel
            Panel statsPanel = CreateStatisticsPanel();
            this.Controls.Add(statsPanel);

            this.ResumeLayout(false);
        }

        private Panel CreateHeaderPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(41, 128, 185),
                Padding = new Padding(20, 10, 20, 10)
            };

            Label titleLabel = new Label
            {
                Text = "📚 Question Bank Manager",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            Label subtitleLabel = new Label
            {
                Text = "Manage your reusable question library",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(236, 240, 241),
                AutoSize = true,
                Location = new Point(20, 50)
            };

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(subtitleLabel);

            return panel;
        }

        private Panel CreateFilterPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // Search TextBox
            txtSearch = new TextBox
            {
                Location = new Point(20, 25),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10)
            };
            // FIX #1: Remove PlaceholderText property (not available in .NET Framework)
            // Instead, we'll handle it with events
            txtSearch.Text = "🔍 Search questions...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Enter += TxtSearch_Enter;
            txtSearch.Leave += TxtSearch_Leave;
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // Topic Filter
            Label lblTopic = new Label
            {
                Text = "Topic:",
                Location = new Point(290, 28),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            cboTopic = new ComboBox
            {
                Location = new Point(340, 25),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cboTopic.SelectedIndexChanged += CboFilter_SelectedIndexChanged;

            // Difficulty Filter
            Label lblDifficulty = new Label
            {
                Text = "Difficulty:",
                Location = new Point(510, 28),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            cboDifficulty = new ComboBox
            {
                Location = new Point(580, 25),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cboDifficulty.SelectedIndexChanged += CboFilter_SelectedIndexChanged;

            // Cognitive Level Filter
            Label lblCognitive = new Label
            {
                Text = "Cognitive:",
                Location = new Point(720, 28),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            cboCognitive = new ComboBox
            {
                Location = new Point(790, 25),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cboCognitive.SelectedIndexChanged += CboFilter_SelectedIndexChanged;

            // Clear Filter Button
            Button btnClearFilter = new Button
            {
                Text = "Clear",
                Location = new Point(930, 23),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnClearFilter.Click += BtnClearFilter_Click;

            // Result Count Label
            lblResultCount = new Label
            {
                Text = "Total: 0 questions",
                Location = new Point(20, 65),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray
            };

            panel.Controls.AddRange(new Control[] {
                txtSearch, lblTopic, cboTopic, lblDifficulty, cboDifficulty,
                lblCognitive, cboCognitive, btnClearFilter, lblResultCount
            });

            return panel;
        }

        private Panel CreateMainPanel()
        {
            Panel panel = new Panel
            {
                Location = new Point(20, 200),
                Size = new Size(850, 430),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // DataGridView for questions
            dgvQuestions = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(830, 350),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false
            };
            dgvQuestions.SelectionChanged += DgvQuestions_SelectionChanged;

            // Action Buttons
            btnAdd = CreateActionButton("➕ Add Question", new Point(10, 370), Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;

            btnEdit = CreateActionButton("✏️ Edit", new Point(160, 370), Color.FromArgb(52, 152, 219));
            btnEdit.Click += BtnEdit_Click;

            btnDuplicate = CreateActionButton("📋 Duplicate", new Point(260, 370), Color.FromArgb(241, 196, 15));
            btnDuplicate.Click += BtnDuplicate_Click;

            btnDelete = CreateActionButton("🗑️ Delete", new Point(390, 370), Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;

            btnView = CreateActionButton("👁️ View Details", new Point(510, 370), Color.FromArgb(155, 89, 182));
            btnView.Click += BtnView_Click;

            btnRefresh = CreateActionButton("🔄 Refresh", new Point(660, 370), Color.FromArgb(52, 73, 94));
            btnRefresh.Click += BtnRefresh_Click;

            panel.Controls.Add(dgvQuestions);
            panel.Controls.AddRange(new Control[] {
                btnAdd, btnEdit, btnDuplicate, btnDelete, btnView, btnRefresh
            });

            return panel;
        }

        private Panel CreateStatisticsPanel()
        {
            Panel panel = new Panel
            {
                Location = new Point(890, 200),
                Size = new Size(280, 430),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblStatsTitle = new Label
            {
                Text = "📊 Statistics",
                Location = new Point(10, 10),
                Size = new Size(260, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            // Total Questions
            lblTotalQuestions = CreateStatLabel("Total Questions:", "0", new Point(10, 50));

            // Difficulty Stats
            Label lblDifficultyTitle = new Label
            {
                Text = "By Difficulty:",
                Location = new Point(10, 90),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            lblEasy = CreateStatLabel("Easy:", "0 (0%)", new Point(20, 115));
            lblAverage = CreateStatLabel("Average:", "0 (0%)", new Point(20, 140));
            lblDifficult = CreateStatLabel("Difficult:", "0 (0%)", new Point(20, 165));

            // Cognitive Stats
            Label lblCognitiveTitle = new Label
            {
                Text = "By Cognitive Level:",
                Location = new Point(10, 200),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            lblRemember = CreateStatLabel("Remember:", "0", new Point(20, 225));
            lblUnderstand = CreateStatLabel("Understand:", "0", new Point(20, 250));
            lblApply = CreateStatLabel("Apply:", "0", new Point(20, 275));
            lblAnalyze = CreateStatLabel("Analyze:", "0", new Point(20, 300));
            lblEvaluate = CreateStatLabel("Evaluate:", "0", new Point(20, 325));
            lblCreate = CreateStatLabel("Create:", "0", new Point(20, 350));

            lblTopics = CreateStatLabel("Unique Topics:", "0", new Point(10, 385));

            panel.Controls.AddRange(new Control[] {
                lblStatsTitle, lblTotalQuestions, lblDifficultyTitle,
                lblEasy, lblAverage, lblDifficult, lblCognitiveTitle,
                lblRemember, lblUnderstand, lblApply, lblAnalyze,
                lblEvaluate, lblCreate, lblTopics
            });

            return panel;
        }

        private Button CreateActionButton(string text, Point location, Color color)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(120, 35),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        private Label CreateStatLabel(string text, string value, Point location)
        {
            return new Label
            {
                Text = $"{text} {value}",
                Location = location,
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
        }

        #endregion

        #region Data Loading

        private void LoadQuestions()
        {
            try
            {
                var questions = _questionBankManager.GetAllQuestionBankQuestions();
                DisplayQuestions(questions);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading questions: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayQuestions(System.Collections.Generic.List<MultipleChoiceQuestion> questions)
        {
            dgvQuestions.DataSource = null;
            dgvQuestions.Rows.Clear();
            dgvQuestions.Columns.Clear();

            // Setup columns
            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QuestionId",
                HeaderText = "ID",
                Width = 50,
                Visible = false
            });

            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QuestionText",
                HeaderText = "Question",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Topic",
                HeaderText = "Topic",
                Width = 120
            });

            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Difficulty",
                HeaderText = "Difficulty",
                Width = 90
            });

            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cognitive",
                HeaderText = "Cognitive",
                Width = 100
            });

            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Points",
                HeaderText = "Points",
                Width = 60
            });

            dgvQuestions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Choices",
                HeaderText = "Choices",
                Width = 70
            });

            // Populate rows
            foreach (var q in questions)
            {
                string questionPreview = q.QuestionText.Length > 80
                    ? q.QuestionText.Substring(0, 77) + "..."
                    : q.QuestionText;

                dgvQuestions.Rows.Add(
                    q.QuestionId,
                    questionPreview,
                    q.Topic ?? "N/A",
                    q.DifficultyLevel.ToString(),
                    q.CognitiveLevel.ToString(),
                    q.PointValue,
                    q.Choices?.Count ?? 0
                );
            }

            lblResultCount.Text = $"Total: {questions.Count} questions";
            UpdateButtonStates();
        }

        private void LoadFilters()
        {
            // Topics
            cboTopic.Items.Clear();
            cboTopic.Items.Add("-- All Topics --");
            var topics = _questionBankManager.GetAllTopics();
            foreach (var topic in topics)
            {
                cboTopic.Items.Add(topic);
            }
            cboTopic.SelectedIndex = 0;

            // Difficulty
            cboDifficulty.Items.Clear();
            cboDifficulty.Items.Add("-- All Difficulties --");
            cboDifficulty.Items.AddRange(Enum.GetNames(typeof(DifficultyLevel)));
            cboDifficulty.SelectedIndex = 0;

            // Cognitive
            cboCognitive.Items.Clear();
            cboCognitive.Items.Add("-- All Cognitive Levels --");
            cboCognitive.Items.AddRange(Enum.GetNames(typeof(CognitiveLevel)));
            cboCognitive.SelectedIndex = 0;
        }

        private void LoadStatistics()
        {
            try
            {
                var stats = _questionBankManager.GetQuestionBankStatistics();

                lblTotalQuestions.Text = $"Total Questions: {stats.TotalQuestions}";
                lblEasy.Text = $"Easy: {stats.EasyQuestions} ({stats.EasyPercentage:F1}%)";
                lblAverage.Text = $"Average: {stats.AverageQuestions} ({stats.AveragePercentage:F1}%)";
                lblDifficult.Text = $"Difficult: {stats.DifficultQuestions} ({stats.DifficultPercentage:F1}%)";

                lblRemember.Text = $"Remember: {stats.RememberLevel}";
                lblUnderstand.Text = $"Understand: {stats.UnderstandLevel}";
                lblApply.Text = $"Apply: {stats.ApplyLevel}";
                lblAnalyze.Text = $"Analyze: {stats.AnalyzeLevel}";
                lblEvaluate.Text = $"Evaluate: {stats.EvaluateLevel}";
                lblCreate.Text = $"Create: {stats.CreateLevel}";

                lblTopics.Text = $"Unique Topics: {stats.TopicsCount}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Event Handlers - Search TextBox Placeholder

        // FIX #1: Implement placeholder text manually
        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "🔍 Search questions...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void TxtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "🔍 Search questions...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Don't search if it's the placeholder text
            if (txtSearch.Text == "🔍 Search questions...")
                return;

            ApplyFilters();
        }

        private void CboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "🔍 Search questions...";
            txtSearch.ForeColor = Color.Gray;
            cboTopic.SelectedIndex = 0;
            cboDifficulty.SelectedIndex = 0;
            cboCognitive.SelectedIndex = 0;
            LoadQuestions();
        }

        private void ApplyFilters()
        {
            string searchText = (txtSearch.Text == "🔍 Search questions..." || string.IsNullOrWhiteSpace(txtSearch.Text))
                ? null
                : txtSearch.Text.Trim();

            string topic = cboTopic.SelectedIndex > 0 ? cboTopic.SelectedItem.ToString() : null;

            // FIX #2 & #3: Use Enum.TryParse instead of direct casting
            DifficultyLevel? difficulty = null;
            if (cboDifficulty.SelectedIndex > 0)
            {
                DifficultyLevel diffLevel;
                if (Enum.TryParse(cboDifficulty.SelectedItem.ToString(), out diffLevel))
                {
                    difficulty = diffLevel;
                }
            }

            CognitiveLevel? cognitive = null;
            if (cboCognitive.SelectedIndex > 0)
            {
                CognitiveLevel cogLevel;
                if (Enum.TryParse(cboCognitive.SelectedItem.ToString(), out cogLevel))
                {
                    cognitive = cogLevel;
                }
            }

            var filtered = _questionBankManager.SearchQuestions(searchText, topic, difficulty, cognitive);
            DisplayQuestions(filtered);
        }

        private void DgvQuestions_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvQuestions.SelectedRows.Count > 0)
            {
                int questionId = Convert.ToInt32(dgvQuestions.SelectedRows[0].Cells["QuestionId"].Value);
                _selectedQuestion = _questionBankManager.GetQuestionById(questionId);
            }
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = dgvQuestions.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDuplicate.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
            btnView.Enabled = hasSelection;
        }

        // FIX #4 & #5: Update method signatures to match your AddQuestionForm
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Adjust this based on your actual AddQuestionForm constructor
            var addForm = new AddQuestionForm(); // Remove the isQuestionBank parameter if it doesn't exist
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadQuestions();
                LoadFilters();
                LoadStatistics();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedQuestion == null) return;

            // Adjust this based on your actual AddQuestionForm constructor
            var editForm = new AddQuestionForm(); // Pass your question differently if needed
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadQuestions();
                LoadStatistics();
            }
        }

        private void BtnDuplicate_Click(object sender, EventArgs e)
        {
            if (_selectedQuestion == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to duplicate this question?\n\n{_selectedQuestion.QuestionText}",
                "Confirm Duplicate",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                int newId = _questionBankManager.DuplicateQuestion(_selectedQuestion.QuestionId, out string error);
                if (newId > 0)
                {
                    MessageBox.Show("Question duplicated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadQuestions();
                    LoadStatistics();
                }
                else
                {
                    MessageBox.Show($"Failed to duplicate question: {error}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedQuestion == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete this question?\n\n{_selectedQuestion.QuestionText}\n\nThis action cannot be undone!",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool success = _questionBankManager.DeleteQuestion(_selectedQuestion.QuestionId, out string error);
                if (success)
                {
                    MessageBox.Show("Question deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadQuestions();
                    LoadFilters();
                    LoadStatistics();
                }
                else
                {
                    MessageBox.Show($"Failed to delete question: {error}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            if (_selectedQuestion == null) return;

            var viewForm = new ViewQuestionDetailsForm(_selectedQuestion);
            viewForm.ShowDialog();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadQuestions();
            LoadFilters();
            LoadStatistics();
            MessageBox.Show("Question bank refreshed!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Form Controls Declaration

        private TextBox txtSearch;
        private ComboBox cboTopic;
        private ComboBox cboDifficulty;
        private ComboBox cboCognitive;
        private DataGridView dgvQuestions;
        private Button btnAdd, btnEdit, btnDuplicate, btnDelete, btnView, btnRefresh;
        private Label lblResultCount;
        private Label lblTotalQuestions, lblEasy, lblAverage, lblDifficult;
        private Label lblRemember, lblUnderstand, lblApply, lblAnalyze, lblEvaluate, lblCreate, lblTopics;

        #endregion
    }
}