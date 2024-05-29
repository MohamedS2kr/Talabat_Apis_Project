using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDbContext _context;

        public GenericRepository(StoreDbContext context)
        {
            _context = context;
        }

        #region WithOut Specification

        public async Task<T?> GetByIdAsync(int id)
        {
            if (typeof(T) == typeof(Product))
            {
                return await _context.Products.Where(p => p.Id == id).Include(P => P.Category).Include(P => P.Brand).FirstOrDefaultAsync() as T;
            }
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
            {
                return (IReadOnlyList<T>)await _context.Products.Include(P => P.Category).Include(P => P.Brand).ToListAsync();
            }
            return await _context.Set<T>().ToListAsync();
        }

        #endregion

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
           return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<T?> GetByIdWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<int> GetCountAsync(ISpecifications<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }
        
        private IQueryable<T> ApplySpecification (ISpecifications<T> Spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>(),Spec);//(Input Query , Specification : بتاخدي مني حاجتين )
            // GetQuery دي اسم الكلاس و مستخدم منه الفينكشن الي اسمها 
        }

        public async Task AddAsync(T item)
        {
           await  _context.Set<T>().AddAsync(item);
        }
    }
}
 