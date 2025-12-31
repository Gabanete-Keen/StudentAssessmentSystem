using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Results;
using StudentAssessmentSystem.DataAccess;
// Purpose: Separate class for difficulty calculations (following Single Responsibility)
// Connected to: ItemAnalyzer
namespace StudentAssessmentSystem.BusinessLogic.Analysis
{
    // Simple calculator for difficulty index
    public class DifficultyCalculator
    {
       
        /// Calculate difficulty (P-value) from a list of answers
        /// P = (Correct answers) / (Total answers)
        public decimal Calculate(List<StudentAnswer> answers)
        {
            if (answers == null || answers.Count == 0)
                return 0;

            int correctCount = answers.Count(a => a.IsCorrect);
            int totalCount = answers.Count;

            return (decimal)correctCount / totalCount;
        }

       
        /// Categorize difficulty level
        public string Categorize(decimal pValue)
        {
            if (pValue > 0.75m)
                return "Easy";
            else if (pValue >= 0.25m)
                return "Average";
            else
                return "Difficult";
        }

    }
}
