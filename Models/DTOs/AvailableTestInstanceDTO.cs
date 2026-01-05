using System;

namespace StudentAssessmentSystem.Models.DTOs
{
    /// DTO for displaying available test instances to students
    public class AvailableTestInstanceDTO
    {
        // Test Instance Properties
        public int InstanceId { get; set; }
        public int TestId { get; set; }
        public int TeacherId { get; set; }
        public string InstanceTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Test Properties
        public string TestTitle { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalPoints { get; set; }
        public decimal PassingScore { get; set; }
        public string Instructions { get; set; }
        public bool RandomizeQuestions { get; set; }
        public bool RandomizeChoices { get; set; }

        // Related Data
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
    }
}
