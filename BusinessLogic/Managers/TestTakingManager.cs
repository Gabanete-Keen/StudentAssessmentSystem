using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentAssessmentSystem.BusinessLogic.Managers
{
    /// <summary>
    /// Manages test-taking sessions
    /// SOLID: Single Responsibility - handles test session logic only
    /// </summary>
    public class TestTakingManager
    {
        private readonly TestRepository _testRepository;
        private readonly TestInstanceRepository _instanceRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly TestResultRepository _resultRepository;
        private readonly StudentAnswerRepository _answerRepository;

        public TestTakingManager()
        {
            _testRepository = new TestRepository();
            _instanceRepository = new TestInstanceRepository();
            _questionRepository = new QuestionRepository();
            _resultRepository = new TestResultRepository();
            _answerRepository = new StudentAnswerRepository();
        }

        /// <summary>
        /// Starts a new test session for a student
        /// Creates a TestResult record to track the session
        /// </summary>
        public TestResult StartTestSession(int studentId, int instanceId, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // Debug 1: Check parameters
                Console.WriteLine($"[DEBUG] Starting test session - StudentId: {studentId}, InstanceId: {instanceId}");

                // Check if student already has a result for this instance
                TestResult existingResult = _resultRepository.GetResultByStudentAndInstance(studentId, instanceId);

                if (existingResult != null && existingResult.IsCompleted)
                {
                    errorMessage = "You have already completed this test.";
                    return null;
                }

                // If there's an incomplete result, return it (resume session)
                if (existingResult != null && !existingResult.IsCompleted)
                {
                    return existingResult;
                }

                // Debug 2: Get test instance
                Console.WriteLine($"[DEBUG] Getting test instance with ID: {instanceId}");
                TestInstance instance = _instanceRepository.GetById(instanceId);

                if (instance == null)
                {
                    errorMessage = "Test instance not found.";
                    return null;
                }

                Console.WriteLine($"[DEBUG] Instance found - TestId: {instance.TestId}");

                // Debug 3: Get test details
                Console.WriteLine($"[DEBUG] Getting test with ID: {instance.TestId}");

                // ✅ ADD NULL CHECK BEFORE PARSING
                if (instance.TestId <= 0)
                {
                    errorMessage = $"Invalid Test ID: {instance.TestId}";
                    return null;
                }

                Test test = null;
                try
                {
                    test = _testRepository.GetById(instance.TestId);
                }
                catch (FormatException fex)
                {
                    errorMessage = $"Error getting test by ID: {fex.Message} (TestId was: {instance.TestId})";
                    return null;
                }

                if (test == null)
                {
                    errorMessage = $"Test with ID {instance.TestId} not found.";
                    return null;
                }

                Console.WriteLine($"[DEBUG] Test found - Title: {test.TestTitle}, Points: {test.TotalPoints}");

                // Create new test result
                TestResult result = new TestResult
                {
                    InstanceId = instanceId,
                    StudentId = studentId,
                    StartTime = DateTime.Now,
                    TotalPoints = test.TotalPoints,
                    IsCompleted = false
                };

                int resultId = _resultRepository.Add(result);

                if (resultId > 0)
                {
                    result.ResultId = resultId;
                    return result;
                }
                else
                {
                    errorMessage = "Failed to create test session.";
                    return null;
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error starting test: {ex.Message}";
                return null;
            }
        }


        /// <summary>
        /// Loads all questions for a test
        /// </summary>
        public List<MultipleChoiceQuestion> LoadTestQuestions(int testId, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                return _questionRepository.GetQuestionsByTest(testId);
            }
            catch (Exception ex)
            {
                errorMessage = $"Error loading questions: {ex.Message}";
                return new List<MultipleChoiceQuestion>();
            }
        }

        /// <summary>
        /// Saves a student's answer to a question
        /// Auto-scores the answer
        /// </summary>
        public bool SaveAnswer(int resultId, MultipleChoiceQuestion question, int selectedChoiceId, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // get the TestResult to find InstanceId
                TestResult result = _resultRepository.GetById(resultId);
                if (result == null)
                {
                    errorMessage = "Test result not found.";
                    return false;
                }

                // Check if answer is correct
                bool isCorrect = question.CheckAnswer(selectedChoiceId);
                int pointsEarned = isCorrect ? question.PointValue : 0;  

                // Create student answer
                StudentAnswer answer = new StudentAnswer
                {
                    InstanceId = result.InstanceId,  
                    ResultId = resultId,
                    QuestionId = question.QuestionId,
                    SelectedChoiceId = selectedChoiceId,
                    IsCorrect = isCorrect,
                    PointsEarned = pointsEarned,
                    TimeSpentSeconds = 0
                };

                int answerId = _answerRepository.Add(answer);
                return answerId > 0;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error saving answer: {ex.Message}";
                return false;
            }
        }

        /// Submits the test and calculates final score
        /// <summary>
        /// Submits the test and calculates final score
        /// </summary>
        public bool SubmitTest(int resultId, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // Get the result
                TestResult result = _resultRepository.GetById(resultId);

                if (result == null)
                {
                    errorMessage = "Test result not found.";
                    return false;
                }

                // Get all answers for this result
                List<StudentAnswer> answers = _answerRepository.GetAnswersByResult(resultId);

                // ✅ FIX: Get the actual questions to calculate total points
                TestInstance instance = _instanceRepository.GetById(result.InstanceId);
                if (instance == null)
                {
                    errorMessage = "Test instance not found.";
                    return false;
                }

                List<MultipleChoiceQuestion> questions = _questionRepository.GetQuestionsByTest(instance.TestId);
                if (questions == null || questions.Count == 0)
                {
                    errorMessage = "No questions found for this test.";
                    return false;
                }

                // Calculate total possible points from questions
                int totalPossiblePoints = 0;
                foreach (var question in questions)
                {
                    totalPossiblePoints += question.PointValue;
                }

                // Calculate total points earned from answers
                int totalPointsEarned = 0;
                foreach (var answer in answers)
                {
                    totalPointsEarned += answer.PointsEarned;
                }

                Console.WriteLine($"DEBUG SubmitTest: Earned={totalPointsEarned}, Total={totalPossiblePoints}");

                // Update result with calculated scores
                result.RawScore = totalPointsEarned;
                result.TotalPoints = totalPossiblePoints;  // ✅ FIX: Set correct total
                result.SubmitTime = DateTime.Now;
                result.IsCompleted = true;

                // Calculate percentage
                if (totalPossiblePoints > 0)
                {
                    result.Percentage = ((decimal)totalPointsEarned / totalPossiblePoints) * 100;
                }
                else
                {
                    result.Percentage = 0;
                }

                // Assign letter grade
                result.AssignLetterGrade();

                // Determine if passed
                Test test = _testRepository.GetById(instance.TestId);
                if (test != null)
                {
                    result.Passed = result.Percentage >= test.PassingScore;
                }
                else
                {
                    result.Passed = result.Percentage >= 60; // Default 60%
                }

                Console.WriteLine($"DEBUG Final: {result.RawScore}/{result.TotalPoints} = {result.Percentage}%, Grade={result.LetterGrade}, Passed={result.Passed}");

                // Save the updated result
                return _resultRepository.Update(result);
            }
            catch (Exception ex)
            {
                errorMessage = $"Error submitting test: {ex.Message}";
                Console.WriteLine($"ERROR in SubmitTest: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }


        /// Gets test instance with full test details
        public TestInstance GetTestInstanceWithDetails(int instanceId)
        {
            return _instanceRepository.GetById(instanceId);
        }
    }
}
