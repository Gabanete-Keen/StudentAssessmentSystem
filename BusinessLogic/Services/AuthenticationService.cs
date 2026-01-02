using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Users;
using System.Security.Cryptography;
// Purpose: Handles user login authentication
// Connected to: UserRepository, LoginForm
namespace StudentAssessmentSystem.BusinessLogic.Services
{
    // Simple authentication service - handles login
    public class AuthenticationService
    {
        private readonly UserRepositories _userRepository;

        public AuthenticationService()
        {
            _userRepository = new UserRepositories();
        }

        /// Authenticates a user with username and password
        /// Returns User if successful, NULL if failed
        public User Authenticate(string username, string password)
        {
            // Get user from database
            User user = _userRepository.GetByUsername(username);

            // Check if user exists and is active
            if (user == null || !user.IsActive)
                return null;

            // Check password
            // NOTE: For learning, we're using plain text passwords
            // MUST hash passwords!
            if (user.PasswordHash == password)
            {
                return user;
            }

            return null;
        }

     
        /// Changes a user's password
        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            try
            {
                // Get user
                User user = _userRepository.GetById(userId);
                if (user == null)
                    return false;

                // Verify old password
                if (user.PasswordHash != oldPassword)
                    return false;

                // Update password
                user.PasswordHash = newPassword;
                return _userRepository.Update(user);
            }
            catch
            {
                return false;
            }
        }

     
        /// Hashes a password using SHA256
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

    
        /// Validates password strength
        public bool IsPasswordStrong(string password)
        {
            // Simple validation
            if (password.Length < 8)
                return false;

            return true;
        }
    }
}

