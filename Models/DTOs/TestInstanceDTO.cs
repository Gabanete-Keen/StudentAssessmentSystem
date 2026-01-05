using System;

namespace StudentAssessmentSystem.Models.DTOs
{
    /// Data Transfer Object for displaying test instance information to students
    /// Combines TestInstance data with Test details for easy display
    /// SOLID: Single Responsibility - Only for transferring combined data
    public class TestInstanceDTO
    {
        // TestInstance properties
        public int InstanceId { get; set; }
        public int TestId { get; set; }
        public int TeacherId { get; set; }
        public string InstanceTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Test properties (from Tests table)
        public string TestTitle { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalPoints { get; set; }
        public decimal PassingScore { get; set; }
        public string Instructions { get; set; }
        public bool RandomizeQuestions { get; set; }
        public bool RandomizeChoices { get; set; }

        // Additional information for display
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }

        /// Gets formatted display text for ListBox/ComboBox
        public string DisplayText
        {
            get
            {
                string timeInfo = $"{StartDate:MMM dd, yyyy h:mm tt} - {EndDate:h:mm tt}";
                return $"{InstanceTitle} | {TestTitle} | {SubjectName} | {timeInfo}";
            }
        }

        /// Checks if the test instance is currently available (within time window)
        public bool IsCurrentlyAvailable
        {
            get
            {
                DateTime now = DateTime.Now;
                return IsActive && now >= StartDate && now <= EndDate;
            }
        }

        /// Gets time remaining in minutes
        public int MinutesUntilEnd
        {
            get
            {
                TimeSpan remaining = EndDate - DateTime.Now;
                return remaining.TotalMinutes > 0 ? (int)remaining.TotalMinutes : 0;
            }
        }

        /// Gets formatted duration text
        public string DurationText
        {
            get
            {
                if (DurationMinutes < 60)
                    return $"{DurationMinutes} minutes";
                else
                {
                    int hours = DurationMinutes / 60;
                    int minutes = DurationMinutes % 60;
                    return minutes > 0 ? $"{hours}h {minutes}m" : $"{hours} hour(s)";
                }
            }
        }
    }
}
