using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithCountSpecifications : BaseSpecifications<Product>
    {
        public ProductWithCountSpecifications(ProductSpecPramas Params) 
            : base(P =>
                    (!Params.BrandId.HasValue || P.BrandId == Params.BrandId)
                    &&
                    (!Params.CategoryId.HasValue || P.CategoryId == Params.CategoryId))
        {

        }
        
    }
}
