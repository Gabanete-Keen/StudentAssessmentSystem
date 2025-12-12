using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;  
using StudentAssessmentSystem.Models.Users;
using StudentAssessmentSystem.Models.Enums;
// Purpose: Handles all MySQL database operations for Users
// Connected to: User, Admin, Teacher, Student models, UserManager
// SOLID: Single Responsibility - Only database operations for users
namespace StudentAssessmentSystem.DataAccess.Repositories
{
    /// Repository for User database operations - MySQL version
    /// Implements CRUD operations for users
    public class UserRepositories : IRepository<User>
    {
       
        /// Adds a new user to the MySQL database
        /// POLYMORPHISM: Works with Admin, Teacher, or Student
       
        public int Add(User entity)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    
                    string query = @"INSERT INTO Users (Username, PasswordHash, FirstName, LastName, Email, UserRole, IsActive, CreatedDate)
                                   VALUES (@Username, @PasswordHash, @FirstName, @LastName, @Email, @UserRole, @IsActive, @CreatedDate);
                                   SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", entity.Username);
                        cmd.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
                        cmd.Parameters.AddWithValue("@FirstName", entity.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", entity.LastName);
                        cmd.Parameters.AddWithValue("@Email", entity.Email);
                        cmd.Parameters.AddWithValue("@UserRole", entity.Role.ToString());
                        cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedDate", entity.CreatedDate);

                        int userId = Convert.ToInt32(cmd.ExecuteScalar());

                        // Insert into role-specific table
                        InsertRoleSpecificData(conn, entity, userId);

                        return userId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding user: {ex.Message}", ex);
            }
        }

      
        /// Inserts data into Admin/Teacher/Student tables
        /// POLYMORPHISM: Handles different user types    
        private void InsertRoleSpecificData(MySqlConnection conn, User entity, int userId)
        {
            if (entity is Admin admin)
            {
                string query = "INSERT INTO Admins (AdminId, AccessLevel) VALUES (@AdminId, @AccessLevel)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AdminId", userId);
                    cmd.Parameters.AddWithValue("@AccessLevel", admin.AccessLevel);
                    cmd.ExecuteNonQuery();
                }
            }
            else if (entity is Teacher teacher)
            {
                string query = "INSERT INTO Teachers (TeacherId, EmployeeNumber, Department) VALUES (@TeacherId, @EmployeeNumber, @Department)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TeacherId", userId);
                    cmd.Parameters.AddWithValue("@EmployeeNumber", teacher.EmployeeNumber ?? "");
                    cmd.Parameters.AddWithValue("@Department", teacher.Department ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
            else if (entity is Student student)
            {
                string query = "INSERT INTO Students (StudentId, StudentNumber, YearLevel) VALUES (@StudentId, @StudentNumber, @YearLevel)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", userId);
                    cmd.Parameters.AddWithValue("@StudentNumber", student.StudentNumber ?? "");
                    cmd.Parameters.AddWithValue("@YearLevel", student.YearLevel);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// Gets user by ID
        public User GetById(int id)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT * FROM Users WHERE UserId = @UserId";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapReaderToUser(reader);
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user: {ex.Message}", ex);
            }
        }

        /// Gets all users
        public List<User> GetAll()
        {
            List<User> users = new List<User>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // MySQL uses TRUE/FALSE instead of 1/0 for boolean
                    string query = "SELECT * FROM Users WHERE IsActive = TRUE";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(MapReaderToUser(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting users: {ex.Message}", ex);
            }

            return users;
        }

    
        /// Updates user information  
        public bool Update(User entity)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE Users SET 
                                   Username = @Username, 
                                   FirstName = @FirstName, 
                                   LastName = @LastName, 
                                   Email = @Email, 
                                   IsActive = @IsActive
                                   WHERE UserId = @UserId";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", entity.UserId);
                        cmd.Parameters.AddWithValue("@Username", entity.Username);
                        cmd.Parameters.AddWithValue("@FirstName", entity.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", entity.LastName);
                        cmd.Parameters.AddWithValue("@Email", entity.Email);
                        cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}", ex);
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

                    // MySQL uses FALSE instead of 0
                    string query = "UPDATE Users SET IsActive = FALSE WHERE UserId = @UserId";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", id);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}", ex);
            }
        }

      
        /// Gets user by username (for login)
        public User GetByUsername(string username)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT * FROM Users WHERE Username = @Username AND IsActive = TRUE";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapReaderToUser(reader);
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user by username: {ex.Message}", ex);
            }
        }

      
        /// Updates last login date   
        public bool UpdateLastLogin(int userId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                   
                    string query = "UPDATE Users SET LastLoginDate = NOW() WHERE UserId = @UserId";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating last login: {ex.Message}", ex);
            }
        }

        /// Maps MySQL database row to User object
        /// POLYMORPHISM: Creates correct user type (Admin/Teacher/Student)
        private User MapReaderToUser(MySqlDataReader reader)
        {
            // Read role to determine which type to create
            string roleString = reader["UserRole"].ToString();
            UserRole role = (UserRole)Enum.Parse(typeof(UserRole), roleString);

            User user = null;

            // Create appropriate user type
            switch (role)
            {
                case UserRole.Admin:
                    user = new Admin();
                    break;
                case UserRole.Teacher:
                    user = new Teacher();
                    break;
                case UserRole.Student:
                    user = new Student();
                    break;
            }

            // Fill common properties using MySQL-specific methods
            user.UserId = reader.GetInt32("UserId");
            user.Username = reader.GetString("Username");
            user.PasswordHash = reader.GetString("PasswordHash");
            user.FirstName = reader.GetString("FirstName");
            user.LastName = reader.GetString("LastName");
            user.Email = reader.GetString("Email");
            user.Role = role;
            user.IsActive = reader.GetBoolean("IsActive");
            user.CreatedDate = reader.GetDateTime("CreatedDate");

            // Check for NULL using GetOrdinal
            int lastLoginOrdinal = reader.GetOrdinal("LastLoginDate");
            if (!reader.IsDBNull(lastLoginOrdinal))
                user.LastLoginDate = reader.GetDateTime(lastLoginOrdinal);

            return user;
        }
    }
}

