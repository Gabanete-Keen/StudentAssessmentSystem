using StudentAssessmentSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// Abstract base class for all users in the system
/// ABSTRACTION: Common properties all users share
/// INHERITANCE: Admin, Teacher, Student inherit from this
namespace StudentAssessmentSystem.Models.Users

{
    public abstract class User
    {
        // ENCAPSULATION: Private fields with public properties
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // Computed property - DRY principle (Don't Repeat Yourself)
        public string FullName => $"{FirstName} {LastName}";

        // Constructor
        protected User()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
        }

        // POLYMORPHISM: Virtual method that can be overridden
        
        /// Gets role-specific dashboard data
        /// Each user type implements this differently
       
        public virtual string GetDashboardMessage()
        {
            return $"Welcome, {FullName}!";
        }

        // ABSTRACTION: Force child classes to implement
        
        /// Each user type must define their permissions
        
        public abstract bool CanAccessAdminPanel();
    }
}
