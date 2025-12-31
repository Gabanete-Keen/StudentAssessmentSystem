using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

// Purpose: Centralized MySQL database connection management
// SOLID: Single Responsibility - Only handles database connections
// Connected to: All Repositories
namespace StudentAssessmentSystem.DataAccess
{
    /// Manages MySQL database connections for SQLyog
    /// Singleton pattern for connection string management
    public class DatabaseConnection
    {
        // ENCAPSULATION: Private connection string
        private static string _connectionString;

     
        /// Initializes the MySQL database connection string
        /// Call this once at application startup in Program.cs
      
        /// <param name="server">MySQL server address (usually "localhost" or "127.0.0.1")</param>
        /// <param name="database">Database name (StudentAssessmentDB)</param>
        /// <param name="username">MySQL username (default: root)</param>
        /// <param name="password">MySQL password</param>
        /// <param name="port">MySQL port (default: 3306)</param>
        public static void Initialize(
            string server,
            string database,
            string username,
            string password,
            uint port = 3306)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = server,              // e.g., "localhost" or "127.0.0.1"
                Database = database,          // "StudentAssessmentDB"
                UserID = username,            // e.g., "root"
                Password = password,          // Your MySQL password
                Port = port,                  // Default: 3306
                CharacterSet = "utf8mb4",     // Support for all characters

                // Performance settings
                Pooling = true,
                MinimumPoolSize = 0,
                MaximumPoolSize = 100,
                ConnectionTimeout = 30,

                // Additional settings
                AllowUserVariables = true,
                UseAffectedRows = false
            };

            _connectionString = builder.ConnectionString;
        }

        /// <summary>
        /// Gets a new MySQL database connection
        /// IMPORTANT: Always use 'using' statement when calling this!
        /// Example: using (var conn = DatabaseConnection.GetConnection()) { }
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection not initialized. Call Initialize() first in Program.cs");
            }

            return new MySqlConnection(_connectionString);
        }

        /// <summary>
        /// Tests if MySQL database connection works
        /// Returns true if successful, false otherwise
        /// </summary>
        public static bool TestConnection(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    // Test query
                    using (var cmd = new MySqlCommand("SELECT 1", conn))
                    {
                        cmd.ExecuteScalar();
                    }

                    return true;
                }
            }
            catch (MySqlException ex)
            {
                // MySQL specific errors
                errorMessage = $"MySQL Error: {ex.Message}\nError Code: {ex.Number}";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Connection Error: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Gets the current database name
        /// </summary>
        public static string GetDatabaseName()
        {
            if (string.IsNullOrEmpty(_connectionString))
                return "Not initialized";

            try
            {
                var builder = new MySqlConnectionStringBuilder(_connectionString);
                return builder.Database;
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Gets connection information for display (without password)
        /// </summary>
        public static string GetConnectionInfo()
        {
            if (string.IsNullOrEmpty(_connectionString))
                return "Connection not initialized";

            try
            {
                var builder = new MySqlConnectionStringBuilder(_connectionString);
                return $"Server: {builder.Server}:{builder.Port}\n" +
                       $"Database: {builder.Database}\n" +
                       $"User: {builder.UserID}";
            }
            catch
            {
                return "Unable to parse connection string";
            }
        }
    }
}
