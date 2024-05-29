using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Interfaces;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{

    public class ProductsController : BaseApiController
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;


        public ProductsController(IMapper mapper,IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            
        }


        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery] ProductSpecPramas Params) // FromQuery دي جايه من ال sort القيمة بتاعة ال
        {
            var spec = new ProductWithBrandAndTypeSpecification(Params);  //يا رايق  Include دي علشان ال 
            var Product = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            var countSpec = new ProductWithCountSpecifications(Params);
            var count = _unitOfWork.Repository<Product>().GetCountAsync(countSpec);

            var MappedProduct = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(Product);


            return Ok(new Pagination<ProductToReturnDto>(Params.PageSize, Params.PageIndex, await count, MappedProduct));
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProductById(int id)
        {
            var spec = new ProductWithBrandAndTypeSpecification(id); //يا رايق  Include دي علشان ال 
            var product = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(spec);
            if (product is null)
            {
                return NotFound(new ApiResponse(404));
            }
            var MappedProduct = _mapper.Map<Product, ProductToReturnDto>(product);
            return Ok(MappedProduct);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<ProductBrand>>> GetBrands()
        {
            var result = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ProductBrand>>> GetCategories()
        {
            var result = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();
            return Ok(result);
        }

        

    }
}
