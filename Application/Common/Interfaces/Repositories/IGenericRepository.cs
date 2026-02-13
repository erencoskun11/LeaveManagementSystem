using Domain.Common; // BaseEntity'nin olduğu namespace (Bunu eklemeyi unutma!)
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Application.Common.Interfaces.Repositories
{
    // DÜZELTME: 'where T : class' YERİNE 'where T : BaseEntity' YAZIYORUZ
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        // Best Practice Metotları
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool trackChanges = true);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, bool trackChanges = true);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
    }
}