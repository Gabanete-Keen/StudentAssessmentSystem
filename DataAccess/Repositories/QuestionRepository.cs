using MySql.Data.MySqlClient;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Enums;
using System;
using System.Collections.Generic;

namespace StudentAssessmentSystem.DataAccess.Repositories
{
    /// Repository for Question database operations
    public class QuestionRepository
    {
        /// Adds a new question to the question bank
        public int AddQuestion(MultipleChoiceQuestion question)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    //  Insert question (WITHOUT OrderNumber and IsActive if they don't exist)
                    string query = @"INSERT INTO Questions 
                           (TestId, QuestionText, QuestionType, PointValue, DifficultyLevel, CognitiveLevel, Topic, Explanation)
                           VALUES 
                           (@TestId, @QuestionText, @QuestionType, @PointValue, @DifficultyLevel, @CognitiveLevel, @Topic, @Explanation);
                           SELECT LAST_INSERT_ID();";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TestId", question.TestId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                        cmd.Parameters.AddWithValue("@QuestionType", question.QuestionType ?? "MultipleChoice");
                        cmd.Parameters.AddWithValue("@PointValue", question.PointValue);
                        cmd.Parameters.AddWithValue("@DifficultyLevel", question.DifficultyLevel.ToString());
                        cmd.Parameters.AddWithValue("@CognitiveLevel", question.CognitiveLevel.ToString());
                        cmd.Parameters.AddWithValue("@Topic", question.Topic ?? "");
                        cmd.Parameters.AddWithValue("@Explanation", question.Explanation ?? "");

                        int questionId = Convert.ToInt32(cmd.ExecuteScalar());

                        // Insert choices
                        if (question.Choices != null && question.Choices.Count > 0)
                        {
                            foreach (var choice in question.Choices)
                            {
                                // ✅ FIXED: Use OrderNumber consistently
                                string choiceQuery = @"INSERT INTO QuestionChoices 
                                             (QuestionId, ChoiceText, IsCorrect, OrderNumber)
                                             VALUES 
                                             (@QuestionId, @ChoiceText, @IsCorrect, @OrderNumber)";

                                using (MySqlCommand choiceCmd = new MySqlCommand(choiceQuery, conn))
                                {
                                    choiceCmd.Parameters.AddWithValue("@QuestionId", questionId);
                                    choiceCmd.Parameters.AddWithValue("@ChoiceText", choice.ChoiceText);
                                    choiceCmd.Parameters.AddWithValue("@IsCorrect", choice.IsCorrect);
                                    choiceCmd.Parameters.AddWithValue("@OrderNumber", choice.OrderNumber);
                                    choiceCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        return questionId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding question: {ex.Message}", ex);
            }
        }

        /// Get questions by test
        public List<MultipleChoiceQuestion> GetQuestionsByTest(int testId)
        {
            List<MultipleChoiceQuestion> questions = new List<MultipleChoiceQuestion>();

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // ✅ SIMPLER: Query directly from Questions table using TestId
                    string query = @"SELECT * FROM Questions 
                           WHERE TestId = @TestId 
                           ORDER BY QuestionId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TestId", testId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var question = MapReaderToQuestion(reader);
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
                throw new Exception($"Error getting questions by test: {ex.Message}", ex);
            }

            return questions;
        }

        // ADD THIS METHOD TO YOUR QuestionRepository.cs class

        /// <summary>
        /// Gets all questions from the question bank (where TestId IS NULL)
        /// These are reusable questions not assigned to any specific test
        /// </summary>
        public List<MultipleChoiceQuestion> GetQuestionBankQuestions()
        {
            List<MultipleChoiceQuestion> questions = new List<MultipleChoiceQuestion>();

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Get questions where TestId is NULL (question bank only)
                    string query = @"SELECT * FROM Questions 
                           WHERE TestId IS NULL 
                           ORDER BY Topic, DifficultyLevel, QuestionId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var question = MapReaderToQuestion(reader);
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
                throw new Exception($"Error getting question bank questions: {ex.Message}", ex);
            }

            return questions;
        }

        /// Gets questions with choices that can be updated (including update to choices)
        /// This is an enhanced update that also handles choice modifications
        public bool UpdateQuestionWithChoices(int questionId, MultipleChoiceQuestion question)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Update question
                            string questionQuery = @"UPDATE Questions SET 
                                           QuestionText = @QuestionText,
                                           PointValue = @PointValue,
                                           DifficultyLevel = @DifficultyLevel,
                                           CognitiveLevel = @CognitiveLevel,
                                           Topic = @Topic,
                                           Explanation = @Explanation
                                           WHERE QuestionId = @QuestionId";

