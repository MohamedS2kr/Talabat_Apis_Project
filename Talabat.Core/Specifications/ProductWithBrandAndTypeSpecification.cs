using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithBrandAndTypeSpecification : BaseSpecifications<Product>
    {
        public ProductWithBrandAndTypeSpecification(ProductSpecPramas Params) 
            : base( 
                    P => 
                    (string.IsNullOrEmpty(Params.Search) || P.Name.ToLower().Contains(Params.Search))
                    &&
                    (!Params.BrandId.HasValue || P.BrandId == Params.BrandId) 
                    &&
                    (!Params.CategoryId.HasValue || P.CategoryId == Params.CategoryId) 
                  )
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);

            if (!string.IsNullOrEmpty(Params.sort))
            {
                switch (Params.sort)
                {
                    case "priceAsc":
                       AddOrderBy(P => P.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDesc(P => P.Price);
                        break;
                    default:
                        AddOrderBy(P => P.Name);
                        break;
                }
            }
            else
            {
                AddOrderBy(P => P.Name);
            }

            // Total 1000
            // PageIndex = 9 
            // PageSize = 50
            // Skip = PageSize * (PageIndex - 1) = 8*50 = 400
            // Take = 50
            ApplyPagination(Params.PageSize * (Params.PageIndex - 1),Params.PageSize);
        }

        public ProductWithBrandAndTypeSpecification(int id) : base(p => p.Id ==  id)
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);
        }
    }
}
