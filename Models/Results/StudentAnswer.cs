using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Records a student's answer to a specific question
// Connected to: TestResult, Question, QuestionChoice
namespace StudentAssessmentSystem.Models.Results
{
    /// Represents a student's answer to a question
    /// Used for scoring and item analysis
    public class StudentAnswer
    {
        public int AnswerId { get; set; }
        public int ResultId { get; set; }
        public int QuestionId { get; set; }

        // For multiple choice - which choice was selected
        public int? SelectedChoiceId { get; set; }

        // For other question types (future expansion)
        public string AnswerText { get; set; }

        // Scoring
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
        public int TimeSpentSeconds { get; set; }

        
        /// Validates the answer data
        
        public bool IsValid()
        {
            return ResultId > 0 && QuestionId > 0;
        }

    }
}