                            using (MySqlCommand cmd = new MySqlCommand(questionQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuestionId", questionId);
                                cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                                cmd.Parameters.AddWithValue("@PointValue", question.PointValue);
                                cmd.Parameters.AddWithValue("@DifficultyLevel", question.DifficultyLevel.ToString());
                                cmd.Parameters.AddWithValue("@CognitiveLevel", question.CognitiveLevel.ToString());
                                cmd.Parameters.AddWithValue("@Topic", question.Topic ?? "");
                                cmd.Parameters.AddWithValue("@Explanation", question.Explanation ?? "");

                                cmd.ExecuteNonQuery();
                            }

                            // Delete existing choices
                            string deleteChoices = "DELETE FROM QuestionChoices WHERE QuestionId = @QuestionId";
                            using (MySqlCommand cmd = new MySqlCommand(deleteChoices, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuestionId", questionId);
                                cmd.ExecuteNonQuery();
                            }

                            // Insert new choices
                            if (question.Choices != null && question.Choices.Count > 0)
                            {
                                foreach (var choice in question.Choices)
                                {
                                    string choiceQuery = @"INSERT INTO QuestionChoices 
                                                 (QuestionId, ChoiceText, IsCorrect, OrderNumber)
                                                 VALUES 
                                                 (@QuestionId, @ChoiceText, @IsCorrect, @OrderNumber)";

                                    using (MySqlCommand cmd = new MySqlCommand(choiceQuery, conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@QuestionId", questionId);
                                        cmd.Parameters.AddWithValue("@ChoiceText", choice.ChoiceText);
                                        cmd.Parameters.AddWithValue("@IsCorrect", choice.IsCorrect);
                                        cmd.Parameters.AddWithValue("@OrderNumber", choice.OrderNumber);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating question with choices: {ex.Message}", ex);
            }
        }


        /// Get question by ID
        public MultipleChoiceQuestion GetQuestionById(int questionId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT * FROM Questions WHERE QuestionId = @QuestionId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuestionId", questionId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var question = MapReaderToQuestion(reader);
                                reader.Close();

                                // Load choices
                                question.Choices = GetChoicesByQuestion(conn, questionId);
                                return question;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting question: {ex.Message}", ex);
            }
        }

        /// Update question
        public bool UpdateQuestion(int questionId, MultipleChoiceQuestion question)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Changed Points to PointValue
                    string query = @"UPDATE Questions SET 
                                   QuestionText = @QuestionText,
                                   PointValue = @PointValue,
                                   DifficultyLevel = @DifficultyLevel,
                                   CognitiveLevel = @CognitiveLevel,
                                   Topic = @Topic,
                                   Explanation = @Explanation
                                   WHERE QuestionId = @QuestionId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuestionId", questionId);
                        cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                        cmd.Parameters.AddWithValue("@PointValue", question.PointValue); // ✅ FIXED
                        cmd.Parameters.AddWithValue("@DifficultyLevel", question.DifficultyLevel.ToString());
                        cmd.Parameters.AddWithValue("@CognitiveLevel", question.CognitiveLevel.ToString());
                        cmd.Parameters.AddWithValue("@Topic", question.Topic ?? "");
                        cmd.Parameters.AddWithValue("@Explanation", question.Explanation ?? "");

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating question: {ex.Message}", ex);
            }
        }

        /// Delete question
        public bool DeleteQuestion(int questionId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Delete choices first (foreign key constraint)
                    string deleteChoices = "DELETE FROM QuestionChoices WHERE QuestionId = @QuestionId";
                    using (MySqlCommand cmd = new MySqlCommand(deleteChoices, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuestionId", questionId);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete question
                    string deleteQuestion = "DELETE FROM Questions WHERE QuestionId = @QuestionId";
                    using (MySqlCommand cmd = new MySqlCommand(deleteQuestion, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuestionId", questionId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting question: {ex.Message}", ex);
            }
        }

        /// Get choices for a question
        private List<QuestionChoice> GetChoicesByQuestion(MySqlConnection conn, int questionId)
        {
            List<QuestionChoice> choices = new List<QuestionChoice>();

            // ✅ FIXED: Changed ChoiceOrder to OrderNumber in ORDER BY
            string query = "SELECT * FROM QuestionChoices WHERE QuestionId = @QuestionId ORDER BY OrderNumber";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@QuestionId", questionId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        choices.Add(new QuestionChoice
                        {
                            ChoiceId = reader.GetInt32("ChoiceId"),
                            QuestionId = reader.GetInt32("QuestionId"),
                            ChoiceText = reader.GetString("ChoiceText"),
                            IsCorrect = reader.GetBoolean("IsCorrect"),
                            OrderNumber = reader.GetInt32("OrderNumber") // ✅ FIXED
                        });
                    }
                }
            }

            return choices;
        }

        /// Map database reader to question object
        private MultipleChoiceQuestion MapReaderToQuestion(MySqlDataReader reader)
        {
            return new MultipleChoiceQuestion
            {
                QuestionId = reader.GetInt32("QuestionId"),
                TestId = reader.IsDBNull(reader.GetOrdinal("TestId")) ? (int?)null : reader.GetInt32("TestId"),
                QuestionText = reader.GetString("QuestionText"),
                QuestionType = reader.IsDBNull(reader.GetOrdinal("QuestionType")) ? "MultipleChoice" : reader.GetString("QuestionType"),
                PointValue = reader.GetInt32("PointValue"), // ✅ FIXED from "Points"
                DifficultyLevel = (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), reader.GetString("DifficultyLevel")),
                CognitiveLevel = (CognitiveLevel)Enum.Parse(typeof(CognitiveLevel), reader.GetString("CognitiveLevel")),
                Topic = reader.IsDBNull(reader.GetOrdinal("Topic")) ? "" : reader.GetString("Topic"),
                Explanation = reader.IsDBNull(reader.GetOrdinal("Explanation")) ? "" : reader.GetString("Explanation")
            };
        }
    }
}
