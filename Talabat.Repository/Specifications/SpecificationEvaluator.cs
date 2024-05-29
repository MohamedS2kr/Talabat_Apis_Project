using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public static class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        //Function to build Query 
        //_context.Products.Where(p=>p.Id == id).Include(P => P.Category).Include(P => P.Brand).FirstOrDefaultAsync();
    
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> spec)
        {
            var Query = inputQuery; //  _context.Set<T>
            if (spec.Criteria is not null)
            {
                Query = Query.Where(spec.Criteria);// _context.Products.Where(p=>p.Id == id).
            }
            // P => P.Category , P => P.Brand
    
            if(spec.OrderBy is not null)
            {
                Query = Query.OrderBy(spec.OrderBy);
            }
    
            if(spec.OrderByDesc is not null) 
            {
                Query = Query.OrderByDescending(spec.OrderByDesc); 
            }
    
            if (spec.IsPaginationEnabled)
            {
                Query = Query.Skip(spec.Skip).Take(spec.Take);
            }
    
            Query = spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));
    
            return Query;
        }
    }
    
}
