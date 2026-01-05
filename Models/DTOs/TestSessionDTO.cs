using System;
using System.Collections.Generic;

namespace StudentAssessmentSystem.Models.DTOs
{
    /// DTO for a complete test-taking session
    /// Contains test instance details and all questions with choices
    public class TestSessionDTO
    {
        // Test Instance Info
        public int InstanceId { get; set; }
        public int TestId { get; set; }
        public string InstanceTitle { get; set; }
        public string TestTitle { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalPoints { get; set; }
        public int TotalQuestions { get; set; }

        // Questions for this test
        public List<TestQuestionDTO> Questions { get; set; }

        public TestSessionDTO()
        {
            Questions = new List<TestQuestionDTO>();
        }
    }

    /// DTO for a single question in a test session
    public class TestQuestionDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int Points { get; set; }
        public int OrderNumber { get; set; }
        public string DifficultyLevel { get; set; }
        public string CognitiveLevel { get; set; }

        // Multiple choice options
        public List<QuestionChoiceDTO> Choices { get; set; }

        // Student's selected answer (to be filled during test)
        public int? SelectedChoiceId { get; set; }
        public bool IsAnswered { get; set; }

        public TestQuestionDTO()
        {
            Choices = new List<QuestionChoiceDTO>();
        }
    }

    /// DTO for a single choice option
    public class QuestionChoiceDTO
    {
        public int ChoiceId { get; set; }
        public string ChoiceText { get; set; }
        public int ChoiceOrder { get; set; }
    }
}
