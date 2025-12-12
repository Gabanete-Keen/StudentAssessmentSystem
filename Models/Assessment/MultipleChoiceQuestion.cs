using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// Multiple choice question with multiple options
/// INHERITANCE: Extends Question abstract class
namespace StudentAssessmentSystem.Models.Assessment
{
    public class MultipleChoiceQuestion : Question
    {
        public List<QuestionChoice> Choices { get; set; }

        public MultipleChoiceQuestion()
        {
            QuestionType = "MultipleChoice";
            Choices = new List<QuestionChoice>();
        }

        /// <summary>
        /// POLYMORPHISM: Implements abstract method from Question
        /// Checks if the selected choice is correct
        /// </summary>
        /// <param name="studentAnswer">The ChoiceId selected by student</param>
        public override bool CheckAnswer(object studentAnswer)
        {
            if (studentAnswer == null || Choices == null) return false;

            // Cast to integer (ChoiceId)
            if (studentAnswer is int selectedChoiceId)
            {
                var selectedChoice = Choices.FirstOrDefault(c => c.ChoiceId == selectedChoiceId);
                return selectedChoice != null && selectedChoice.IsCorrect;
            }

            return false;
        }


        /// POLYMORPHISM: Implements abstract method
        /// Returns the correct choice(s)

        public override object GetCorrectAnswer()
        {
            return Choices?.FirstOrDefault(c => c.IsCorrect);
        }


        /// Override validation to include choices check

        public override bool IsValid()
        {
            return base.IsValid() &&
                   Choices != null &&
                   Choices.Count >= 2 &&  // At least 2 choices
                   Choices.Any(c => c.IsCorrect);  // At least 1 correct answer
        }


        /// Adds a choice to this question
        /// ENCAPSULATION: Controlled way to add choices
        public void AddChoice(string choiceText, bool isCorrect, int orderNumber = 0)
        {
            Choices.Add(new QuestionChoice
            {
                QuestionId = this.QuestionId,
                ChoiceText = choiceText,
                IsCorrect = isCorrect,
                OrderNumber = orderNumber > 0 ? orderNumber : Choices.Count + 1
            });
        }
    }
}