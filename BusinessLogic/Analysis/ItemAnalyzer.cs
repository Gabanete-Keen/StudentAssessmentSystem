using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Results;
using StudentAssessmentSystem.DataAccess.Repositories;

// Purpose: Performs item analysis calculations (Difficulty & Discrimination)
// Connected to: QuestionStatistics, TestResult, StudentAnswer
// Uses: MySQL database through repositories

namespace StudentAssessmentSystem.BusinessLogic.Analysis
{
    /// <summary>
    /// Simple item analyzer - calculates difficulty and discrimination
    /// </summary>
    public class ItemAnalyzer
    {
        // Repositories to get data from database
        private readonly TestResultRepository _resultRepository;
        private readonly StudentAnswerRepository _answerRepository;

        public ItemAnalyzer()
        {
            _resultRepository = new TestResultRepository();
            _answerRepository = new StudentAnswerRepository();
        }

        /// <summary>
        /// Analyzes a single question
        /// Returns statistics with difficulty and discrimination indices
        /// </summary>
        /// <param name="questionId">The question to analyze</param>
        /// <param name="testInstanceId">Which test administration</param>
        public QuestionStatistics AnalyzeQuestion(int questionId, int testInstanceId)
        {
            // STEP 1: Get all student answers for this question
            List<StudentAnswer> answers = _answerRepository.GetAnswersForQuestion(questionId, testInstanceId);

            // ✅ UPDATED: More flexible validation (removed 30-student requirement)
            if (answers.Count == 0)
            {
                throw new Exception($"No student responses found for this question.");
            }

            // STEP 2: Calculate Difficulty Index (P-value)
            // This works with any sample size > 0
            decimal difficultyIndex = CalculateDifficultyIndex(answers);

            // STEP 3: Calculate Discrimination Index (D-value)
            // This requires minimum 10 students for meaningful 27% groups
            decimal? discriminationIndex = null;
            if (answers.Count >= 10)
            {
                discriminationIndex = CalculateDiscriminationIndex(questionId, testInstanceId, answers);
            }

            // STEP 4: Create statistics object
            QuestionStatistics stats = new QuestionStatistics
            {
                QuestionId = questionId,
                TestInstanceId = testInstanceId,
                TotalAttempts = answers.Count,
                CorrectCount = answers.Count(a => a.IsCorrect),
                DifficultyIndex = difficultyIndex,
                DiscriminationIndex = discriminationIndex,
                LastCalculated = DateTime.Now
            };

            return stats;
        }

        /// <summary>
        /// Calculates Difficulty Index (P-value)
        /// Formula: P = (Number of correct answers) / (Total students)
        /// </summary>
        /// Simple interpretation:
        /// - P > 0.75 = Easy question (more than 75% got it right)
        /// - 0.25 to 0.75 = Average question (good!)
        /// - P < 0.25 = Difficult question (less than 25% got it right)
        public decimal CalculateDifficultyIndex(List<StudentAnswer> answers)
        {
            if (answers == null || answers.Count == 0)
                return 0;

            // Count how many students got it correct
            int correctCount = answers.Count(a => a.IsCorrect);

            // Total number of students
            int totalCount = answers.Count;

            // Calculate P-value
            decimal pValue = (decimal)correctCount / totalCount;

            // Round to 4 decimal places (e.g., 0.7500)
            return Math.Round(pValue, 4);
        }

