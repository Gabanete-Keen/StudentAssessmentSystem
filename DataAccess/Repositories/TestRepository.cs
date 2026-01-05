using MySql.Data.MySqlClient;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.DTOs;
using StudentAssessmentSystem.Models.Enums;
using StudentAssessmentSystem.Models.Results;
using StudentAssessmentSystem.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAssessmentSystem.DataAccess.Repositories
{
    public class TestRepository : IRepository<Test>
    {

        public int Add(Test entity)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO Tests 
                                   (SubjectId, TeacherId, TestTitle, Description, TestType, TotalPoints, 
                                    PassingScore, DurationMinutes, Instructions, RandomizeQuestions, 
                                    RandomizeChoices, ShowCorrectAnswers, AllowReview, CreatedDate, IsActive)
                                   VALUES 
                                   (@SubjectId, @TeacherId, @TestTitle, @Description, @TestType, @TotalPoints,
                                    @PassingScore, @DurationMinutes, @Instructions, @RandomizeQuestions,
                                    @RandomizeChoices, @ShowCorrectAnswers, @AllowReview, @CreatedDate, @IsActive);
                                   SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SubjectId", entity.SubjectId);
                        cmd.Parameters.AddWithValue("@TeacherId", entity.TeacherId);
                        cmd.Parameters.AddWithValue("@TestTitle", entity.TestTitle);
                        cmd.Parameters.AddWithValue("@Description", entity.Description ?? "");
                        cmd.Parameters.AddWithValue("@TestType", entity.TestType.ToString());
                        cmd.Parameters.AddWithValue("@TotalPoints", entity.TotalPoints);
                        cmd.Parameters.AddWithValue("@PassingScore", entity.PassingScore);
                        cmd.Parameters.AddWithValue("@DurationMinutes", entity.DurationMinutes);
                        cmd.Parameters.AddWithValue("@Instructions", entity.Instructions ?? "");
                        cmd.Parameters.AddWithValue("@RandomizeQuestions", entity.RandomizeQuestions);
                        cmd.Parameters.AddWithValue("@RandomizeChoices", entity.RandomizeChoices);
                        cmd.Parameters.AddWithValue("@ShowCorrectAnswers", entity.ShowCorrectAnswers);
                        cmd.Parameters.AddWithValue("@AllowReview", entity.AllowReview);
                        cmd.Parameters.AddWithValue("@CreatedDate", entity.CreatedDate);
                        cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);

                        int testId = Convert.ToInt32(cmd.ExecuteScalar());
                        return testId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding test: {ex.Message}", ex);
            }
        }
     
        public Test GetById(int id)
        {
            return GetTestById(id);
        }

        public Test GetTestById(int testId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // ✅ JOIN with Subjects table to get SubjectName
                    string query = @"SELECT t.*, s.SubjectName 
                           FROM Tests t
                           LEFT JOIN Subjects s ON t.SubjectId = s.SubjectId
                           WHERE t.TestId = @TestId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TestId", testId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Test test = new Test
                                {
                                    TestId = reader.GetInt32("TestId"),
                                    SubjectId = reader.GetInt32("SubjectId"),
                                    TeacherId = reader.GetInt32("TeacherId"),
                                    TestTitle = reader.GetString("TestTitle"),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                        ? null
                                        : reader.GetString("Description"),
                                    TestType = (TestType)reader.GetInt32("TestType"),
                                    TotalPoints = reader.GetInt32("TotalPoints"),
                                    PassingScore = reader.GetDecimal("PassingScore"),
                                    DurationMinutes = reader.GetInt32("DurationMinutes"),
                                    Instructions = reader.IsDBNull(reader.GetOrdinal("Instructions"))
                                        ? null
                                        : reader.GetString("Instructions"),
                                    RandomizeQuestions = reader.GetBoolean("RandomizeQuestions"),
                                    RandomizeChoices = reader.GetBoolean("RandomizeChoices"),
                                    ShowCorrectAnswers = reader.GetBoolean("ShowCorrectAnswers"),
                                    AllowReview = reader.GetBoolean("AllowReview"),
                                    CreatedDate = reader.GetDateTime("CreatedDate"),
                                    IsActive = reader.GetBoolean("IsActive"),

                                    // ✅ Populate SubjectName from JOIN
                                    SubjectName = reader.IsDBNull(reader.GetOrdinal("SubjectName"))
                                        ? "N/A"
                                        : reader.GetString("SubjectName")
                                };

                                // ✅ Load questions using QuestionRepository
                                QuestionRepository questionRepo = new QuestionRepository();
                                var mcQuestions = questionRepo.GetQuestionsByTest(testId);

                                // Convert List<MultipleChoiceQuestion> to List<Question>
                                test.Questions = new List<Question>();
                                foreach (var mcq in mcQuestions)
                                {
                                    test.Questions.Add(mcq);
                                }

                                return test;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting test by ID: {ex.Message}", ex);
            }

            return null;
        }




        public Test GetTestByInstanceId(int testInstanceId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT t.* 
                           FROM Tests t
                           INNER JOIN TestInstances ti ON t.TestId = ti.TestId
                           WHERE ti.InstanceId = @InstanceId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InstanceId", testInstanceId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Test test = MapReaderToTest(reader);
                                return test;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting test by instance: {ex.Message}", ex);
            }
        }


        public List<Test> GetAll()
        {
            List<Test> tests = new List<Test>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT * FROM Tests WHERE IsActive = TRUE ORDER BY CreatedDate DESC";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tests.Add(MapReaderToTest(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting tests: {ex.Message}", ex);
            }

            return tests;
        }

        public List<Test> GetTestsByTeacher(int teacherId)
        {
            List<Test> tests = new List<Test>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT * FROM Tests 
                                   WHERE TeacherId = @TeacherId AND IsActive = TRUE 
                                   ORDER BY CreatedDate DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TeacherId", teacherId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tests.Add(MapReaderToTest(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting tests by teacher: {ex.Message}", ex);
            }

            return tests;
        }

        public bool Update(Test entity)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE Tests SET 
                                   TestTitle = @TestTitle,
                                   Description = @Description,
                                   TestType = @TestType,
                                   TotalPoints = @TotalPoints,
                                   PassingScore = @PassingScore,
                                   DurationMinutes = @DurationMinutes,
                                   Instructions = @Instructions,
                                   RandomizeQuestions = @RandomizeQuestions,
                                   RandomizeChoices = @RandomizeChoices,
                                   ShowCorrectAnswers = @ShowCorrectAnswers,
                                   AllowReview = @AllowReview
                                   WHERE TestId = @TestId";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TestId", entity.TestId);
                        cmd.Parameters.AddWithValue("@TestTitle", entity.TestTitle);
                        cmd.Parameters.AddWithValue("@Description", entity.Description ?? "");
                        cmd.Parameters.AddWithValue("@TestType", entity.TestType.ToString());
                        cmd.Parameters.AddWithValue("@TotalPoints", entity.TotalPoints);
                        cmd.Parameters.AddWithValue("@PassingScore", entity.PassingScore);
                        cmd.Parameters.AddWithValue("@DurationMinutes", entity.DurationMinutes);
                        cmd.Parameters.AddWithValue("@Instructions", entity.Instructions ?? "");
                        cmd.Parameters.AddWithValue("@RandomizeQuestions", entity.RandomizeQuestions);
                        cmd.Parameters.AddWithValue("@RandomizeChoices", entity.RandomizeChoices);
                        cmd.Parameters.AddWithValue("@ShowCorrectAnswers", entity.ShowCorrectAnswers);
                        cmd.Parameters.AddWithValue("@AllowReview", entity.AllowReview);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating test: {ex.Message}", ex);
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = "UPDATE Tests SET IsActive = FALSE WHERE TestId = @TestId";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TestId", id);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting test: {ex.Message}", ex);
            }
        }

        private Test MapReaderToTest(MySqlDataReader reader)
        {
            return new Test
            {
                TestId = reader.GetInt32("TestId"),
                SubjectId = reader.GetInt32("SubjectId"),
                TeacherId = reader.GetInt32("TeacherId"),
                TestTitle = reader.GetString("TestTitle"),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                TestType = (TestType)Enum.Parse(typeof(TestType), reader.GetString("TestType")),
                TotalPoints = reader.GetInt32("TotalPoints"),
                PassingScore = reader.GetDecimal("PassingScore"),
                DurationMinutes = reader.GetInt32("DurationMinutes"),
                Instructions = reader.IsDBNull(reader.GetOrdinal("Instructions")) ? "" : reader.GetString("Instructions"),
                RandomizeQuestions = reader.GetBoolean("RandomizeQuestions"),
                RandomizeChoices = reader.GetBoolean("RandomizeChoices"),
                ShowCorrectAnswers = reader.GetBoolean("ShowCorrectAnswers"),
                AllowReview = reader.GetBoolean("AllowReview"),
                CreatedDate = reader.GetDateTime("CreatedDate"),
                IsActive = reader.GetBoolean("IsActive")
            };
        }

        // Gets available test instances for students
        public List<AvailableTestInstanceDTO> GetAvailableTestInstancesForStudents()
        {
            var instances = new List<AvailableTestInstanceDTO>();

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            ti.InstanceId,
                            ti.TestId,
                            ti.TeacherId,
                            ti.InstanceTitle,
                            ti.StartDate,
                            ti.EndDate,
                            ti.IsActive,
                            ti.CreatedDate,
                            t.TestTitle,
                            t.Description,
                            t.DurationMinutes,
                            t.TotalPoints,
                            t.PassingScore,
                            t.Instructions,
                            t.RandomizeQuestions,
                            t.RandomizeChoices,
                            s.SubjectName,
                            CONCAT(u.FirstName, ' ', u.LastName) AS TeacherName
                        FROM TestInstances ti
                        INNER JOIN Tests t ON ti.TestId = t.TestId
                        INNER JOIN Subjects s ON t.SubjectId = s.SubjectId
                        INNER JOIN Users u ON ti.TeacherId = u.UserId
                        WHERE ti.IsActive = 1
                          AND NOW() >= ti.StartDate
                          AND NOW() <= ti.EndDate
                        ORDER BY ti.StartDate ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dto = new AvailableTestInstanceDTO
                                {
                                    InstanceId = reader.GetInt32("InstanceId"),
                                    TestId = reader.GetInt32("TestId"),
                                    TeacherId = reader.GetInt32("TeacherId"),
                                    InstanceTitle = reader.GetString("InstanceTitle"),
                                    StartDate = reader.GetDateTime("StartDate"),
                                    EndDate = reader.GetDateTime("EndDate"),
                                    IsActive = reader.GetBoolean("IsActive"),
                                    CreatedDate = reader.GetDateTime("CreatedDate"),
                                    TestTitle = reader.GetString("TestTitle"),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                        ? "" : reader.GetString("Description"),
                                    DurationMinutes = reader.GetInt32("DurationMinutes"),
                                    TotalPoints = reader.GetInt32("TotalPoints"),
                                    PassingScore = reader.GetDecimal("PassingScore"),
                                    Instructions = reader.IsDBNull(reader.GetOrdinal("Instructions"))
                                        ? "" : reader.GetString("Instructions"),
                                    RandomizeQuestions = reader.GetBoolean("RandomizeQuestions"),
                                    RandomizeChoices = reader.GetBoolean("RandomizeChoices"),
                                    SubjectName = reader.GetString("SubjectName"),
                                    TeacherName = reader.GetString("TeacherName")
                                };

                                instances.Add(dto);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting available test instances: {ex.Message}");
            }

            return instances;
        }

        //  Gets complete test session for student to take
        public TestSessionDTO GetTestSessionForStudent(int instanceId)
        {
            TestSessionDTO session = null;

            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();

                    // Get test instance details
                    string query = @"
                SELECT 
                    ti.InstanceId,
                    ti.TestId,
                    ti.InstanceTitle,
                    t.TestTitle,
                    s.SubjectName,
                    CONCAT(u.FirstName, ' ', u.LastName) as TeacherName,
                    ti.StartDate,
                    ti.EndDate,
                    t.DurationMinutes,
                    t.TotalPoints,
                    (SELECT COUNT(*) FROM TestQuestions WHERE TestId = t.TestId) as TotalQuestions
                FROM TestInstances ti
                INNER JOIN Tests t ON ti.TestId = t.TestId
                INNER JOIN Subjects s ON t.SubjectId = s.SubjectId
                INNER JOIN Users u ON ti.TeacherId = u.UserId
                WHERE ti.InstanceId = @InstanceId
                  AND ti.IsActive = 1
                  AND NOW() >= ti.StartDate
                  AND NOW() <= ti.EndDate";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InstanceId", instanceId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                session = new TestSessionDTO
                                {
                                    InstanceId = reader.GetInt32("InstanceId"),
                                    TestId = reader.GetInt32("TestId"),
                                    InstanceTitle = reader.GetString("InstanceTitle"),
                                    TestTitle = reader.GetString("TestTitle"),
                                    SubjectName = reader.GetString("SubjectName"),
                                    TeacherName = reader.GetString("TeacherName"),
                                    StartDate = reader.GetDateTime("StartDate"),
                                    EndDate = reader.GetDateTime("EndDate"),
                                    DurationMinutes = reader.GetInt32("DurationMinutes"),
                                    TotalPoints = reader.GetInt32("TotalPoints"),
                                    TotalQuestions = reader.GetInt32("TotalQuestions")
                                };
                            }
                        }
                    }

                    if (session == null)
                    {
                        throw new Exception("Test session not found or no longer available.");
                    }

                    // Get all questions for this test
                    string questionQuery = @"
                SELECT 
                    q.QuestionId,
                    q.QuestionText,
                    q.Points,
                    tq.OrderNumber,
                    q.DifficultyLevel,
                    q.CognitiveLevel
                FROM TestQuestions tq
                INNER JOIN Questions q ON tq.QuestionId = q.QuestionId
                WHERE tq.TestId = @TestId
                ORDER BY tq.OrderNumber";

                    using (MySqlCommand command = new MySqlCommand(questionQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TestId", session.TestId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TestQuestionDTO question = new TestQuestionDTO
                                {
                                    QuestionId = reader.GetInt32("QuestionId"),
                                    QuestionText = reader.GetString("QuestionText"),
                                    Points = reader.GetInt32("Points"),
                                    OrderNumber = reader.GetInt32("OrderNumber"),
                                    DifficultyLevel = reader.GetString("DifficultyLevel"),
                                    CognitiveLevel = reader.GetString("CognitiveLevel")
                                };

                                session.Questions.Add(question);
                            }
                        }
                    }

                    // Get choices for each question
                    foreach (TestQuestionDTO question in session.Questions)
                    {
                        string choiceQuery = @"
                    SELECT 
                        ChoiceId,
                        ChoiceText,
                        ChoiceOrder
                    FROM QuestionChoices
                    WHERE QuestionId = @QuestionId
                    ORDER BY ChoiceOrder";

                        using (MySqlCommand command = new MySqlCommand(choiceQuery, connection))
                        {
                            command.Parameters.AddWithValue("@QuestionId", question.QuestionId);

                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    QuestionChoiceDTO choice = new QuestionChoiceDTO
                                    {
                                        ChoiceId = reader.GetInt32("ChoiceId"),
                                        ChoiceText = reader.GetString("ChoiceText"),
                                        ChoiceOrder = reader.GetInt32("ChoiceOrder")
                                    };

                                    question.Choices.Add(choice);
                                }
                            }
                        }
                    }

                    connection.Close();
                }

                return session;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading test session: {ex.Message}", ex);
            }
        }
        /// Gets all active test instances (tests that are currently available)
        public List<TestInstance> GetActiveTestInstances()
        {
            List<TestInstance> instances = new List<TestInstance>();

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT * FROM TestInstances 
                           WHERE IsActive = 1 
                           AND StartDate <= @Now 
                           AND EndDate >= @Now
                           ORDER BY StartDate DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Now", DateTime.Now);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                instances.Add(new TestInstance
                                {
                                    InstanceId = reader.GetInt32("InstanceId"),
                                    TestId = reader.GetInt32("TestId"),
                                    StartDate = reader.GetDateTime("StartDate"),
                                    EndDate = reader.GetDateTime("EndDate"),
                                    IsActive = reader.GetBoolean("IsActive")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting active test instances: {ex.Message}", ex);
            }

            return instances;
        }
    }
}
