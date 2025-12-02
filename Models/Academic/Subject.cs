using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Represents a course/subject in the school
// SOLID: Single Responsibility - Only handles subject data
// Connected to: Section, Test, SubjectRepository
namespace StudentAssessmentSystem.Models.Academic
{
    /// Represents a subject/course in the curriculum
    /// ENCAPSULATION: Properties with validation
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; }  // ex. "CS101"
        public string SubjectName { get; set; }  // ex "Introduction to Programming"
        public string Description { get; set; }
        public int Units { get; set; }         
        public bool IsActive { get; set; }

        public Subject()
        {
            IsActive = true;
            Units = 3; 
        }

        
        /// Validates if subject data is complete
        /// DRY: Reusable validation logic
        
        public bool IsValid() //Checks if the subject has all required fields filled
        {
            return !string.IsNullOrWhiteSpace(SubjectCode) &&
                   !string.IsNullOrWhiteSpace(SubjectName) &&
                   Units > 0;
        }

        // Override ToString for display purposes
        public override string ToString()
        {
            return $"{SubjectCode} - {SubjectName}";
        }
    }
}
