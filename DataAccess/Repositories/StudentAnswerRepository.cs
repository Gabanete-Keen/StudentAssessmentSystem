using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentAssessmentSystem.Models.Results;
using MySql.Data.MySqlClient;

namespace StudentAssessmentSystem.DataAccess.Repositories
{
    /// <summary>
    /// Repository for StudentAnswer database operations
    /// Purpose: Gets student answers from MySQL database
    /// Connected to: StudentAnswer model, ItemAnalyzer
    /// </summary>
    public class StudentAnswerRepository
    {
        /// <summary>
        /// Gets all answers for a specific question in a test instance
        /// Used for item analysis
        /// </summary>
        public List<StudentAnswer> GetAnswersForQuestion(int questionId, int testInstanceId)
        {
            List<StudentAnswer> answers = new List<StudentAnswer>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT sa.* 
                        FROM StudentAnswers sa
                        INNER JOIN TestResults tr ON sa.ResultId = tr.ResultId
                        WHERE sa.QuestionId = @QuestionId 
                        AND tr.InstanceId = @InstanceId
                        AND tr.IsCompleted = TRUE";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuestionId", questionId);
                        cmd.Parameters.AddWithValue("@InstanceId", testInstanceId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                answers.Add(new StudentAnswer
                                {
                                    AnswerId = reader.GetInt32("AnswerId"),
                                    ResultId = reader.GetInt32("ResultId"),
                                    QuestionId = reader.GetInt32("QuestionId"),
                                    SelectedChoiceId = reader.IsDBNull(reader.GetOrdinal("SelectedChoiceId"))
                                        ? (int?)null
                                        : reader.GetInt32("SelectedChoiceId"),
                                    IsCorrect = reader.GetBoolean("IsCorrect"),
                                    PointsEarned = reader.GetInt32("PointsEarned")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting answers: {ex.Message}", ex);
            }

            return answers;
        }

        /// <summary>
        /// Adds a student answer to database
        /// </summary>
        public int Add(StudentAnswer answer)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"
                INSERT INTO StudentAnswers 
                (InstanceId, ResultId, QuestionId, SelectedChoiceId, AnswerText, IsCorrect, PointsEarned, TimeSpentSeconds)
                VALUES 
                (@InstanceId, @ResultId, @QuestionId, @SelectedChoiceId, @AnswerText, @IsCorrect, @PointsEarned, @TimeSpentSeconds);
                SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InstanceId", answer.InstanceId);  // NEW: Critical FK
                        cmd.Parameters.AddWithValue("@ResultId", answer.ResultId);
                        cmd.Parameters.AddWithValue("@QuestionId", answer.QuestionId);
                        cmd.Parameters.AddWithValue("@SelectedChoiceId",
                            answer.SelectedChoiceId.HasValue ? (object)answer.SelectedChoiceId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@AnswerText", answer.AnswerText ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsCorrect", answer.IsCorrect);
                        cmd.Parameters.AddWithValue("@PointsEarned", answer.PointsEarned);
                        cmd.Parameters.AddWithValue("@TimeSpentSeconds", answer.TimeSpentSeconds ?? (object)0);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding answer: {ex.Message}", ex);
            }
        }




        /// <summary>
        /// ✅ NEW METHOD: Gets all answers for a specific test result
        /// Used when submitting test to calculate total score
        /// </summary>
        public List<StudentAnswer> GetAnswersByResult(int resultId)
        {
            List<StudentAnswer> answers = new List<StudentAnswer>();

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT * FROM StudentAnswers WHERE ResultId = @ResultId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ResultId", resultId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                answers.Add(new StudentAnswer
                                {
                                    AnswerId = reader.GetInt32("AnswerId"),
                                    ResultId = reader.GetInt32("ResultId"),
                                    QuestionId = reader.GetInt32("QuestionId"),
                                    SelectedChoiceId = reader.IsDBNull(reader.GetOrdinal("SelectedChoiceId"))
                                        ? (int?)null
                                        : reader.GetInt32("SelectedChoiceId"),
                                    AnswerText = reader.IsDBNull(reader.GetOrdinal("AnswerText"))
                                        ? null
                                        : reader.GetString("AnswerText"),
                                    IsCorrect = reader.GetBoolean("IsCorrect"),
                                    PointsEarned = reader.GetInt32("PointsEarned"),
                                    TimeSpentSeconds = reader.GetInt32("TimeSpentSeconds")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting student answers: {ex.Message}", ex);
            }

            return answers;
        }
    }
}
