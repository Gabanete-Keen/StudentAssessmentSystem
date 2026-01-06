using MySql.Data.MySqlClient;
using StudentAssessmentSystem.Models.Assessment;
using System;
using System.Collections.Generic;

namespace StudentAssessmentSystem.DataAccess.Repositories
{
    
    /// Repository for TestInstance database operations
    /// Manages test sessions/instances for students
   
    public class TestInstanceRepository
    {
        /// Creates a new test instance (test session)
        public int CreateTestInstance(TestInstance instance)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO TestInstances 
                        (TestId, TeacherId, InstanceTitle, StartDate, EndDate, IsActive, CreatedDate) 
                        VALUES 
                        (@TestId, @TeacherId, @InstanceTitle, @StartDate, @EndDate, @IsActive, @CreatedDate);
                        SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TestId", instance.TestId);
                        cmd.Parameters.AddWithValue("@TeacherId", instance.TeacherId);
                        cmd.Parameters.AddWithValue("@InstanceTitle", instance.InstanceTitle);
                        cmd.Parameters.AddWithValue("@StartDate", instance.StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", instance.EndDate);
                        cmd.Parameters.AddWithValue("@IsActive", instance.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedDate", instance.CreatedDate);

                        int instanceId = Convert.ToInt32(cmd.ExecuteScalar());
                        return instanceId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating test instance: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all active test instances (for students to view available tests)
        /// </summary>
        public List<TestInstance> GetActiveTestInstances()
        {
            List<TestInstance> instances = new List<TestInstance>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT ti.*, t.TestTitle, t.DurationMinutes, t.TotalPoints
                        FROM TestInstances ti
                        INNER JOIN Tests t ON ti.TestId = t.TestId
                        WHERE ti.IsActive = TRUE 
                        AND ti.StartDate <= NOW() 
                        AND ti.EndDate >= NOW()
                        ORDER BY ti.StartDate DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var instance = new TestInstance
                            {
                                InstanceId = reader.GetInt32("InstanceId"),
                                TestId = reader.GetInt32("TestId"),
                                TeacherId = reader.GetInt32("TeacherId"),
                                InstanceTitle = reader.GetString("InstanceTitle"),
                                StartDate = reader.GetDateTime("StartDate"),
                                EndDate = reader.GetDateTime("EndDate"),
                                IsActive = reader.GetBoolean("IsActive"),
                                CreatedDate = reader.GetDateTime("CreatedDate")
                            };

                            instances.Add(instance);
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

        /// <summary>
        /// Gets test instances created by a specific teacher
        /// </summary>
        public List<TestInstance> GetInstancesByTeacher(int teacherId)
        {
            List<TestInstance> instances = new List<TestInstance>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT ti.*, t.TestTitle
                        FROM TestInstances ti
                        INNER JOIN Tests t ON ti.TestId = t.TestId
                        WHERE ti.TeacherId = @TeacherId
                        ORDER BY ti.CreatedDate DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TeacherId", teacherId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var instance = new TestInstance
                                {
                                    InstanceId = reader.GetInt32("InstanceId"),
                                    TestId = reader.GetInt32("TestId"),
                                    TeacherId = reader.GetInt32("TeacherId"),
                                    InstanceTitle = reader.GetString("InstanceTitle"),
                                    StartDate = reader.GetDateTime("StartDate"),
                                    EndDate = reader.GetDateTime("EndDate"),
                                    IsActive = reader.GetBoolean("IsActive"),
                                    CreatedDate = reader.GetDateTime("CreatedDate")
                                };

                                instances.Add(instance);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting teacher's test instances: {ex.Message}", ex);
            }

            return instances;
        }

        /// <summary>
        /// Gets a specific test instance by ID
        /// </summary>
        public TestInstance GetById(int instanceId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // ✅ JOIN with Tests table to get DurationMinutes
                    string query = @"SELECT ti.*, t.DurationMinutes 
                            FROM TestInstances ti
                            INNER JOIN Tests t ON ti.TestId = t.TestId
                            WHERE ti.InstanceId = @InstanceId";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InstanceId", instanceId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TestInstance
                                {
                                    InstanceId = reader.GetInt32("InstanceId"),
                                    TestId = reader.GetInt32("TestId"),
                                    TeacherId = reader.GetInt32("TeacherId"),
                                    InstanceTitle = reader.GetString("InstanceTitle"),
                                    StartDate = reader.GetDateTime("StartDate"),
                                    EndDate = reader.GetDateTime("EndDate"),
                                    IsActive = reader.GetBoolean("IsActive"),
                                    CreatedDate = reader.GetDateTime("CreatedDate"),

                                    // ✅ ADD DurationMinutes from JOIN
                                    DurationMinutes = reader.IsDBNull(reader.GetOrdinal("DurationMinutes"))
                                        ? 60  // Default 60 minutes if NULL
                                        : reader.GetInt32("DurationMinutes")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting test instance: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Assigns a section to a test instance
        /// </summary>
        public bool AssignSectionToInstance(int instanceId, int sectionId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO testinstancesections (InstanceId, SectionId)
                            VALUES (@InstanceId, @SectionId)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InstanceId", instanceId);
                        cmd.Parameters.AddWithValue("@SectionId", sectionId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error assigning section to instance: {ex.Message}", ex);
            }
        }



        /// <summary>
        /// Checks if a student has already taken this test instance
        /// </summary>
        public bool HasStudentTakenTest(int instanceId, int studentId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT COUNT(*) FROM TestResults 
                        WHERE InstanceId = @InstanceId 
                        AND StudentId = @StudentId 
                        AND IsCompleted = TRUE";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InstanceId", instanceId);
                        cmd.Parameters.AddWithValue("@StudentId", studentId);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking if student took test: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates test instance status
        /// </summary>
        public bool UpdateInstanceStatus(int instanceId, bool isActive)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE TestInstances 
                        SET IsActive = @IsActive 
                        WHERE InstanceId = @InstanceId";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InstanceId", instanceId);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating test instance status: {ex.Message}", ex);
            }
        }

        /// Gets active test instances for a student based on their enrolled sections
        /// Filters out tests the student has already completed
        public List<TestInstance> GetActiveTestInstancesByStudentSections(int studentId, List<int> sectionIds)
        {
            List<TestInstance> instances = new List<TestInstance>();

            try
            {
                if (sectionIds == null || sectionIds.Count == 0)
                    return instances;

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Build parameterized IN clause
                    string sectionIdsString = string.Join(",", sectionIds);

                    string query = $@"
                SELECT DISTINCT
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
                    s.SubjectName,
                    (SELECT COUNT(*) FROM questions WHERE TestId = t.TestId) as QuestionCount
                FROM testinstances ti
                INNER JOIN tests t ON ti.TestId = t.TestId
                LEFT JOIN subjects s ON t.SubjectId = s.SubjectId
                INNER JOIN testinstancesections tis ON ti.InstanceId = tis.InstanceId
                WHERE ti.IsActive = 1
                  AND NOW() BETWEEN ti.StartDate AND ti.EndDate
                  AND tis.SectionId IN ({sectionIdsString})
                  AND NOT EXISTS (
                      SELECT 1 FROM testresults tr 
                      WHERE tr.InstanceId = ti.InstanceId 
                        AND tr.StudentId = @StudentId
                        AND tr.IsCompleted = 1
                  )
                ORDER BY ti.StartDate DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Only include tests with questions
                                int questionCount = reader.GetInt32("QuestionCount");
                                if (questionCount == 0)
                                    continue;

                                var instance = new TestInstance
                                {
                                    InstanceId = reader.GetInt32("InstanceId"),
                                    TestId = reader.GetInt32("TestId"),
                                    TeacherId = reader.GetInt32("TeacherId"),
                                    InstanceTitle = reader.GetString("InstanceTitle"),
                                    StartDate = reader.GetDateTime("StartDate"),
                                    EndDate = reader.GetDateTime("EndDate"),
                                    IsActive = reader.GetBoolean("IsActive"),
                                    CreatedDate = reader.GetDateTime("CreatedDate"),

                                    // Display properties
                                    TestTitle = reader.GetString("TestTitle"),
                                    TestDescription = reader.IsDBNull(reader.GetOrdinal("Description"))
                                        ? "" : reader.GetString("Description"),
                                    DurationMinutes = reader.GetInt32("DurationMinutes"),
                                    TotalPoints = reader.GetInt32("TotalPoints"),
                                    PassingScore = reader.GetDouble("PassingScore"),
                                    SubjectName = reader.IsDBNull(reader.GetOrdinal("SubjectName"))
                                        ? "N/A" : reader.GetString("SubjectName")
                                };

                                instances.Add(instance);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting test instances for student sections: {ex.Message}\n{ex.StackTrace}", ex);
            }

            return instances;
        }

    }
}
