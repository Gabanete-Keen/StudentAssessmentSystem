using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Results;
// Purpose: Handles automatic scoring of tests
// Connected to: TestResult, StudentAnswer, Question
namespace StudentAssessmentSystem.BusinessLogic.Services
{
    // Simple scoring service - automatically scores tests
    public class ScoringService
    {
        /// Scores a single question answer
        /// Returns points earned
        public int ScoreAnswer(Question question, object studentAnswer)
        {
            // Check if answer is correct
            bool isCorrect = question.CheckAnswer(studentAnswer);

            // If correct, give full points
            if (isCorrect)
                return question.PointValue;
            else
                return 0;
        }

        /// Scores all answers in a test result
        /// Updates the TestResult object with calculated scores
        public void ScoreTestResult(TestResult result, List<Question> questions)
        {
            if (result.Answers == null || questions == null)
                return;

            int totalPoints = 0;
            int earnedPoints = 0;

            // Score each answer
            foreach (var answer in result.Answers)
            {
                // Find the question
                Question question = questions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);
                if (question == null)
                    continue;

                // Add to total possible points
                totalPoints += question.PointValue;

                // Check if answer is correct
                bool isCorrect = question.CheckAnswer(answer.SelectedChoiceId);

                if (isCorrect)
                {
                    answer.IsCorrect = true;
                    answer.PointsEarned = question.PointValue;
                    earnedPoints += question.PointValue;
                }
                else
                {
                    answer.IsCorrect = false;
                    answer.PointsEarned = 0;
                }
            }

            // Update test result
            result.RawScore = earnedPoints;
            result.TotalPoints = totalPoints;

            // Calculate percentage
            if (totalPoints > 0)
                result.Percentage = ((decimal)earnedPoints / totalPoints) * 100;
            else
                result.Percentage = 0;

            // Assign letter grade
            result.LetterGrade = CalculateLetterGrade(result.Percentage);

            // Check if passed
            result.Passed = result.Percentage >= 60; // 60% passing
        }

        
        /// Calculates letter grade from percentage
        public string CalculateLetterGrade(decimal percentage)
        {
            if (percentage >= 90)
                return "A";
            else if (percentage >= 80)
                return "B";
            else if (percentage >= 70)
                return "C";
            else if (percentage >= 60)
                return "D";
            else
                return "F";
        }

     
        /// Calculates grade description
        public string GetGradeDescription(string letterGrade)
        {
            switch (letterGrade)
            {
                case "A":
                    return "Excellent";
                case "B":
                    return "Very Good";
                case "C":
                    return "Good";
                case "D":
                    return "Passed";
                case "F":
                    return "Failed";
                default:
                    return "Unknown";
            }
        }

       
        /// Checks if score is passing
        public bool IsPassing(decimal percentage, decimal passingScore)
        {
            return percentage >= passingScore;
        }
    }
}

