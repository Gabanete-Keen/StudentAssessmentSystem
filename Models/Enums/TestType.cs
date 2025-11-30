using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Categorize different types of assessments
// Connected to: Test class
namespace StudentAssessmentSystem.Models.Enums
{
    /// Types of assessments in the system
    public enum TestType
    {
        Quiz,       // Short assessment
        Exam,       // Major assessment
        Diagnostic, // Pre-assessment
        Practice    // For student practice
    }
}
