using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Assessment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAssessmentSystem.BusinessLogic.Managers
{
    public class QuestionBankManager
    {
        /// Manager for question bank operations
        /// Handles reusable questions for multiple tests
            private readonly QuestionRepository _questionRepository;

            public QuestionBankManager()
            {
                _questionRepository = new QuestionRepository();
            }

        
            /// Adds question to question bank (not attached to specific test)
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

       
            /// Gets questions from test
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
        }
    }

