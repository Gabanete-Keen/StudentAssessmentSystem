using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// Represents a single answer choice in a multiple choice question
namespace StudentAssessmentSystem.Models.Assessment
{
    public class QuestionChoice
    {
        public int ChoiceId { get; set; }
        public int QuestionId { get; set; }
        public string ChoiceText { get; set; }
        public bool IsCorrect { get; set; }
        public int OrderNumber { get; set; }  // A, B, C, D ordering

        /// <summary>
        /// Gets the letter representation (A, B, C, D...)
        /// </summary>
        public string ChoiceLetter
        {
            get
            {
                if (OrderNumber <= 0) return "?";
                return ((char)('A' + OrderNumber - 1)).ToString();
            }
        }

        public override string ToString()
        {
            return $"{ChoiceLetter}. {ChoiceText}";
        }
    }
}
