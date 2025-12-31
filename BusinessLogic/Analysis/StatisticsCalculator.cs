using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: General statistics calculations (mean, median, etc.)
// Connected to: TestResult, Performance rep

namespace StudentAssessmentSystem.BusinessLogic.Analysis
{
    
    public class StatisticsCalculator
    {
        // Calculate average (mean) score
        public decimal CalculateMean(List<int> scores)
        {
            if (scores == null || scores.Count == 0)
                return 0;

            return (decimal)scores.Average();
        }

        
        /// Calculate median (middle value)
        public decimal CalculateMedian(List<int> scores)
        {
            if (scores == null || scores.Count == 0)
                return 0;

            // Sort scores
            List<int> sorted = scores.OrderBy(s => s).ToList();
            int count = sorted.Count;

            // If odd number of scores, take middle one
            if (count % 2 == 1)
            {
                return sorted[count / 2];
            }
            else
            {
                // If even, take average of two middle scores
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2m;
            }
        }

     
        /// Calculate standard deviation (how spread out the scores are)
        public decimal CalculateStandardDeviation(List<int> scores)
        {
            if (scores == null || scores.Count == 0)
                return 0;

            decimal mean = CalculateMean(scores);
            
            // Calculate variance
            decimal sumOfSquares = 0;
            foreach (int score in scores)
            {
                decimal diff = score - mean;
                sumOfSquares += diff * diff;
            }
            
            decimal variance = sumOfSquares / scores.Count;
            
            // Standard deviation is square root of variance
            return (decimal)Math.Sqrt((double)variance);
        }

       
        /// Calculate passing rate (percentage who passed)
        public decimal CalculatePassingRate(List<bool> passedStatus)
        {
            if (passedStatus == null || passedStatus.Count == 0)
                return 0;

            int passedCount = passedStatus.Count(p => p);
            return ((decimal)passedCount / passedStatus.Count) * 100;
        }

       
        /// Find highest score
        public int GetMaxScore(List<int> scores)
        {
            if (scores == null || scores.Count == 0)
                return 0;

            return scores.Max();
        }

        
        /// Find lowest score
        public int GetMinScore(List<int> scores)
        {
            if (scores == null || scores.Count == 0)
                return 0;

            return scores.Min();
        }

    }
}
