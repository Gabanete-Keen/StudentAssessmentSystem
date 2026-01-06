using MySql.Data.MySqlClient;
using StudentAssessmentSystem.Models.Academic;
using System;
using System.Collections.Generic;

namespace StudentAssessmentSystem.DataAccess.Repositories
{
    public class SectionRepository
    {
        public List<Section> GetAll()
        {
            List<Section> sections = new List<Section>();

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // ✅ SIMPLEST VERSION - Just get what we need
                    string query = "SELECT SectionId, SectionName FROM sections ORDER BY SectionName";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sections.Add(new Section
                                {
                                    SectionId = reader.GetInt32("SectionId"),
                                    SectionName = reader.GetString("SectionName"),
                                    IsActive = true  // ✅ Don't read from DB, just set true
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting sections: {ex.Message}", ex);
            }

            return sections;
        }
    }
}
