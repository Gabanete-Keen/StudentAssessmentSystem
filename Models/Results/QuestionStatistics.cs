using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Stores item analysis statistics for a question
// Connected to: Question, ItemAnalyzer, DifficultyCalculator
namespace StudentAssessmentSystem.Models.Results
{
    /// Statistical data for item analysis
    /// Calculated after test administration
    public class QuestionStatistics
    {
        public int StatId { get; set; }
        public int QuestionId { get; set; }
        public int TestInstanceId { get; set; }

        // Basic statistics
        public int TotalAttempts { get; set; }
        public int CorrectCount { get; set; }

        // Item analysis indices
        public decimal DifficultyIndex { get; set; }      // P-value (0-1)
        public decimal? DiscriminationIndex { get; set; }  // D-value (-1 to 1)

        public DateTime LastCalculated { get; set; }

        public QuestionStatistics()
        {
            LastCalculated = DateTime.Now;
        }

        /// Gets difficulty level category based on P-value
        public string GetDifficultyCategory()
        {
            if (DifficultyIndex > 0.75m) return "Easy";
            if (DifficultyIndex < 0.25m) return "Difficult";
            return "Average";
        }

        /// Gets discrimination quality interpretation
        public string GetDiscriminationQuality()
        {
            if (!DiscriminationIndex.HasValue) return "Not Calculated";

            decimal d = DiscriminationIndex.Value;
            if (d >= 0.40m) return "Excellent";
            if (d >= 0.30m) return "Good";
            if (d >= 0.20m) return "Marginal";
            if (d >= 0.10m) return "Poor";
            return "Very Poor / Revise";
        }

        /// Checks if question needs review based on indices
        public bool NeedsReview()
        {
            // Flag questions that are too easy/difficult or discriminate poorly
            bool tooEasyOrHard = DifficultyIndex > 0.90m || DifficultyIndex < 0.15m;
            bool poorDiscrimination = DiscriminationIndex.HasValue && DiscriminationIndex.Value < 0.20m;

            return tooEasyOrHard || poorDiscrimination;
        }

    }
}
