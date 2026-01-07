using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentAssessmentSystem.BusinessLogic.Managers
{
    /// <summary>
    /// Manager for question bank operations
    /// Handles reusable questions for multiple tests
    /// </summary>
    public class QuestionBankManager
    {
        private readonly QuestionRepository _questionRepository;

        public QuestionBankManager()
        {
            _questionRepository = new QuestionRepository();
        }

        #region Add/Update/Delete Operations

        /// <summary>
        /// Adds question to question bank (not attached to specific test)
        /// </summary>
        public int AddToQuestionBank(MultipleChoiceQuestion question, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // VALIDATION
                if (string.IsNullOrWhiteSpace(question.QuestionText))
                {
                    errorMessage = "Question text is required.";
                    return 0;
                }

                if (question.Choices == null || question.Choices.Count < 2)
                {
                    errorMessage = "Question must have at least 2 choices.";
                    return 0;
                }

                if (!question.Choices.Exists(c => c.IsCorrect))
                {
                    errorMessage = "Question must have at least one correct answer.";
                    return 0;
                }

                if (question.PointValue <= 0)
                {
                    errorMessage = "Point value must be greater than 0.";
                    return 0;
                }

                // Set TestId to null for question bank
                question.TestId = null;

                int questionId = _questionRepository.AddQuestion(question);

                if (questionId <= 0)
                    errorMessage = "Failed to add question to bank.";

                return questionId;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error adding question: {ex.Message}";
                return 0;
            }
        }

        /// <summary>
        /// Updates an existing question in the question bank
        /// </summary>
        public bool UpdateQuestion(MultipleChoiceQuestion question, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                // VALIDATION
                if (string.IsNullOrWhiteSpace(question.QuestionText))
                {
                    errorMessage = "Question text is required.";
                    return false;
                }

                if (question.Choices == null || question.Choices.Count < 2)
                {
                    errorMessage = "Question must have at least 2 choices.";
                    return false;
                }

                if (!question.Choices.Exists(c => c.IsCorrect))
                {
                    errorMessage = "Question must have at least one correct answer.";
                    return false;
                }

                if (question.PointValue <= 0)
                {
                    errorMessage = "Point value must be greater than 0.";
                    return false;
                }

                bool result = _questionRepository.UpdateQuestion(question.QuestionId, question);

                if (!result)
                {
                    errorMessage = "Failed to update question.";
                }

                return result;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error updating question: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Deletes a question from the question bank
        /// </summary>
        public bool DeleteQuestion(int questionId, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                if (questionId <= 0)
                {
                    errorMessage = "Invalid question ID.";
                    return false;
                }

                // Check if question is being used in any tests
                var question = _questionRepository.GetQuestionById(questionId);
                if (question != null && question.TestId.HasValue)
                {
                    errorMessage = "Cannot delete a question that is assigned to a test. Remove it from the test first.";
                    return false;
                }

                return _questionRepository.DeleteQuestion(questionId);
            }
            catch (Exception ex)
            {
                errorMessage = $"Error deleting question: {ex.Message}";
                return false;
            }
        }

        #endregion

        #region Retrieval Operations

        /// <summary>
        /// Gets a single question by ID
        /// </summary>
        public MultipleChoiceQuestion GetQuestionById(int questionId)
        {
            try
            {
                return _questionRepository.GetQuestionById(questionId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving question: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all questions from the question bank (TestId is NULL)
        /// </summary>
        public List<MultipleChoiceQuestion> GetAllQuestionBankQuestions()
        {
            try
            {
                return _questionRepository.GetQuestionBankQuestions();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting question bank: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets questions by test (for test management, not question bank)
        /// </summary>
        public List<MultipleChoiceQuestion> GetQuestionsByTest(int testId)
        {
            try
            {
                return _questionRepository.GetQuestionsByTest(testId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting questions: {ex.Message}", ex);
            }
        }

        #endregion

        #region Search and Filter Operations

        /// <summary>
        /// Searches questions in the bank by multiple criteria
        /// </summary>
        public List<MultipleChoiceQuestion> SearchQuestions(
            string searchText = null,
            string topic = null,
            DifficultyLevel? difficulty = null,
            CognitiveLevel? cognitiveLevel = null)
        {
            try
            {
                var allQuestions = _questionRepository.GetQuestionBankQuestions();

                // Apply filters
                var filtered = allQuestions.Where(q =>
                {
                    // Search in question text
                    bool matchesText = string.IsNullOrWhiteSpace(searchText) ||
                                     q.QuestionText.ToLower().Contains(searchText.ToLower());

                    // Filter by topic
                    bool matchesTopic = string.IsNullOrWhiteSpace(topic) ||
                                      (q.Topic != null && q.Topic.Equals(topic, StringComparison.OrdinalIgnoreCase));

                    // Filter by difficulty
                    bool matchesDifficulty = !difficulty.HasValue ||
                                           q.DifficultyLevel == difficulty.Value;

                    // Filter by cognitive level
                    bool matchesCognitive = !cognitiveLevel.HasValue ||
                                          q.CognitiveLevel == cognitiveLevel.Value;

                    return matchesText && matchesTopic && matchesDifficulty && matchesCognitive;
                }).ToList();

                return filtered;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching questions: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all unique topics from the question bank
        /// </summary>
        public List<string> GetAllTopics()
        {
            try
            {
                var questions = _questionRepository.GetQuestionBankQuestions();
                return questions
                    .Where(q => !string.IsNullOrWhiteSpace(q.Topic))
                    .Select(q => q.Topic)
                    .Distinct()
                    .OrderBy(t => t)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting topics: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets questions by specific topic
        /// </summary>
        public List<MultipleChoiceQuestion> GetQuestionsByTopic(string topic)
        {
            try
            {
                var questions = _questionRepository.GetQuestionBankQuestions();
                return questions
                    .Where(q => q.Topic != null && q.Topic.Equals(topic, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting questions by topic: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets questions by difficulty level
        /// </summary>
        public List<MultipleChoiceQuestion> GetQuestionsByDifficulty(DifficultyLevel difficulty)
        {
            try
            {
                var questions = _questionRepository.GetQuestionBankQuestions();
                return questions.Where(q => q.DifficultyLevel == difficulty).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting questions by difficulty: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets questions by cognitive level
        /// </summary>
        public List<MultipleChoiceQuestion> GetQuestionsByCognitiveLevel(CognitiveLevel cognitiveLevel)
        {
            try
            {
                var questions = _questionRepository.GetQuestionBankQuestions();
                return questions.Where(q => q.CognitiveLevel == cognitiveLevel).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting questions by cognitive level: {ex.Message}", ex);
            }
        }

        #endregion

        #region Question Bank Statistics

        /// <summary>
        /// Gets statistics about the question bank
        /// </summary>
        public QuestionBankStatistics GetQuestionBankStatistics()
        {
            try
            {
                var questions = _questionRepository.GetQuestionBankQuestions();

                var stats = new QuestionBankStatistics
                {
                    TotalQuestions = questions.Count,
                    EasyQuestions = questions.Count(q => q.DifficultyLevel == DifficultyLevel.Easy),
                    AverageQuestions = questions.Count(q => q.DifficultyLevel == DifficultyLevel.Average),
                    DifficultQuestions = questions.Count(q => q.DifficultyLevel == DifficultyLevel.Difficult),

                    RememberLevel = questions.Count(q => q.CognitiveLevel == CognitiveLevel.Remember),
                    UnderstandLevel = questions.Count(q => q.CognitiveLevel == CognitiveLevel.Understand),
                    ApplyLevel = questions.Count(q => q.CognitiveLevel == CognitiveLevel.Apply),
                    AnalyzeLevel = questions.Count(q => q.CognitiveLevel == CognitiveLevel.Analyze),
                    EvaluateLevel = questions.Count(q => q.CognitiveLevel == CognitiveLevel.Evaluate),
                    CreateLevel = questions.Count(q => q.CognitiveLevel == CognitiveLevel.Create),

                    TopicsCount = questions.Where(q => !string.IsNullOrWhiteSpace(q.Topic))
                                         .Select(q => q.Topic).Distinct().Count()
                };

                return stats;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting statistics: {ex.Message}", ex);
            }
        }

        #endregion

        #region Question Copy/Clone Operations

        /// <summary>
        /// Copies a question from the question bank to a specific test
        /// Creates a new question instance associated with the test
        /// </summary>
        public int CopyQuestionToTest(int questionId, int testId, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // Get the original question
                var originalQuestion = _questionRepository.GetQuestionById(questionId);

                if (originalQuestion == null)
                {
                    errorMessage = "Question not found.";
                    return 0;
                }

                // Create a new question instance for the test
                var copiedQuestion = new MultipleChoiceQuestion
                {
                    TestId = testId,  // Assign to test
                    QuestionText = originalQuestion.QuestionText,
                    QuestionType = originalQuestion.QuestionType,
                    PointValue = originalQuestion.PointValue,
                    DifficultyLevel = originalQuestion.DifficultyLevel,
                    CognitiveLevel = originalQuestion.CognitiveLevel,
                    Topic = originalQuestion.Topic,
                    Explanation = originalQuestion.Explanation,
                    ImagePath = originalQuestion.ImagePath,
                    Choices = new List<QuestionChoice>()
                };

                // Copy choices
                foreach (var choice in originalQuestion.Choices)
                {
                    copiedQuestion.Choices.Add(new QuestionChoice
                    {
                        ChoiceText = choice.ChoiceText,
                        IsCorrect = choice.IsCorrect,
                        OrderNumber = choice.OrderNumber
                    });
                }

                // Add the copied question
                int newQuestionId = _questionRepository.AddQuestion(copiedQuestion);

                if (newQuestionId <= 0)
                {
                    errorMessage = "Failed to copy question to test.";
                }

                return newQuestionId;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error copying question: {ex.Message}";
                return 0;
            }
        }

        /// <summary>
        /// Duplicates a question in the question bank (creates a copy)
        /// </summary>
        public int DuplicateQuestion(int questionId, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var originalQuestion = _questionRepository.GetQuestionById(questionId);

                if (originalQuestion == null)
                {
                    errorMessage = "Question not found.";
                    return 0;
                }

                // Create a duplicate with modified text
                var duplicatedQuestion = new MultipleChoiceQuestion
                {
                    TestId = null,  // Keep in question bank
                    QuestionText = originalQuestion.QuestionText + " (Copy)",
                    QuestionType = originalQuestion.QuestionType,
                    PointValue = originalQuestion.PointValue,
                    DifficultyLevel = originalQuestion.DifficultyLevel,
                    CognitiveLevel = originalQuestion.CognitiveLevel,
                    Topic = originalQuestion.Topic,
                    Explanation = originalQuestion.Explanation,
                    ImagePath = originalQuestion.ImagePath,
                    Choices = new List<QuestionChoice>()
                };

                // Copy choices
                foreach (var choice in originalQuestion.Choices)
                {
                    duplicatedQuestion.Choices.Add(new QuestionChoice
                    {
                        ChoiceText = choice.ChoiceText,
                        IsCorrect = choice.IsCorrect,
                        OrderNumber = choice.OrderNumber
                    });
                }

                return AddToQuestionBank(duplicatedQuestion, out errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage = $"Error duplicating question: {ex.Message}";
                return 0;
            }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates if a question can be deleted
        /// </summary>
        public bool CanDeleteQuestion(int questionId, out string message)
        {
            message = string.Empty;

            try
            {
                var question = _questionRepository.GetQuestionById(questionId);

                if (question == null)
                {
                    message = "Question not found.";
                    return false;
                }

                if (question.TestId.HasValue)
                {
                    message = "This question is assigned to a test and cannot be deleted from the question bank.";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                message = $"Error validating deletion: {ex.Message}";
                return false;
            }
        }

        #endregion
    }

    #region Support Classes

    /// <summary>
    /// Statistics summary for the question bank
    /// </summary>
    public class QuestionBankStatistics
    {
        public int TotalQuestions { get; set; }

        // By Difficulty
        public int EasyQuestions { get; set; }
        public int AverageQuestions { get; set; }
        public int DifficultQuestions { get; set; }

        // By Cognitive Level
        public int RememberLevel { get; set; }
        public int UnderstandLevel { get; set; }
        public int ApplyLevel { get; set; }
        public int AnalyzeLevel { get; set; }
        public int EvaluateLevel { get; set; }
        public int CreateLevel { get; set; }

        // Other
        public int TopicsCount { get; set; }

        // Calculated properties
        public double EasyPercentage => TotalQuestions > 0 ? (EasyQuestions * 100.0 / TotalQuestions) : 0;
        public double AveragePercentage => TotalQuestions > 0 ? (AverageQuestions * 100.0 / TotalQuestions) : 0;
        public double DifficultPercentage => TotalQuestions > 0 ? (DifficultQuestions * 100.0 / TotalQuestions) : 0;
    }

    #endregion
}