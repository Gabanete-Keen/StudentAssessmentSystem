using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Stores a student's result for a test instance
// Connected to: TestInstance, Student, StudentAnswer, ScoringManager
namespace StudentAssessmentSystem.Models.Results
{
    /// Represents a student's test result
    /// Links a student's performance on a specific test instance
    public class TestResult
    {
        public int ResultId { get; set; }
        public int InstanceId { get; set; }
        public int StudentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? SubmitTime { get; set; }

        // Scoring
        public int RawScore { get; set; }         // Points earned
        public int TotalPoints { get; set; }      // Total possible points
        public decimal Percentage { get; set; }   // Calculated percentage
        public string LetterGrade { get; set; }   // A, B, C, D, F
        public bool Passed { get; set; }
        public bool IsCompleted { get; set; }

        // Navigation property
        public List<StudentAnswer> Answers { get; set; }

        public TestResult()
        {
            StartTime = DateTime.Now;
            Answers = new List<StudentAnswer>();
            IsCompleted = false;
        }

        
        /// Calculates percentage score
        /// DRY: Centralized calculation logic
        
        public void CalculatePercentage()
        {
            if (TotalPoints > 0)
            {
                Percentage = ((decimal)RawScore / TotalPoints) * 100;
            }
            else
            {
                Percentage = 0;
            }
        }

        /// Determines letter grade based on percentage
        /// Can be customized based on school grading scale     
        public void AssignLetterGrade()
        {
            if (Percentage >= 90) LetterGrade = "A";
            else if (Percentage >= 80) LetterGrade = "B";
            else if (Percentage >= 70) LetterGrade = "C";
            else if (Percentage >= 60) LetterGrade = "D";
            else LetterGrade = "F";
        }
       
        /// Gets the time taken to complete the test
        public TimeSpan? GetTimeTaken()
        {
            if (SubmitTime.HasValue)
            {
                return SubmitTime.Value - StartTime;
            }
            return null;
        }

    }
}
