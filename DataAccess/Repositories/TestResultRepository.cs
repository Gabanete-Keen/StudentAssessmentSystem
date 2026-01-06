    using MySql.Data.MySqlClient;
    using StudentAssessmentSystem.Models.Results;
    using System;
    using System.Collections.Generic;

    namespace StudentAssessmentSystem.DataAccess.Repositories
    {
        /// Repository for TestResult database operations
        public class TestResultRepository
        {
            // Add a test result to database
            public int Add(TestResult result)
            {
                try
                {
                    using (MySqlConnection conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();

                        string query = @"INSERT INTO TestResults 
                                       (InstanceId, StudentId, StartTime, SubmitTime, RawScore, 
                                        TotalPoints, Percentage, LetterGrade, Passed, IsCompleted)
                                       VALUES 
                                       (@InstanceId, @StudentId, @StartTime, @SubmitTime, @RawScore,
                                        @TotalPoints, @Percentage, @LetterGrade, @Passed, @IsCompleted);
                                       SELECT LAST_INSERT_ID();";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
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

                            return Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error adding test result: {ex.Message}", ex);
                }
            }

            // Get test results by test instance
            public List<TestResult> GetResultsByInstance(int instanceId)
            {
                List<TestResult> results = new List<TestResult>();

                try
                {
                    using (MySqlConnection conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();

                        string query = @"SELECT * FROM TestResults WHERE InstanceId = @InstanceId";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@InstanceId", instanceId);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
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
        public TestResult GetById(int resultId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT * FROM TestResults WHERE ResultId = @ResultId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ResultId", resultId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TestResult
                                {
                                    ResultId = reader.GetInt32("ResultId"),
                                    InstanceId = reader.GetInt32("InstanceId"),
                                    StudentId = reader.GetInt32("StudentId"),
                                    StartTime = reader.GetDateTime("StartTime"),
                                    SubmitTime = reader.IsDBNull(reader.GetOrdinal("SubmitTime"))
                                        ? (DateTime?)null
                                        : reader.GetDateTime("SubmitTime"),
                                    RawScore = reader.GetInt32("RawScore"),
                                    TotalPoints = reader.GetInt32("TotalPoints"),
                                    Percentage = reader.GetDecimal("Percentage"), 
                                    LetterGrade = reader.IsDBNull(reader.GetOrdinal("LetterGrade"))
                                        ? null
                                        : reader.GetString("LetterGrade"),
                                    Passed = reader.GetBoolean("Passed"),
                                    IsCompleted = reader.GetBoolean("IsCompleted")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting test result by ID: {ex.Message}", ex);
            }

            return null;
        }


        // Get result by student and instance
        /// Gets test result by student and instance
        public TestResult GetResultByStudentAndInstance(int studentId, int instanceId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT * FROM TestResults 
                           WHERE StudentId = @StudentId 
                           AND InstanceId = @InstanceId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        cmd.Parameters.AddWithValue("@InstanceId", instanceId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TestResult
                                {
                                    ResultId = reader.GetInt32("ResultId"),
                                    InstanceId = reader.GetInt32("InstanceId"),
                                    StudentId = reader.GetInt32("StudentId"),
                                    StartTime = reader.GetDateTime("StartTime"),
                                    SubmitTime = reader.IsDBNull(reader.GetOrdinal("SubmitTime"))
                                        ? (DateTime?)null
                                        : reader.GetDateTime("SubmitTime"),
                                    IsCompleted = reader.GetBoolean("IsCompleted"),
                                    RawScore = reader.GetInt32("RawScore"),
                                    TotalPoints = reader.GetInt32("TotalPoints"),
                                    Percentage = reader.GetDecimal("Percentage"),
                                    LetterGrade = reader.GetString("LetterGrade"),
                                    Passed = reader.GetBoolean("Passed")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting result: {ex.Message}", ex);
            }

            return null;
        }


        // Update method - for updating existing result
        public bool Update(TestResult result)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE TestResults SET 
                            SubmitTime = @SubmitTime,
                            RawScore = @RawScore,
                            Percentage = @Percentage,
                            LetterGrade = @LetterGrade,
                            Passed = @Passed,
                            IsCompleted = @IsCompleted
                            WHERE ResultId = @ResultId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ResultId", result.ResultId);
                        cmd.Parameters.AddWithValue("@SubmitTime", result.SubmitTime);
                        cmd.Parameters.AddWithValue("@RawScore", result.RawScore);
                        cmd.Parameters.AddWithValue("@Percentage", result.Percentage);
                        cmd.Parameters.AddWithValue("@LetterGrade", result.LetterGrade ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Passed", result.Passed);
                        cmd.Parameters.AddWithValue("@IsCompleted", result.IsCompleted);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating test result: {ex.Message}", ex);
            }
        }


        private TestResult MapReaderToTestResult(MySqlDataReader reader)
            {
                return new TestResult
                {
                    ResultId = reader.GetInt32("ResultId"),
                    InstanceId = reader.GetInt32("InstanceId"),
                    StudentId = reader.GetInt32("StudentId"),
                    StartTime = reader.GetDateTime("StartTime"),
                    SubmitTime = reader.IsDBNull(reader.GetOrdinal("SubmitTime"))
                        ? (DateTime?)null : reader.GetDateTime("SubmitTime"),
                    RawScore = reader.GetInt32("RawScore"),
                    TotalPoints = reader.GetInt32("TotalPoints"),
                    Percentage = reader.GetDecimal("Percentage"),
                    LetterGrade = reader.IsDBNull(reader.GetOrdinal("LetterGrade"))
                        ? "" : reader.GetString("LetterGrade"),
                    Passed = reader.GetBoolean("Passed"),
                    IsCompleted = reader.GetBoolean("IsCompleted")
                };
            }
        // ✅ GET RESULTS BY STUDENT - Used by ScoringManager, StudentResultsForm
        public List<TestResult> GetResultsByStudent(int studentId)
        {
            List<TestResult> results = new List<TestResult>();

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT * FROM TestResults 
                           WHERE StudentId = @StudentId
                           ORDER BY SubmitTime DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
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
                throw new Exception($"Error getting results by student: {ex.Message}", ex);
            }

            return results;
        }

        /// Gets a test result by its ID
        public TestResult GetResultById(int resultId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM TestResults WHERE ResultId = @ResultId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ResultId", resultId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapReaderToTestResult(reader);
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting test result: {ex.Message}", ex);
            }
        }

    }
}
