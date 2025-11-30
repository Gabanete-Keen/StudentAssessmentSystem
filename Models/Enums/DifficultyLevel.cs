using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Purpose: Define difficulty levels for questions
// Connected to: Question class, ItemAnalyzer class
namespace StudentAssessmentSystem.Models.Enums
{
 /// Represents the difficulty level of a question
 /// Used in item analysis to categorize questions
    public enum DifficultyLevel
    {
        Easy,       // P-value > 0.75
        Average,    // P-value 0.25 - 0.75
        Difficult   // P-value < 0.25
    }
}
