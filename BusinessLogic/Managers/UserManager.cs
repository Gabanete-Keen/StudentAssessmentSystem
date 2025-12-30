using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentAssessmentSystem.Models.Users;
using StudentAssessmentSystem.DataAccess.Repositories;

// Purpose: Handles user-related business logic
// SOLID: Single Responsibility - Only user management
// Connected to: UserRepository (DAL), AuthenticationService, UI Forms

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

   
            /// Deactivates a user (soft delete)
            public bool DeactivateUser(int userId, out string errorMessage)
            {
                errorMessage = string.Empty;

                try
                {
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

 
            /// Gets all teachers
            public List<Teacher> GetAllTeachers()
            {
                try
                {
                    List<User> allUsers = _userRepository.GetAll();
                    List<Teacher> teachers = new List<Teacher>();

                    foreach (var user in allUsers)
                    {
                        if (user is Teacher teacher)
                        {
                            teachers.Add(teacher);
                        }
                    }

                    return teachers;
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
                    List<Student> students = new List<Student>();

                    foreach (var user in allUsers)
                    {
                        if (user is Student student)
                        {
                            students.Add(student);
                        }
                    }

                    return students;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error getting students: {ex.Message}", ex);
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
    }
