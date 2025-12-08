using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Represents a SPECIFIC administration of a test to a section
// Why separate from Test? A test TEMPLATE can be used multiple times for different sections
// Connected to: Test, Section, TestResult, TestInstanceRepository
namespace StudentAssessmentSystem.Models.Assessment
{
 /// Represents a specific administration of a test to a section
 /// EXAMPLE: "Midterm Exam" (Test) -> Given to "CS101-A Section" on "Oct 15" (TestInstance)
 /// SOLID: Single Responsibility - Handles test scheduling/administration
    public class TestInstance
    {
        public int InstanceId { get; set; }
        public int TestId { get; set; }
        public int SectionId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public Test Test { get; set; }

        public TestInstance()
        {
            IsActive = true;
        }

       
        /// Checks if the test is currently open for taking
        
        public bool IsCurrentlyActive()
        {
            DateTime now = DateTime.Now;
            return IsActive && now >= StartDateTime && now <= EndDateTime;
        }

        
        /// Checks if the test period has ended
        
        public bool HasEnded()
        {
            return DateTime.Now > EndDateTime;
        }

        
        /// Gets remaining time in minutes
        
        public int GetRemainingMinutes()
        {
            if (!IsCurrentlyActive()) return 0;
            TimeSpan remaining = EndDateTime - DateTime.Now;
            return (int)remaining.TotalMinutes;
        }
    }
}
