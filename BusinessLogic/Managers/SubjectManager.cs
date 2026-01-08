using StudentAssessmentSystem.DataAccess.Repositories;
using StudentAssessmentSystem.Models.Academic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentAssessmentSystem.BusinessLogic.Managers
{
    /// Manager for subject/course operations
    /// Business logic layer between UI and Data Access
    public class SubjectManager
    {
        private readonly SubjectRepository _subjectRepository;

        public SubjectManager()
        {
            _subjectRepository = new SubjectRepository();
        }

        /// Adds a new subject
        public int AddSubject(Subject subject, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // VALIDATION: Check if subject code already exists
                if (_subjectRepository.SubjectCodeExists(subject.SubjectCode))
                {
                    errorMessage = "Subject code already exists. Please use a different code.";
                    return 0;
                }

                // VALIDATION: Check required fields
                if (!subject.IsValid())
                {
                    errorMessage = "Please fill all required fields (Subject Code, Subject Name, Units).";
                    return 0;
                }

                // VALIDATION: Subject code format (optional but good practice)
                if (subject.SubjectCode.Length < 2)
                {
                    errorMessage = "Subject code must be at least 2 characters long.";
                    return 0;
                }

                // Add to database
                int subjectId = _subjectRepository.Add(subject);

                if (subjectId <= 0)
                {
                    errorMessage = "Failed to add subject.";
                }

                return subjectId;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error adding subject: {ex.Message}";
                return 0;
            }
        }

        /// Updates an existing subject
        public bool UpdateSubject(Subject subject, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // VALIDATION
                if (subject.SubjectId <= 0)
                {
                    errorMessage = "Invalid subject ID.";
                    return false;
                }

                // Check if subject code exists for other subjects
                if (_subjectRepository.SubjectCodeExists(subject.SubjectCode, subject.SubjectId))
                {
                    errorMessage = "Subject code already exists. Please use a different code.";
                    return false;
                }

                // VALIDATION: Check required fields
                if (!subject.IsValid())
                {
                    errorMessage = "Please fill all required fields (Subject Code, Subject Name, Units).";
                    return false;
                }

                bool success = _subjectRepository.Update(subject);

                if (!success)
                {
                    errorMessage = "Failed to update subject.";
                }

                return success;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error updating subject: {ex.Message}";
                return false;
            }
        }

        /// Deletes a subject (soft delete)
        public bool DeleteSubject(int subjectId, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                if (subjectId <= 0)
                {
                    errorMessage = "Invalid subject ID.";
                    return false;
                }

                // Optional: Check if subject is being used in sections or tests

                bool success = _subjectRepository.Delete(subjectId);

                if (!success)
                {
                    errorMessage = "Failed to delete subject.";
                }

                return success;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error deleting subject: {ex.Message}";
                return false;
            }
        }

        /// Gets a subject by ID
        public Subject GetSubjectById(int subjectId)
        {
            try
            {
                return _subjectRepository.GetById(subjectId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving subject: {ex.Message}", ex);
            }
        }

        /// Gets all active subjects
        public List<Subject> GetAllSubjects()
        {
            try
            {
                return _subjectRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting subjects: {ex.Message}", ex);
            }
        }

        /// Searches subjects by code or name
        public List<Subject> SearchSubjects(string searchText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchText))
                    return GetAllSubjects();

                List<Subject> allSubjects = _subjectRepository.GetAll();
                string search = searchText.ToLower();

                return allSubjects.Where(s =>
                    s.SubjectCode.ToLower().Contains(search) ||
                    s.SubjectName.ToLower().Contains(search) ||
                    (s.Description != null && s.Description.ToLower().Contains(search))
                ).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching subjects: {ex.Message}", ex);
            }
        }


        /// Gets subject statistics
        public SubjectStatistics GetStatistics()
        {
            try
            {
                List<Subject> subjects = GetAllSubjects();

                return new SubjectStatistics
                {
                    TotalSubjects = subjects.Count,
                    TotalUnits = subjects.Sum(s => s.Units),
                    AverageUnits = subjects.Count > 0 ? subjects.Average(s => s.Units) : 0
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting statistics: {ex.Message}", ex);
            }
        }

    }

   

    /// Statistics data transfer object
    public class SubjectStatistics
    {
        public int TotalSubjects { get; set; }
        public int TotalUnits { get; set; }
        public double AverageUnits { get; set; }
    }

   
}