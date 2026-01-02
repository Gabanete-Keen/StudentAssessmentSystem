using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
// Simple validation service for input validation
namespace StudentAssessmentSystem.BusinessLogic.Services
{
    public class ValidationService
    {
        // Validates email format
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Simple email validation using regex
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }

       
        /// Validates username (alphanumeric only, 4-20 characters)
        public bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            if (username.Length < 4 || username.Length > 20)
                return false;

            // Only letters, numbers, and underscore
            return Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
        }

       
        /// Validates text is not empty
        public bool IsNotEmpty(string text)
        {
            return !string.IsNullOrWhiteSpace(text);
        }

    
        /// Validates number is in range
        public bool IsInRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// Validates percentage (0-100)
        public bool IsValidPercentage(decimal value)
        {
            return value >= 0 && value <= 100;
        }

    
        /// Validates year level (1-4 for college)
        public bool IsValidYearLevel(int yearLevel)
        {
            return yearLevel >= 1 && yearLevel <= 4;
        }
    }
}


