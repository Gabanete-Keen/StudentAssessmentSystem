using System;
using StudentAssessmentSystem.Models.Users;
using StudentAssessmentSystem.Models.Enums;

namespace StudentAssessmentSystem.Utilities
{
    /// Manages the current user session
    /// Simple static class - only one user can be logged in at a time
    public static class SessionManager
    {
        /// The currently logged-in user
        /// NULL if no one is logged in
        public static User CurrentUser { get; set; }

        /// Check if someone is logged in
        public static bool IsLoggedIn => CurrentUser != null;

        ///  Get the current user's ID (as a property)
        /// Returns 0 if no one is logged in
        public static int CurrentUserId => CurrentUser?.UserId ?? 0;

        /// Get the current user's ID (as a method)
        /// Returns 0 if no one is logged in
        public static int GetCurrentUserId()
        {
            return CurrentUser?.UserId ?? 0;
        }

        /// Check if current user is an Admin
        public static bool IsAdmin()
        {
            return CurrentUser?.Role == UserRole.Admin;
        }

        /// Check if current user is a Teacher
        public static bool IsTeacher()
        {
            return CurrentUser?.Role == UserRole.Teacher;
        }

        /// Check if current user is a Student
        public static bool IsStudent()
        {
            return CurrentUser?.Role == UserRole.Student;
        }

        /// Logs out the current user
        
        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
