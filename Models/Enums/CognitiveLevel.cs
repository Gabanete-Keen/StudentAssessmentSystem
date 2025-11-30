using StudentAssessmentSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

// Purpose: Bloom's Taxonomy cognitive levels
// Connected to: Question class for educational standards
namespace StudentAssessmentSystem.Models.Enums
{
    
    /// Helps teachers create balanced assessments
    public enum CognitiveLevel
    {
        Remember,   // Recall facts
        Understand, // Explain ideas
        Apply,      // Use information in new situations
        Analyze,    // Draw connections
        Evaluate,   // Justify decisions
        Create      // Produce new work
    }
}