        /// <summary>
        /// Calculates Discrimination Index (D-value)
        /// Measures if the question separates good students from weak students
        /// </summary>
        /// Formula: D = P(upper 27%) - P(lower 27%)
        /// 
        /// Simple interpretation:
        /// - D >= 0.40 = Excellent question (keep it!)
        /// - 0.30 to 0.39 = Good question
        /// - 0.20 to 0.29 = OK, but can improve
        /// - D < 0.20 = Poor question (needs revision)
        /// - D <= 0 = Bad question! (Good students got it wrong, weak students got it right)
        /// 
        /// Returns null if sample size is too small (< 10 students)
        public decimal? CalculateDiscriminationIndex(int questionId, int testInstanceId, List<StudentAnswer> answers)
        {
            // ✅ UPDATED: Return null instead of 0 for small samples
            if (answers == null || answers.Count < 10)
                return null;

            // STEP 1: Get all test results for this test
            List<TestResult> allResults = _resultRepository.GetResultsByInstance(testInstanceId);

            if (allResults.Count < 10)
                return null;

            // STEP 2: Sort students by total score (highest to lowest)
            allResults = allResults.OrderByDescending(r => r.RawScore).ToList();

            // STEP 3: Get upper 27% (top students) and lower 27% (bottom students)
            int groupSize = (int)Math.Ceiling(allResults.Count * 0.27);

            // Upper group = Best students (highest scores)
            List<TestResult> upperGroup = allResults.Take(groupSize).ToList();

            // Lower group = Weakest students (lowest scores)
            List<TestResult> lowerGroup = allResults
                .Skip(allResults.Count - groupSize)
                .Take(groupSize)
                .ToList();

            // STEP 4: Get answers for this question from each group
            var upperAnswers = answers.Where(a => upperGroup.Any(r => r.ResultId == a.ResultId)).ToList();
            var lowerAnswers = answers.Where(a => lowerGroup.Any(r => r.ResultId == a.ResultId)).ToList();

            if (upperAnswers.Count == 0 || lowerAnswers.Count == 0)
                return null;

            // STEP 5: Calculate percentage correct for each group
            decimal pUpper = (decimal)upperAnswers.Count(a => a.IsCorrect) / upperAnswers.Count;
            decimal pLower = (decimal)lowerAnswers.Count(a => a.IsCorrect) / lowerAnswers.Count;

            // STEP 6: Calculate discrimination index
            decimal discriminationIndex = pUpper - pLower;

            return Math.Round(discriminationIndex, 4);
        }

        /// <summary>
        /// Analyzes all questions in a test
        /// Returns list of statistics for each question
        /// </summary>
        public List<QuestionStatistics> AnalyzeAllQuestions(int testInstanceId)
        {
            List<QuestionStatistics> statisticsList = new List<QuestionStatistics>();

            // Get all questions for this test
            var questionRepository = new QuestionRepository();
            var test = new TestRepository().GetTestByInstanceId(testInstanceId);

            if (test?.Questions == null)
                return statisticsList;

            // Analyze each question
            foreach (var question in test.Questions)
            {
                try
                {
                    QuestionStatistics stats = AnalyzeQuestion(question.QuestionId, testInstanceId);
                    statisticsList.Add(stats);
                }
                catch (Exception ex)
                {
                    // Skip questions that can't be analyzed
                    Console.WriteLine($"Error analyzing question {question.QuestionId}: {ex.Message}");
                }
            }

            return statisticsList;
        }

        /// <summary>
        /// Gets simple text interpretation of difficulty
        /// </summary>
        public string GetDifficultyInterpretation(decimal pValue)
        {
            if (pValue > 0.75m)
                return "Easy - Most students got this correct";
            else if (pValue >= 0.50m)
                return "Moderate - Good difficulty level";
            else if (pValue >= 0.25m)
                return "Difficult - Many students missed this";
            else
                return "Very Difficult - Almost everyone missed this";
        }

        /// <summary>
        /// Gets simple text interpretation of discrimination
        /// </summary>
        public string GetDiscriminationInterpretation(decimal dValue)
        {
            if (dValue >= 0.40m)
                return "Excellent - Keep this question!";
            else if (dValue >= 0.30m)
                return "Good - Question works well";
            else if (dValue >= 0.20m)
                return "Marginal - Consider improving";
            else if (dValue >= 0.10m)
                return "Poor - Needs revision";
            else if (dValue > 0)
                return "Very Poor - Revise this question";
            else
                return "PROBLEM! Good students got it wrong. Check answer key!";
        }
    }
}
