using StudentAssessmentSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Teacher/Instructor who creates and manages tests
// Connected to: User (inherits), Test, Section, TeacherRepository
namespace StudentAssessmentSystem.Models.Users
{
    /// Teacher who creates tests and views item analysis
    /// INHERITANCE: Extends User base class

    public class Teacher : User
    {
        public string EmployeeNumber { get; set; }
        public string Department { get; set; }

        public Teacher()
        {
            Role = UserRole.Teacher;
        }

        // POLYMORPHISM: Override with teacher-specific message
        public override string GetDashboardMessage()
        {
            return $"Teacher Dashboard - Welcome, {FullName}! Department: {Department}";
        }

        // ABSTRACTION: Implement abstract method
        public override bool CanAccessAdminPanel()
        {
            return false; // Teachers cannot access admin panel
        }

        // Teacher-specific method
        /// <summary>
        /// Check if teacher can edit a specific test
        /// </summary>
        public bool CanEditTest(int testCreatorId)
        {
            return this.UserId == testCreatorId; // Can only edit own tests
        }
    }
}
