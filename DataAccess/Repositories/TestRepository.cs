using MySql.Data.MySqlClient;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Enums;
using StudentAssessmentSystem.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Purpose: Handles all MySQL database operations for Tests
// Connected to: Test model, TestManager
namespace StudentAssessmentSystem.DataAccess.Repositories
{
    // Repository for Test database operations
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

        
        /// Gets test by ID    
        public Test GetById(int id)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT * FROM Tests WHERE TestId = @TestId";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TestId", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapReaderToTest(reader);
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting test: {ex.Message}", ex);
            }
        }

        public Test GetTestByInstanceId(int testInstanceId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // First, get the TestId from TestInstances table
                    string query = @"SELECT t.* 
                                   FROM Tests t
                                   INNER JOIN TestInstances ti ON t.TestId = ti.TestId
                                   WHERE ti.InstanceId = @InstanceId";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InstanceId", testInstanceId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Test test = MapReaderToTest(reader);

                                // Close reader before loading questions
                                reader.Close();

                                // Load questions for this test
                                var questionRepo = new QuestionRepository();
                                test.Questions = questionRepo.GetQuestionsByTest(test.TestId).Cast<Question>().ToList();

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
    
        /// Gets all active tests
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

        
        /// Gets all tests created by a specific teacher  
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

        
        /// Updates test information
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

     
        /// Soft delete - sets IsActive to false
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

        
        /// Maps MySQL reader to Test object 
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
    }

    // ============================================
    // QUESTION REPOSITORY
    // ============================================

   
    /// Repository for Question database operations 
    public class QuestionRepository
    {
       
        /// Adds a new question with its choices     
        public int AddQuestion(MultipleChoiceQuestion question)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Insert question
                    string questionQuery = @"INSERT INTO Questions 
                                           (TestId, QuestionText, QuestionType, PointValue, 
                                            DifficultyLevel, CognitiveLevel, Topic, Explanation, OrderNumber)
                                           VALUES 
                                           (@TestId, @QuestionText, @QuestionType, @PointValue,
                                            @DifficultyLevel, @CognitiveLevel, @Topic, @Explanation, @OrderNumber);
                                           SELECT LAST_INSERT_ID();";

                    int questionId;
                    using (var cmd = new MySqlCommand(questionQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@TestId", question.TestId);
                        cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                        cmd.Parameters.AddWithValue("@QuestionType", question.QuestionType);
                        cmd.Parameters.AddWithValue("@PointValue", question.PointValue);
                        cmd.Parameters.AddWithValue("@DifficultyLevel", question.DifficultyLevel.ToString());
                        cmd.Parameters.AddWithValue("@CognitiveLevel", question.CognitiveLevel.ToString());
                        cmd.Parameters.AddWithValue("@Topic", question.Topic ?? "");
                        cmd.Parameters.AddWithValue("@Explanation", question.Explanation ?? "");
                        cmd.Parameters.AddWithValue("@OrderNumber", question.OrderNumber);

                        questionId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Insert choices
                    if (question.Choices != null && question.Choices.Count > 0)
                    {
                        string choiceQuery = @"INSERT INTO QuestionChoices 
                                             (QuestionId, ChoiceText, IsCorrect, OrderNumber)
                                             VALUES 
                                             (@QuestionId, @ChoiceText, @IsCorrect, @OrderNumber)";

                        foreach (var choice in question.Choices)
                        {
                            using (var cmd = new MySqlCommand(choiceQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@QuestionId", questionId);
                                cmd.Parameters.AddWithValue("@ChoiceText", choice.ChoiceText);
                                cmd.Parameters.AddWithValue("@IsCorrect", choice.IsCorrect);
                                cmd.Parameters.AddWithValue("@OrderNumber", choice.OrderNumber);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    return questionId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding question: {ex.Message}", ex);
            }
        }

        /// Gets all questions for a specific test
        public List<MultipleChoiceQuestion> GetQuestionsByTest(int testId)
        {
            List<MultipleChoiceQuestion> questions = new List<MultipleChoiceQuestion>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Get questions
                    string query = "SELECT * FROM Questions WHERE TestId = @TestId ORDER BY OrderNumber";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TestId", testId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var question = new MultipleChoiceQuestion
                                {
                                    QuestionId = reader.GetInt32("QuestionId"),
                                    TestId = reader.IsDBNull(reader.GetOrdinal("TestId")) ? (int?)null : reader.GetInt32("TestId"),
                                    QuestionText = reader.GetString("QuestionText"),
                                    QuestionType = reader.GetString("QuestionType"),
                                    PointValue = reader.GetInt32("PointValue"),
                                    DifficultyLevel = (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), reader.GetString("DifficultyLevel")),
                                    CognitiveLevel = (CognitiveLevel)Enum.Parse(typeof(CognitiveLevel), reader.GetString("CognitiveLevel")),
                                    Topic = reader.IsDBNull(reader.GetOrdinal("Topic")) ? "" : reader.GetString("Topic"),
                                    Explanation = reader.IsDBNull(reader.GetOrdinal("Explanation")) ? "" : reader.GetString("Explanation"),
                                    OrderNumber = reader.GetInt32("OrderNumber")
                                };

                                questions.Add(question);
                            }
                        }
                    }

                    // Get choices for each question
                    foreach (var question in questions)
                    {
                        question.Choices = GetChoicesByQuestion(conn, question.QuestionId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting questions: {ex.Message}", ex);
            }

            return questions;
        }

        /// Gets all choices for a specific question  
        private List<QuestionChoice> GetChoicesByQuestion(MySqlConnection conn, int questionId)
        {
            List<QuestionChoice> choices = new List<QuestionChoice>();

            string query = "SELECT * FROM QuestionChoices WHERE QuestionId = @QuestionId ORDER BY OrderNumber";
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@QuestionId", questionId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        choices.Add(new QuestionChoice
                        {
                            ChoiceId = reader.GetInt32("ChoiceId"),
                            QuestionId = reader.GetInt32("QuestionId"),
                            ChoiceText = reader.GetString("ChoiceText"),
                            IsCorrect = reader.GetBoolean("IsCorrect"),
                            OrderNumber = reader.GetInt32("OrderNumber")
                        });
                    }
                }
            }

            return choices;
        }
    }

    // ============================================
    // TEST RESULT REPOSITORY
    // ============================================

    /// Repository for TestResult database operations - MySQL version

    public class TestResultRepository
    {
  
        /// Adds a new test result
        public int Add(TestResult result)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO TestResults 
                                   (InstanceId, StudentId, StartTime, SubmitTime, RawScore, 
                                    TotalPoints, Percentage, LetterGrade, Passed, IsCompleted)
                                   VALUES 
                                   (@InstanceId, @StudentId, @StartTime, @SubmitTime, @RawScore,
                                    @TotalPoints, @Percentage, @LetterGrade, @Passed, @IsCompleted);
                                   SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InstanceId", result.InstanceId);
                        cmd.Parameters.AddWithValue("@StudentId", result.StudentId);
                        cmd.Parameters.AddWithValue("@StartTime", result.StartTime);
                        cmd.Parameters.AddWithValue("@SubmitTime", result.SubmitTime.HasValue ? (object)result.SubmitTime.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@RawScore", result.RawScore);
                        cmd.Parameters.AddWithValue("@TotalPoints", result.TotalPoints);
                        cmd.Parameters.AddWithValue("@Percentage", result.Percentage);
                        cmd.Parameters.AddWithValue("@LetterGrade", result.LetterGrade ?? "");
                        cmd.Parameters.AddWithValue("@Passed", result.Passed);
                        cmd.Parameters.AddWithValue("@IsCompleted", result.IsCompleted);

                        int resultId = Convert.ToInt32(cmd.ExecuteScalar());
                        return resultId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding test result: {ex.Message}", ex);
            }
        }

        
        /// Gets test results by student
        public List<TestResult> GetResultsByStudent(int studentId)
        {
            List<TestResult> results = new List<TestResult>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT * FROM TestResults 
                                   WHERE StudentId = @StudentId 
                                   ORDER BY StartTime DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(MapReaderToTestResult(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting test results: {ex.Message}", ex);
            }

            return results;
        }

        public List<TestResult> GetResultsByInstance(int testInstanceId)
        {
            List<TestResult> results = new List<TestResult>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT * FROM TestResults 
                                   WHERE InstanceId = @InstanceId 
                                   AND IsCompleted = TRUE
                                   ORDER BY RawScore DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InstanceId", testInstanceId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(MapReaderToTestResult(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting results by instance: {ex.Message}", ex);
            }

            return results;
        }

        /// Maps reader to TestResult
        private TestResult MapReaderToTestResult(MySqlDataReader reader)
        {
            return new TestResult
            {
                ResultId = reader.GetInt32("ResultId"),
                InstanceId = reader.GetInt32("InstanceId"),
                StudentId = reader.GetInt32("StudentId"),
                StartTime = reader.GetDateTime("StartTime"),
                SubmitTime = reader.IsDBNull(reader.GetOrdinal("SubmitTime")) ? (DateTime?)null : reader.GetDateTime("SubmitTime"),
                RawScore = reader.GetInt32("RawScore"),
                TotalPoints = reader.GetInt32("TotalPoints"),
                Percentage = reader.GetDecimal("Percentage"),
                LetterGrade = reader.IsDBNull(reader.GetOrdinal("LetterGrade")) ? "" : reader.GetString("LetterGrade"),
                Passed = reader.GetBoolean("Passed"),
                IsCompleted = reader.GetBoolean("IsCompleted")
            };
        } /// Adds a new test to MySQL database
    }
}
