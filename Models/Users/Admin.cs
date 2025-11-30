using StudentAssessmentSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Administrator user with full system access
// Connected to: User (inherits), AdminRepository
namespace StudentAssessmentSystem.Models.Users
{
    /// Administrator with full system privileges
    /// INHERITANCE: Extends User base class
    public class Admin : User
    {
        public int AccessLevel { get; set; } // 1=Basic, 2=Moderate, 3=Full

        public Admin()
        {
            Role = UserRole.Admin;
            AccessLevel = 1; // Default to basic admin
        }

        // POLYMORPHISM: Override base method
        public override string GetDashboardMessage()
        {
            return $"Admin Dashboard - Welcome, {FullName}! Access Level: {AccessLevel}";
        }

        // ABSTRACTION: Implement abstract method
        public override bool CanAccessAdminPanel()
        {
            return true; // Admins can always access admin panel
        }
    }
}

