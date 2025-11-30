using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Define user types in the system
// Connected to: User base class, authentication
namespace StudentAssessmentSystem.Models.Enums
{
    /// User roles for access control
    public enum UserRole
    {
        Admin,      // Full system access
        Teacher,    // Create tests, view analysis
        Student     // Take tests, view results
    }
}
