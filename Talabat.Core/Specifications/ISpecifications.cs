using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public interface ISpecifications<T> where T : BaseEntity
    {
        // _context.Products.Where(p=>p.Id == id).Include(P => P.Category).Include(P => P.Brand).FirstOrDefaultAsync();

        //Signature For Property For Where Condition [Where(p=>p.Id == id)]
        public Expression<Func<T,bool>> Criteria { get; set; }

        //Signature For Property For List Of Include [Include(P => P.Category).Include(P => P.Brand).FirstOrDefaultAsync()]

        public List<Expression<Func<T,object>>> Includes { get; set; }

        public Expression<Func<T,object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }

        public int  Take { get; set; }
        public int  Skip { get; set; }
        public bool IsPaginationEnabled { get; set; }
    }
}
