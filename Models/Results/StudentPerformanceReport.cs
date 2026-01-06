using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// Purpose: Holds student performance analysis results
/// Connected to: PerformanceAnalyzer, StudentDashboard
/// SOLID: Single Responsibility - Only stores performance data
namespace StudentAssessmentSystem.Models.Results
{
    /// Represents a comprehensive performance report for a single student
    public class StudentPerformanceReport
    {
        // --- Student Identification ---
        public int StudentId { get; set; }
        public int TestInstanceId { get; set; }
        public string StudentName { get; set; }
        public string TestTitle { get; set; }

        // --- Overall Performance ---
        public decimal OverallAccuracy { get; set; }  // 0.0 to 1.0
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }

        // --- Performance by Cognitive Level ---
        public Dictionary<string, decimal> PerformanceByCognitiveLevel { get; set; }

        // --- Performance by Topic ---
        public Dictionary<string, decimal> PerformanceByTopic { get; set; }

        // --- Strengths & Weaknesses ---
        public List<string> Strengths { get; set; }
        public List<string> Weaknesses { get; set; }
        public List<string> Recommendations { get; set; }

        public DateTime AnalysisDate { get; set; }

        /// Constructor - Initializes collections
        public StudentPerformanceReport()
        {
            PerformanceByCognitiveLevel = new Dictionary<string, decimal>();
            PerformanceByTopic = new Dictionary<string, decimal>();
            Strengths = new List<string>();
            Weaknesses = new List<string>();
            Recommendations = new List<string>();
            AnalysisDate = DateTime.Now;
        }

        /// Gets overall performance level as text
        public string GetPerformanceLevel()
        {
            if (OverallAccuracy >= 0.90m) return "Excellent";
            if (OverallAccuracy >= 0.80m) return "Very Good";
            if (OverallAccuracy >= 0.70m) return "Good";
            if (OverallAccuracy >= 0.60m) return "Satisfactory";
            return "Needs Improvement";
        }

        /// Checks if student has any weaknesses identified
        public bool HasWeaknesses()
        {
            return Weaknesses != null && Weaknesses.Count > 0;
        }

        /// Gets a summary text for quick display
        public string GetSummaryText()
        {
            return $"Overall: {OverallAccuracy:P0} ({CorrectAnswers}/{TotalQuestions} correct) - " +
                   $"Performance Level: {GetPerformanceLevel()}";
        }
    }
}
