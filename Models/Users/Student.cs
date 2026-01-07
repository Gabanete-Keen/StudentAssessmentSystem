using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Student who takes tests and views results
// Connected to: User (inherits), TestResult, StudentAnswer, StudentRepository
namespace StudentAssessmentSystem.Models.Users
{
    using StudentAssessmentSystem.Models.Enums;
    using System.Collections.Generic;
    /// Student who takes assessments
    /// INHERITANCE: Extends User base class
    public class Student : User
    {
        public int StudentId { get; set; }
        public string StudentNumber { get; set; }
        public int YearLevel { get; set; }

        // ENCAPSULATION: Collection of enrolled sections
        public List<int> EnrolledSectionIds { get; set; }

        public Student()
        {
            Role = UserRole.Student;
            EnrolledSectionIds = new List<int>();
        }

        // POLYMORPHISM: Override with student-specific message
        public override string GetDashboardMessage()
        {
            return $"Student Portal - Welcome, {FullName}! Year Level: {YearLevel}";
        }

        // ABSTRACTION: Implement abstract method
        public override bool CanAccessAdminPanel()
        {
            return false; // Students cannot access admin panel
        }
      
        // Student-specific method
        /// Check if student is enrolled in a section
        
        public bool IsEnrolledIn(int sectionId)
        {
            return EnrolledSectionIds.Contains(sectionId);
        }
    }
}
