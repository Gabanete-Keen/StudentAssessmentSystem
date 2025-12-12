using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Purpose: Generic repository interface
// SOLID: Interface Segregation Principle & Dependency Inversion
// Pattern: Repository Pattern for data access abstraction
namespace StudentAssessmentSystem.DataAccess.Repositories
{
    /// Generic repository interface for CRUD operations
    /// SOLID: Dependency Inversion - depend on abstraction, not concrete classes
        public interface IRepository<T> where T : class
        {
            // Create
            int Add(T entity);

            // Read
            T GetById(int id);
            List<T> GetAll();

            // Update
            bool Update(T entity);

            // Delete
            bool Delete(int id);
        }
    }

