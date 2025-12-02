using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Represents a class section (group of students for a subject)
// Connected to: Subject, Teacher, Student, Test, SectionRepository
namespace StudentAssessmentSystem.Models.Academic
{
    /// Represents a section/class
    /// Links students with a subject and teacher
    public class Section
    {
        public int SectionId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public string SectionName { get; set; }  // ex "CS101-A"
        public string AcademicYear { get; set; } // ex "2024-2025"
        public string Semester { get; set; }     // ex "First Semester"
        public string Schedule { get; set; }     // ex "MWF 10:00-11:00"

        // Navigation properties (loaded from database when needed)
        public Subject Subject { get; set; }
        public List<int> EnrolledStudentIds { get; set; }

        public Section()
        {
            EnrolledStudentIds = new List<int>();
        }

        
        /// Gets the number of students enrolled  
        public int EnrollmentCount => EnrolledStudentIds.Count;

       
        /// Validates section data
        public bool IsValid()//Checks if the section has all required fields filled
        {
            return SubjectId > 0 &&
                   TeacherId > 0 &&
                   !string.IsNullOrWhiteSpace(SectionName) &&
                   !string.IsNullOrWhiteSpace(AcademicYear);
        }

        public override string ToString()
        {
            return $"{SectionName} - {Semester} {AcademicYear}";
        }
    }
}
