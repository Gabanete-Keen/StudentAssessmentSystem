using MySql.Data.MySqlClient;
using StudentAssessmentSystem.Models.Users;
using System;
using System.Collections.Generic;

namespace StudentAssessmentSystem.DataAccess.Repositories
{
    /// <summary>
    /// Repository for Student database operations
    /// </summary>
    public class StudentRepository
    {
        /// <summary>
        /// Gets a student by ID
        /// </summary>
        public Student GetById(int studentId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT s.*, u.Username, u.Email, u.FirstName, u.LastName
                        FROM Students s
                        INNER JOIN Users u ON s.UserId = u.UserId
                        WHERE s.StudentId = @StudentId";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Student
                                {
                                    StudentId = reader.GetInt32("StudentId"),
                                    UserId = reader.GetInt32("UserId"),
                                    Username = reader.GetString("Username"),
                                    Email = reader.GetString("Email"),
                                    FirstName = reader.GetString("FirstName"),
                                    LastName = reader.GetString("LastName")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting student: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Gets list of section IDs the student is enrolled in
        /// </summary>
        public List<int> GetEnrolledSectionIds(int studentId)
        {
            List<int> sectionIds = new List<int>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT SectionId 
                        FROM sectionenrollments 
                        WHERE StudentId = @StudentId";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sectionIds.Add(reader.GetInt32("SectionId"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting enrolled sections: {ex.Message}", ex);
            }

            return sectionIds;
        }

        /// <summary>
        /// Gets all students
        /// </summary>
        public List<Student> GetAll()
        {
            List<Student> students = new List<Student>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT s.*, u.Username, u.Email, u.FirstName, u.LastName
                        FROM Students s
                        INNER JOIN Users u ON s.UserId = u.UserId
                        ORDER BY u.LastName, u.FirstName";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(new Student
                            {
                                StudentId = reader.GetInt32("StudentId"),
                                UserId = reader.GetInt32("UserId"),
                                Username = reader.GetString("Username"),
                                Email = reader.GetString("Email"),
                                FirstName = reader.GetString("FirstName"),
                                LastName = reader.GetString("LastName")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting students: {ex.Message}", ex);
            }

            return students;
        }

        /// <summary>
        /// Enrolls a student in a section
        /// </summary>
        public bool EnrollInSection(int studentId, int sectionId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO sectionenrollments (StudentId, SectionId, EnrollmentDate)
                        VALUES (@StudentId, @SectionId, @EnrollmentDate)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        cmd.Parameters.AddWithValue("@SectionId", sectionId);
                        cmd.Parameters.AddWithValue("@EnrollmentDate", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error enrolling student: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Removes a student from a section
        /// </summary>
        public bool RemoveFromSection(int studentId, int sectionId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"DELETE FROM sectionenrollments 
                        WHERE StudentId = @StudentId AND SectionId = @SectionId";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        cmd.Parameters.AddWithValue("@SectionId", sectionId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error removing student from section: {ex.Message}", ex);
            }
        }
    }
}