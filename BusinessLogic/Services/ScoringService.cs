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
            Console.WriteLine($"DEBUG: Scoring {result.Answers?.Count ?? 0} answers...");

            if (result.Answers == null || questions == null)
            {
                Console.WriteLine("DEBUG: Null answers/questions - skipping");
                return;
            }

            int totalPoints = 0, earnedPoints = 0;
            foreach (var answer in result.Answers)
            {
                var question = questions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);
                if (question == null)
                {
                    Console.WriteLine($"DEBUG: Question {answer.QuestionId} not found");
                    continue;
                }

                totalPoints += question.PointValue;
                int studentChoice = answer.SelectedChoiceId ?? 0;
                bool isCorrect = question.CheckAnswer(studentChoice);  // PROBLEM HERE

                Console.WriteLine($"Q{answer.QuestionId}: Student={studentChoice}, Correct?={isCorrect}, Points={question.PointValue}");

                if (isCorrect)
                {
                    earnedPoints += question.PointValue;
                }
            }

            result.RawScore = earnedPoints;
            result.TotalPoints = totalPoints;
            if (totalPoints > 0) result.Percentage = (decimal)earnedPoints / totalPoints * 100;
            result.AssignLetterGrade();
            result.Passed = result.Percentage >= 60;

            Console.WriteLine($"FINAL: {earnedPoints}/{totalPoints} = {result.Percentage}%");
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

