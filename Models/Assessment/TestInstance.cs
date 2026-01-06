using System;

namespace StudentAssessmentSystem.Models.Assessment
{
    /// Represents a specific instance/session of a test
    /// Teachers create test instances that students can take within a date range
    public class TestInstance
    {
        // ===== PRIMARY KEY =====
        public int InstanceId { get; set; }

        // ===== FOREIGN KEYS =====
        public int TestId { get; set; }
        public int TeacherId { get; set; }

        // ===== INSTANCE PROPERTIES =====
        public string InstanceTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // ===== NAVIGATION PROPERTIES =====
        public Test Test { get; set; }
        public string TestTitle { get; set; }
        public string TestDescription { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalPoints { get; set; }
        public double PassingScore { get; set; }
        public string SubjectName { get; set; }


        /// Constructor - Sets default values
        public TestInstance()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        /// Checks if the test instance is currently available for taking
        public bool IsAvailable()
        {
            DateTime now = DateTime.Now;
            return IsActive && now >= StartDate && now <= EndDate;
        }

        /// Checks if the test instance is currently open
        /// (Alias for IsAvailable for backward compatibility)
        public bool IsCurrentlyActive()
        {
            return IsAvailable();
        }

        /// Checks if the test instance has expired
        public bool IsExpired()
        {
            return DateTime.Now > EndDate;
        }

        
        /// Checks if the test period has ended
        /// (Alias for IsExpired for backward compatibility)
      
        public bool HasEnded()
        {
            return IsExpired();
        }

     
        /// Gets remaining time in minutes
       
        public int GetRemainingMinutes()
        {
            if (IsExpired())
                return 0;

            TimeSpan remaining = EndDate - DateTime.Now;
            return (int)remaining.TotalMinutes;
        }

        
        /// Validates test instance dates
        
        public bool ValidateDates(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (EndDate <= StartDate)
            {
                errorMessage = "End date must be after start date.";
                return false;
            }

            if (StartDate < DateTime.Now.AddMinutes(-5)) // Allow 5 minute buffer
            {
                errorMessage = "Start date cannot be in the past.";
                return false;
            }

            return true;
        }

       
        /// Gets status string for display
       
        public string GetStatusString()
        {
            if (!IsActive)
                return "Inactive";

            DateTime now = DateTime.Now;

            if (now < StartDate)
                return "Upcoming";

            if (now > EndDate)
                return "Expired";

            return "Active";
        }
    }
}
