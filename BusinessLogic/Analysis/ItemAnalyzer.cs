using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using StudentAssessmentSystem.Models.Assessment;
using StudentAssessmentSystem.Models.Results;
using StudentAssessmentSystem.DataAccess;
// Purpose: Performs item analysis calculations for test questions
// Connected to: QuestionStatistics, TestResult, StudentAnswer
// Uses MySQL DataReader
namespace StudentAssessmentSystem.BusinessLogic.Analysis
{
    /// Analyzes test questions using Classical Test Theory
    /// Implements item analysis to evaluate question quality
    public class ItemAnalyzer
    {
        
    }
}
