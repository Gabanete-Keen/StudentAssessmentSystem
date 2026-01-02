using StudentAssessmentSystem.DataAccess;
using StudentAssessmentSystem.UI.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentAssessmentSystem
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable visual styles for better-looking controls
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ============================================
            // STEP 1: Initialize MySQL Database Connection
            // ============================================

            try
            {
                // TODO: CHANGE THESE VALUES TO MATCH YOUR MYSQL SETUP!
                DatabaseConnection.Initialize(
                    server: "localhost",              // MySQL server address
                    database: "StudentAssessmentDB",  // Your database name
                    username: "root",                 // Your MySQL username
                    password: "MySql_Admin#88",         // YOUR MYSQL PASSWORD HERE!
                    port: 3306                        // Default MySQL port
                );

                // Test if connection works
                if (DatabaseConnection.TestConnection(out string errorMessage))
                {
                    // SUCCESS: Database connection is working!

                    // Optional: Show success message (remove this later)
                    MessageBox.Show(
                        "Database connected successfully!\n\n" +
                        "Server: localhost\n" +
                        "Database: StudentAssessmentDB\n\n" +
                        "You can now login.",
                        "Connection Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // ============================================
                    // STEP 2: Show Login Form
                    // ============================================

                    // Start the application with LoginForm
                    Application.Run(new LoginForm());
                }
                else
                {
                    // FAILURE: Database connection failed

                    MessageBox.Show(
                        "Failed to connect to MySQL database!\n\n" +
                        "Error: " + errorMessage + "\n\n" +
                        "Please check:\n" +
                        "1. MySQL server is running (check services.msc)\n" +
                        "2. Database 'StudentAssessmentDB' exists\n" +
                        "3. Username and password are correct in Program.cs\n" +
                        "4. Port 3306 is correct\n\n" +
                        "The application will now close.",
                        "Database Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    // Exit application
                    return;
                }
            }
            catch (Exception ex)
            {
                // CRITICAL ERROR: Something went very wrong

                MessageBox.Show(
                    "A critical error occurred during startup:\n\n" +
                    ex.Message + "\n\n" +
                    "Stack Trace:\n" + ex.StackTrace,
                    "Critical Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
    

