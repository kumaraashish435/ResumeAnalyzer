using System.Linq.Expressions;

namespace ResumeAnalyzer.Infrastructure.Data.Repositories;


/// Generic Repository Interface
/// Defines common database operations for any entity type
/// Provides a consistent interface for data access operations, following Repository pattern
/// Uses generics to work with any entity type and provides CRUD operations

/// <typeparam name="T">Entity type that inherits from a base class or interface</typeparam>
public interface IRepository<T> where T : class
{
    
    /// Get entity by ID
    
    Task<T?> GetByIdAsync(int id);
    
    
    /// Get all entities
    
    Task<IEnumerable<T>> GetAllAsync();
    
    
    /// Find entities matching a predicate condition
    
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    
    
    /// Get single entity matching predicate, or null if not found
    
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    
    
    /// Add a new entity to the database
    
    Task<T> AddAsync(T entity);
    
    
    /// Update an existing entity
    
    Task UpdateAsync(T entity);
    
    
    /// Delete an entity from the database
    
    Task DeleteAsync(T entity);
    
    
    /// Check if any entity exists matching the predicate
    
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    
    
    /// Count entities matching the predicate
    
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}

