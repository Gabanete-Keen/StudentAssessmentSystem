using StudentAssessmentSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Represents a test/assessment created by a teacher
// SOLID: Single Responsibility - Only handles test properties
// Connected to: Question, TestInstance, TestRepository, TestManager
namespace StudentAssessmentSystem.Models.Assessment
{
    /// Represents a test/assessment
  
    public class Test
    {
        public int TestId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public string TestTitle { get; set; }
        public string Description { get; set; }
        public TestType TestType { get; set; }

        // Scoring properties
        public int TotalPoints { get; set; }
        public decimal PassingScore { get; set; }  // Percentage (e.g., 75.00)

        // Time constraints
        public int DurationMinutes { get; set; }

        // Instructions
        public string Instructions { get; set; }

        // Configuration flags
        public bool RandomizeQuestions { get; set; }
        public bool RandomizeChoices { get; set; }
        public bool ShowCorrectAnswers { get; set; }
        public bool AllowReview { get; set; }


        // Metadata
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation property - Questions in this test
        public List<Question> Questions { get; set; }

        public Test()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
            Questions = new List<Question>();
            AllowReview = true;
            PassingScore = 60.00m; // Default 60%
        }
        /// Gets the total number of questions in the test
        
        public int QuestionCount => Questions?.Count ?? 0;

       
        /// Calculates total points from all questions
        /// DRY: Centralized calculation
        public void CalculateTotalPoints()
        {
            TotalPoints = 0;
            if (Questions != null)
            {
                foreach (var question in Questions)
                {
                    TotalPoints += question.PointValue;
                }
            }
        }
        /// Validates test data is complete
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(TestTitle) &&
                   SubjectId > 0 &&
                   TeacherId > 0 &&
                   DurationMinutes > 0 &&
                   TotalPoints > 0;
        }
    }
}
    

