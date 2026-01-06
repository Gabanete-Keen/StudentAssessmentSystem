using System;
using System.Collections.Generic;
using System.Linq;
using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Enums;
using StudentAssessmentSystem.Models.Results;

namespace StudentAssessmentSystem.BusinessLogic.Analysis
{
    /// Analyzes individual student performance on a specific test instance.
    /// Produces overall accuracy, performance by cognitive level / topic,
    /// strengths, weaknesses, and recommendations.
    public class PerformanceAnalyzer
    {
        private readonly TestResultRepository _resultRepository;
        private readonly StudentAnswerRepository _answerRepository;
        private readonly TestRepository _testRepository;

        public PerformanceAnalyzer()
        {
            _resultRepository = new TestResultRepository();
            _answerRepository = new StudentAnswerRepository();
            _testRepository = new TestRepository();
        }

        /// Main entry point: analyzes a single student's performance
        /// on a specific test instance.
        public StudentPerformanceReport AnalyzeStudentPerformance(int studentId, int testInstanceId)
        {
            // 1. Get this student's result for this instance ONLY
            TestResult result = _resultRepository
                .GetResultByStudentAndInstance(studentId, testInstanceId);

            if (result == null)
                throw new Exception("No test result found for this student on the selected test.");

            // 2. Load all answers for this result
            List<StudentAnswer> answers =
            _answerRepository.GetAnswersByResult(result.ResultId);

            if (answers == null || answers.Count == 0)
                throw new Exception("No answers found for this test result.");

            // ✅ FIXED: Count ACTUAL correct from answers (ignores corrupt RawScore)
            var submittedAnswers = answers.Where(a => a.SelectedChoiceId.HasValue).ToList();
            int totalQuestions = submittedAnswers.Count;
            int correctAnswers = submittedAnswers.Count(a => a.IsCorrect == true);
            int wrongAnswers = totalQuestions - correctAnswers;
            decimal overallAccuracy = totalQuestions > 0
                ? (decimal)correctAnswers / totalQuestions
                : 0;


            // 3. Build report
            var report = new StudentPerformanceReport
            {
                StudentId = studentId,
                TestInstanceId = testInstanceId,
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctAnswers,
                WrongAnswers = wrongAnswers,
                OverallAccuracy = Math.Round(overallAccuracy, 4),
                PerformanceByCognitiveLevel = new Dictionary<string, decimal>(),
                PerformanceByTopic = new Dictionary<string, decimal>(),
                Strengths = new List<string>(),
                Weaknesses = new List<string>(),
                Recommendations = new List<string>()
            };

            // 4. Load test for cognitive/topic breakdown (optional)
            var test = _testRepository.GetTestByInstanceId(testInstanceId);
            if (test?.Questions != null && test.Questions.Count > 0)
            {
                // Dictionary of questions by ID for quick lookup
                var questionsById = test.Questions
                    .OfType<Question>()
                    .ToDictionary(q => q.QuestionId, q => q);

                // Performance by cognitive level
                var groupedByCognitive = answers
                    .Where(a => questionsById.ContainsKey(a.QuestionId))
                    .GroupBy(a => questionsById[a.QuestionId].CognitiveLevel);

                foreach (var group in groupedByCognitive)
                {
                    int total = group.Count();
                    int correct = group.Count(a => a.IsCorrect);
                    decimal acc = total > 0 ? (decimal)correct / total : 0;
                    report.PerformanceByCognitiveLevel[group.Key.ToString()] = Math.Round(acc, 4);
                }

                // Performance by topic
                var groupedByTopic = answers
                    .Where(a => questionsById.ContainsKey(a.QuestionId))
                    .GroupBy(a => questionsById[a.QuestionId].Topic ?? "Unspecified");

                foreach (var group in groupedByTopic)
                {
                    int total = group.Count();
                    int correct = group.Count(a => a.IsCorrect);
                    decimal acc = total > 0 ? (decimal)correct / total : 0;
                    report.PerformanceByTopic[group.Key] = Math.Round(acc, 4);
                }
            }

            // 5. Strengths and weaknesses (from cognitive/topic OR overall)
            const decimal strengthThreshold = 0.80m;
            const decimal weaknessThreshold = 0.60m;

            foreach (var kvp in report.PerformanceByCognitiveLevel)
            {
                if (kvp.Value >= strengthThreshold)
                    report.Strengths.Add($"{kvp.Key}: {(kvp.Value * 100):F0}% - Strong performance");
                else if (kvp.Value <= weaknessThreshold)
                    report.Weaknesses.Add($"{kvp.Key}: {(kvp.Value * 100):F0}% - Needs improvement");
            }

            foreach (var kvp in report.PerformanceByTopic)
            {
                if (kvp.Value >= strengthThreshold)
                    report.Strengths.Add($"Topic '{kvp.Key}': {(kvp.Value * 100):F0}% - Strong area");
                else if (kvp.Value <= weaknessThreshold)
                    report.Weaknesses.Add($"Topic '{kvp.Key}': {(kvp.Value * 100):F0}% - Needs more practice");
            }

            // Overall recommendations
            if (report.Weaknesses.Count == 0)
            {
                report.Recommendations.Add("Excellent performance overall. Maintain your study habits.");
            }
            else
            {
                report.Recommendations.Add("Focus your review on the areas listed above as weaknesses.");
                report.Recommendations.Add("Revisit lecture notes and practice questions for those weaker areas.");
            }

            return report;
        }


        /// Generates a plain-text summary of the report.
        public string GenerateTextSummary(StudentPerformanceReport report)
        {
            if (report == null)
                return "No report available.";

            var lines = new List<string>
            {
                "STUDENT PERFORMANCE ANALYSIS",
                $"Total Questions : {report.TotalQuestions}",
                $"Correct        : {report.CorrectAnswers}",
                $"Wrong          : {report.WrongAnswers}",
                $"Accuracy       : {(report.OverallAccuracy * 100):F1}%",
                "",
                "STRENGTHS:"
            };

            if (report.Strengths.Count == 0)
            {
                lines.Add("  None identified yet.");
            }
            else
            {
                foreach (var s in report.Strengths)
                    lines.Add("  - " + s);
            }

            lines.Add("");
            lines.Add("WEAKNESSES:");

            if (report.Weaknesses.Count == 0)
            {
                lines.Add("  None identified.");
            }
            else
            {
                foreach (var w in report.Weaknesses)
                    lines.Add("  - " + w);
            }

            lines.Add("");
            lines.Add("RECOMMENDATIONS:");
            foreach (var r in report.Recommendations)
                lines.Add("  - " + r);

            return string.Join(Environment.NewLine, lines);
        }
    }
}
