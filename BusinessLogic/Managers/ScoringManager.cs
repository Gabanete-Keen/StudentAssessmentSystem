using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAssessmentSystem.BusinessLogic.Managers
{
    public class ScoringManager
    {
        /// Manager class for scoring and grading
        /// Handles automatic test scoring and grade calculation
            private readonly TestResultRepository _resultRepository;

            public ScoringManager()
            {
                _resultRepository = new TestResultRepository();
            }

            /// Scores a completed test
            /// Calculates raw score, percentage, and letter grade
            public void ScoreTest(TestResult testResult, List<Question> questions)
            {
                try
                {
                    int totalPointsEarned = 0;

                    // Score each answer
                    foreach (var answer in testResult.Answers)
                    {
                        // Find the corresponding question
                        Question question = questions.Find(q => q.QuestionId == answer.QuestionId);

                        if (question != null)
                        {
                            // Check if answer is correct
                            answer.IsCorrect = question.CheckAnswer(answer.SelectedChoiceId);

                            // Award points if correct
                            if (answer.IsCorrect)
                            {
                                answer.PointsEarned = question.PointValue;
                                totalPointsEarned += question.PointValue;
                            }
                            else
                            {
                                answer.PointsEarned = 0;
                            }
                        }
                    }

                    // Calculate score
                    testResult.RawScore = totalPointsEarned;
                    testResult.CalculatePercentage();
                    testResult.AssignLetterGrade();

                    // Determine if passed (based on passing score from test)
                    // This would need the Test object to get PassingScore
                    // For now, we'll use 60% as default
                    testResult.Passed = testResult.Percentage >= 60;

                    testResult.IsCompleted = true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error scoring test: {ex.Message}", ex);
                }
            }

     
            /// Calculates letter grade based on percentage
            /// Can be customized based on school's grading scale
            public string CalculateLetterGrade(decimal percentage)
            {
                if (percentage >= 90) return "A";
                if (percentage >= 80) return "B";
                if (percentage >= 70) return "C";
                if (percentage >= 60) return "D";
                return "F";
            }

      
            /// Saves test result to database
            public int SaveTestResult(TestResult result, out string errorMessage)
            {
                errorMessage = string.Empty;

                try
                {
                    int resultId = _resultRepository.Add(result);

                    if (resultId <= 0)
                        errorMessage = "Failed to save test result.";

                    return resultId;
                }
                catch (Exception ex)
                {
                    errorMessage = $"Error saving result: {ex.Message}";
                    return 0;
                }
            }

     
            /// Gets test results for a student
            public List<TestResult> GetStudentResults(int studentId)
            {
                try
                {
                    return _resultRepository.GetResultsByStudent(studentId);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error getting results: {ex.Message}", ex);
                }
            }
        }
    }
