using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAssessmentSystem.BusinessLogic.Managers
{
    /// Manager class for test operations
    /// Handles test creation, retrieval, and management
    public class TestManager
    {
        private readonly TestRepository _testRepository;
        private readonly QuestionRepository _questionRepository;

        public TestManager()
        {
            _testRepository = new TestRepository();
            _questionRepository = new QuestionRepository();
        }

 
        /// Creates a new test with all questions
        /// Validates test data before saving
        public int CreateTest(Test test, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // VALIDATION
                if (string.IsNullOrWhiteSpace(test.TestTitle))
                {
                    errorMessage = "Test title is required.";
                    return 0;
                }

                if (test.DurationMinutes <= 0)
                {
                    errorMessage = "Duration must be greater than 0.";
                    return 0;
                }

                if (test.Questions == null || test.Questions.Count == 0)
                {
                    errorMessage = "Test must have at least one question.";
                    return 0;
                }

                // Calculate total points
                test.CalculateTotalPoints();

                // Save test to database
                int testId = _testRepository.Add(test);

                if (testId > 0)
                {
                    // Save questions
                    foreach (var question in test.Questions)
                    {
                        question.TestId = testId;

                        if (question is MultipleChoiceQuestion mcQuestion)
                        {
                            _questionRepository.AddQuestion(mcQuestion);
                        }
                    }

                    return testId;
                }
                else
                {
                    errorMessage = "Failed to create test.";
                    return 0;
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error creating test: {ex.Message}";
                return 0;
            }
        }


        /// Gets test by ID with all questions
        public Test GetTestById(int testId)
        {
            try
            {
                Test test = _testRepository.GetById(testId);

                if (test != null)
                {
                    // Load questions
                    test.Questions = _questionRepository.GetQuestionsByTest(testId)
                        .ConvertAll(q => (Question)q);
                }

                return test;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting test: {ex.Message}", ex);
            }
        }

   
        /// Gets all tests created by a teacher
        public List<Test> GetTestsByTeacher(int teacherId)
        {
            try
            {
                return _testRepository.GetTestsByTeacher(teacherId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting tests: {ex.Message}", ex);
            }
        }

 
        /// Updates test information
        public bool UpdateTest(Test test, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // VALIDATION
                if (test.TestId <= 0)
                {
                    errorMessage = "Invalid test ID.";
                    return false;
                }

                test.CalculateTotalPoints();

                bool success = _testRepository.Update(test);

                if (!success)
                    errorMessage = "Failed to update test.";

                return success;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error updating test: {ex.Message}";
                return false;
            }
        }

 
        /// Deletes a test (soft delete)
        public bool DeleteTest(int testId, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                bool success = _testRepository.Delete(testId);

                if (!success)
                    errorMessage = "Failed to delete test.";

                return success;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error deleting test: {ex.Message}";
                return false;
            }
        }
    }
}