using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentAssessmentSystem.Models.Results;
// Purpose: Generates reports and summaries
// Connected to: TestResult, ItemAnalyzer
namespace StudentAssessmentSystem.BusinessLogic.Services
{
    /// Simple report service - generates text reports
    public class ReportService
    {
        /// Generates a simple test result report for a student
        public string GenerateStudentReport(TestResult result, string testTitle, string studentName)
        {
            StringBuilder report = new StringBuilder();

            report.AppendLine("========================================");
            report.AppendLine("          TEST RESULT REPORT");
            report.AppendLine("========================================");
            report.AppendLine();
            report.AppendLine($"Student: {studentName}");
            report.AppendLine($"Test: {testTitle}");
            report.AppendLine($"Date: {result.StartTime:yyyy-MM-dd}");
            report.AppendLine();
            report.AppendLine("========================================");
            report.AppendLine("              SCORE SUMMARY");
            report.AppendLine("========================================");
            report.AppendLine();
            report.AppendLine($"Raw Score: {result.RawScore} / {result.TotalPoints}");
            report.AppendLine($"Percentage: {result.Percentage:F2}%");
            report.AppendLine($"Letter Grade: {result.LetterGrade}");
            report.AppendLine($"Status: {(result.Passed ? "PASSED" : "FAILED")}");
            report.AppendLine();

            if (result.SubmitTime.HasValue)
            {
                TimeSpan timeTaken = result.SubmitTime.Value - result.StartTime;
                report.AppendLine($"Time Taken: {timeTaken.TotalMinutes:F0} minutes");
            }

            report.AppendLine();
            report.AppendLine("========================================");

            return report.ToString();
        }

        /// <summary>
        /// Generates a simple class performance summary
        /// </summary>
        public string GenerateClassSummary(List<TestResult> results, string testTitle)
        {
            if (results == null || results.Count == 0)
                return "No results to report.";

            StringBuilder report = new StringBuilder();

            report.AppendLine("========================================");
            report.AppendLine("     CLASS PERFORMANCE SUMMARY");
            report.AppendLine("========================================");
            report.AppendLine();
            report.AppendLine($"Test: {testTitle}");
            report.AppendLine($"Total Students: {results.Count}");
            report.AppendLine();
            report.AppendLine("========================================");
            report.AppendLine("            STATISTICS");
            report.AppendLine("========================================");
            report.AppendLine();

            // Calculate statistics
            decimal avgPercentage = results.Average(r => r.Percentage);
            decimal highestScore = results.Max(r => r.Percentage);
            decimal lowestScore = results.Min(r => r.Percentage);
            int passedCount = results.Count(r => r.Passed);
            decimal passingRate = ((decimal)passedCount / results.Count) * 100;

            report.AppendLine($"Average Score: {avgPercentage:F2}%");
            report.AppendLine($"Highest Score: {highestScore:F2}%");
            report.AppendLine($"Lowest Score: {lowestScore:F2}%");
            report.AppendLine($"Passing Rate: {passingRate:F2}% ({passedCount}/{results.Count})");
            report.AppendLine();

            // Grade distribution
            report.AppendLine("========================================");
            report.AppendLine("        GRADE DISTRIBUTION");
            report.AppendLine("========================================");
            report.AppendLine();

            int aCount = results.Count(r => r.LetterGrade == "A");
            int bCount = results.Count(r => r.LetterGrade == "B");
            int cCount = results.Count(r => r.LetterGrade == "C");
            int dCount = results.Count(r => r.LetterGrade == "D");
            int fCount = results.Count(r => r.LetterGrade == "F");

            report.AppendLine($"A: {aCount} ({(decimal)aCount / results.Count * 100:F1}%)");
            report.AppendLine($"B: {bCount} ({(decimal)bCount / results.Count * 100:F1}%)");
            report.AppendLine($"C: {cCount} ({(decimal)cCount / results.Count * 100:F1}%)");
            report.AppendLine($"D: {dCount} ({(decimal)dCount / results.Count * 100:F1}%)");
            report.AppendLine($"F: {fCount} ({(decimal)fCount / results.Count * 100:F1}%)");
            report.AppendLine();
            report.AppendLine("========================================");

            return report.ToString();
        }

        /// <summary>
        /// Generates item analysis summary
        /// </summary>
        public string GenerateItemAnalysisReport(List<QuestionStatistics> statistics)
        {
            if (statistics == null || statistics.Count == 0)
                return "No statistics available.";

            StringBuilder report = new StringBuilder();

            report.AppendLine("========================================");
            report.AppendLine("       ITEM ANALYSIS REPORT");
            report.AppendLine("========================================");
            report.AppendLine();
            report.AppendLine($"Total Questions: {statistics.Count}");
            report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}");
            report.AppendLine();
            report.AppendLine("========================================");
            report.AppendLine(" Q# | Difficulty | Discrimination | Status");
            report.AppendLine("========================================");

            foreach (var stat in statistics)
            {
                string difficulty = stat.DifficultyIndex > 0.75m ? "Easy" :
                                  stat.DifficultyIndex >= 0.25m ? "Avg " :
                                  "Hard";

                string discrimination = stat.DiscriminationIndex >= 0.40m ? "Excellent" :
                                       stat.DiscriminationIndex >= 0.30m ? "Good    " :
                                       stat.DiscriminationIndex >= 0.20m ? "Marginal" :
                                       "Poor    ";

                string status = stat.NeedsReview() ? "REVIEW" : "OK    ";

                report.AppendLine($" {stat.QuestionId,2} | {difficulty,-10} | {discrimination,-14} | {status}");
            }

            report.AppendLine("========================================");

            // Summary
            int needsReview = statistics.Count(s => s.NeedsReview());
            report.AppendLine();
            report.AppendLine($"Questions needing review: {needsReview}");

            return report.ToString();
        }
    }
}

