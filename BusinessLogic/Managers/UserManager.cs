using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentAssessmentSystem.Models.Users;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Enums;

namespace StudentAssessmentSystem.BusinessLogic.Managers
{
    /// Manager class for user operations
    /// Business logic layer between UI and Data Access
    public class UserManager
    {
        private readonly UserRepositories _userRepository;

        public UserManager()
        {
            _userRepository = new UserRepositories();
        }

        /// Registers a new user in the system
        /// Validates user data before saving
        public bool RegisterUser(User user, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // VALIDATION: Check if username already exists
                if (_userRepository.GetByUsername(user.Username) != null)
                {
                    errorMessage = "Username already exists. Please choose a different username.";
                    return false;
                }

                // VALIDATION: Check required fields
                if (string.IsNullOrWhiteSpace(user.Username) ||
                    string.IsNullOrWhiteSpace(user.FirstName) ||
                    string.IsNullOrWhiteSpace(user.LastName) ||
                    string.IsNullOrWhiteSpace(user.Email))
                {
                    errorMessage = "All required fields must be filled.";
                    return false;
                }

                // VALIDATION: Email format (simple check)
                if (!user.Email.Contains("@"))
                {
                    errorMessage = "Invalid email format.";
                    return false;
                }

                // VALIDATION: Username length
                if (user.Username.Length < 4)
                {
                    errorMessage = "Username must be at least 4 characters long.";
                    return false;
                }

                // Add user to database
                int userId = _userRepository.Add(user);

                if (userId > 0)
                {
                    user.UserId = userId;
                    return true;
                }
                else
                {
                    errorMessage = "Failed to create user account.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error registering user: {ex.Message}";
                return false;
            }
        }


        /// Updates user information       
        public bool UpdateUser(User user, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // VALIDATION
                if (user.UserId <= 0)
                {
                    errorMessage = "Invalid user ID.";
                    return false;
                }

                // VALIDATION: Check required fields
                if (string.IsNullOrWhiteSpace(user.Username) ||
                    string.IsNullOrWhiteSpace(user.FirstName) ||
                    string.IsNullOrWhiteSpace(user.LastName) ||
                    string.IsNullOrWhiteSpace(user.Email))
                {
                    errorMessage = "All required fields must be filled.";
                    return false;
                }

                // VALIDATION: Email format
                if (!user.Email.Contains("@"))
                {
                    errorMessage = "Invalid email format.";
                    return false;
                }

                bool success = _userRepository.Update(user);

                if (!success)
                    errorMessage = "Failed to update user.";

                return success;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error updating user: {ex.Message}";
                return false;
            }
        }

        /// Updates role-specific data (Admin/Teacher/Student)
        public bool UpdateRoleSpecificData(User user, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // First update basic user info
                if (!UpdateUser(user, out errorMessage))
                    return false;

                // Role-specific updates are handled in the repository
                // This is just for validation and business logic
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error updating role data: {ex.Message}";
                return false;
            }
        }


        /// Deactivates a user (soft delete)
        public bool DeactivateUser(int userId, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // VALIDATION: Check if user exists
                User user = _userRepository.GetById(userId);
                if (user == null)
                {
                    errorMessage = "User not found.";
                    return false;
                }

                // VALIDATION: Prevent deleting yourself (optional)
                // You might want to add session check here

                bool success = _userRepository.Delete(userId);

                if (!success)
                    errorMessage = "Failed to deactivate user.";

                return success;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error deactivating user: {ex.Message}";
                return false;
            }
        }



        /// Gets user by ID
        public User GetUserById(int userId)
        {
            try
            {
                return _userRepository.GetById(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user: {ex.Message}", ex);
            }
        }

        /// Gets all active users
        public List<User> GetAllUsers()
        {
            try
            {
                return _userRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting users: {ex.Message}", ex);
            }
        }

        /// Gets users by role
        public List<User> GetUsersByRole(UserRole role)
        {
            try
            {
                List<User> allUsers = _userRepository.GetAll();
                return allUsers.Where(u => u.Role == role).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting users by role: {ex.Message}", ex);
            }
        }

        /// Gets all admins
        public List<Admin> GetAllAdmins()
        {
            try
            {
                List<User> allUsers = _userRepository.GetAll();
                return allUsers.OfType<Admin>().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting admins: {ex.Message}", ex);
            }
        }

        /// Gets all teachers
        public List<Teacher> GetAllTeachers()
        {
            try
            {
                List<User> allUsers = _userRepository.GetAll();
                return allUsers.OfType<Teacher>().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting teachers: {ex.Message}", ex);
            }
        }

        /// Gets all students
        public List<Student> GetAllStudents()
        {
            try
            {
                List<User> allUsers = _userRepository.GetAll();
                return allUsers.OfType<Student>().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting students: {ex.Message}", ex);
            }
        }



        /// Searches users by name or username
        public List<User> SearchUsers(string searchText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchText))
                    return GetAllUsers();

                List<User> allUsers = _userRepository.GetAll();
                string search = searchText.ToLower();

                return allUsers.Where(u =>
                    u.Username.ToLower().Contains(search) ||
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search) ||
                    u.FullName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search)
                ).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching users: {ex.Message}", ex);
            }
        }

        /// Searches users with role filter
        public List<User> SearchUsers(string searchText, UserRole? role)
        {
            try
            {
                List<User> results = string.IsNullOrWhiteSpace(searchText)
                    ? GetAllUsers()
                    : SearchUsers(searchText);

                if (role.HasValue)
                {
                    results = results.Where(u => u.Role == role.Value).ToList();
                }

                return results;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching users with filter: {ex.Message}", ex);
            }
        }


        /// Gets user statistics
        public UserStatistics GetUserStatistics()
        {
            try
            {
                List<User> allUsers = GetAllUsers();

                return new UserStatistics
                {
                    TotalUsers = allUsers.Count,
                    TotalAdmins = allUsers.Count(u => u.Role == UserRole.Admin),
                    TotalTeachers = allUsers.Count(u => u.Role == UserRole.Teacher),
                    TotalStudents = allUsers.Count(u => u.Role == UserRole.Student)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting statistics: {ex.Message}", ex);
            }
        }


        /// Checks if username is available
        public bool IsUsernameAvailable(string username, int? excludeUserId = null)
        {
            try
            {
                User existingUser = _userRepository.GetByUsername(username);

                if (existingUser == null)
                    return true;

                // If we're updating a user, exclude their own username
                if (excludeUserId.HasValue && existingUser.UserId == excludeUserId.Value)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking username: {ex.Message}", ex);
            }
        }


        /// Updates last login timestamp
        public bool UpdateLastLogin(int userId)
        {
            try
            {
                return _userRepository.UpdateLastLogin(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating last login: {ex.Message}");
                return false;
            }
        }


    }

    /// Statistics data transfer object
    public class UserStatistics
    {
        public int TotalUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalStudents { get; set; }

        public double AdminPercentage => TotalUsers > 0 ? (TotalAdmins * 100.0 / TotalUsers) : 0;
        public double TeacherPercentage => TotalUsers > 0 ? (TotalTeachers * 100.0 / TotalUsers) : 0;
        public double StudentPercentage => TotalUsers > 0 ? (TotalStudents * 100.0 / TotalUsers) : 0;
    }
}