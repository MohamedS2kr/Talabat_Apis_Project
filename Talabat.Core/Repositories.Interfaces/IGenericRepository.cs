using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Interfaces
{
    public interface IGenericRepository<T> where T: BaseEntity
    {
        #region WithOut Specification
        Task<IReadOnlyList<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);
        #endregion

        #region With Specification

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);

        Task<T?> GetByIdWithSpecAsync(ISpecifications<T> spec); 

        Task<int> GetCountAsync(ISpecifications<T> spec);
        #endregion

        Task AddAsync(T item);
    }
}
