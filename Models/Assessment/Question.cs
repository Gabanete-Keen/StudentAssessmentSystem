using StudentAssessmentSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// Abstract base class for all question types
/// ABSTRACTION: Defines common properties
/// POLYMORPHISM: Different question types implement CheckAnswer differently
namespace StudentAssessmentSystem.Models.Assessment
{
    public abstract class Question
    {
        public int QuestionId { get; set; }
        public int? TestId { get; set; }  // NULL if in question bank
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }  // "MultipleChoice", "TrueFalse", etc.
        public int PointValue { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public CognitiveLevel CognitiveLevel { get; set; }
        public string Topic { get; set; }
        public string Explanation { get; set; }  // Shown after answering
        public string ImagePath { get; set; }    // Optional image
        public int OrderNumber { get; set; }     // Order in test
        public int Points { get; set; }
        protected Question()
        {
            PointValue = 1; // Default 1 point
            DifficultyLevel = DifficultyLevel.Average;
            CognitiveLevel = CognitiveLevel.Remember;
        }

        /// ABSTRACTION: Each question type must implement how to check answers
        /// POLYMORPHISM: MultipleChoice, TrueFalse implement this differently

        /// <param name="studentAnswer">The answer provided by the student</param>
        /// <returns>True if answer is correct</returns>
        public abstract bool CheckAnswer(object studentAnswer);

        /// ABSTRACTION: Each question type must provide correct answer(s)
        public abstract object GetCorrectAnswer();

        /// Virtual method that can be overridden
        /// Validates if question data is complete
        public virtual bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(QuestionText) &&
                   PointValue > 0;
        }
    }
}