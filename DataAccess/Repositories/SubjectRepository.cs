using MySql.Data.MySqlClient;
using StudentAssessmentSystem.Models.Academic;
using System;
using System.Collections.Generic;

namespace StudentAssessmentSystem.DataAccess.Repositories
{
    /// Repository for Subject database operations
    /// Handles CRUD operations for subjects/courses
    public class SubjectRepository
    {
        /// Adds a new subject to the database
        public int Add(Subject subject)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO subjects (SubjectCode, SubjectName, Description, Units, IsActive)
                                   VALUES (@SubjectCode, @SubjectName, @Description, @Units, @IsActive);
                                   SELECT LAST_INSERT_ID();";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SubjectCode", subject.SubjectCode);
                        cmd.Parameters.AddWithValue("@SubjectName", subject.SubjectName);
                        cmd.Parameters.AddWithValue("@Description", subject.Description ?? "");
                        cmd.Parameters.AddWithValue("@Units", subject.Units);
                        cmd.Parameters.AddWithValue("@IsActive", subject.IsActive);

                        int subjectId = Convert.ToInt32(cmd.ExecuteScalar());
                        return subjectId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding subject: {ex.Message}", ex);
            }
        }

        /// Gets a subject by ID
        public Subject GetById(int subjectId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT * FROM subjects WHERE SubjectId = @SubjectId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SubjectId", subjectId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapReaderToSubject(reader);
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting subject: {ex.Message}", ex);
            }
        }

        /// Gets all active subjects
        public List<Subject> GetAll()
        {
            List<Subject> subjects = new List<Subject>();

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT * FROM subjects WHERE IsActive = TRUE ORDER BY SubjectCode";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                subjects.Add(MapReaderToSubject(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting subjects: {ex.Message}", ex);
            }

            return subjects;
        }

        /// Updates an existing subject
        public bool Update(Subject subject)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE subjects SET 
                                   SubjectCode = @SubjectCode,
                                   SubjectName = @SubjectName,
                                   Description = @Description,
                                   Units = @Units,
                                   IsActive = @IsActive
                                   WHERE SubjectId = @SubjectId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SubjectId", subject.SubjectId);
                        cmd.Parameters.AddWithValue("@SubjectCode", subject.SubjectCode);
                        cmd.Parameters.AddWithValue("@SubjectName", subject.SubjectName);
                        cmd.Parameters.AddWithValue("@Description", subject.Description ?? "");
                        cmd.Parameters.AddWithValue("@Units", subject.Units);
                        cmd.Parameters.AddWithValue("@IsActive", subject.IsActive);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating subject: {ex.Message}", ex);
            }
        }

        /// Soft delete - sets IsActive to false
        public bool Delete(int subjectId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = "UPDATE subjects SET IsActive = FALSE WHERE SubjectId = @SubjectId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting subject: {ex.Message}", ex);
            }
        }

        /// Checks if a subject code already exists
        public bool SubjectCodeExists(string subjectCode, int? excludeSubjectId = null)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM subjects WHERE SubjectCode = @SubjectCode AND IsActive = TRUE";

                    if (excludeSubjectId.HasValue)
                    {
                        query += " AND SubjectId != @SubjectId";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SubjectCode", subjectCode);
                        if (excludeSubjectId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@SubjectId", excludeSubjectId.Value);
                        }

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking subject code: {ex.Message}", ex);
            }
        }

        /// Maps database reader to Subject object
        private Subject MapReaderToSubject(MySqlDataReader reader)
        {
            return new Subject
            {
                SubjectId = reader.GetInt32("SubjectId"),
                SubjectCode = reader.GetString("SubjectCode"),
                SubjectName = reader.GetString("SubjectName"),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? "" : reader.GetString("Description"),
                Units = reader.GetInt32("Units"),
                IsActive = reader.GetBoolean("IsActive")
            };
        }
    }
}