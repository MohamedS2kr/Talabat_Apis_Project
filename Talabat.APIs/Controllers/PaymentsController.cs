using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services.Interfaces;

namespace Talabat.APIs.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]

    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentServices _paymentServices;
        private readonly IMapper _mapper;

        public PaymentsController(IPaymentServices paymentServices ,
                                  IMapper mapper)
        {
            _paymentServices = paymentServices;
            _mapper = mapper;
        }

        // Create Or Update EndPoint
        [ProducesResponseType(typeof(CustomerBasketDto) , StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse) , StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var CustomerBasket =  await _paymentServices.CreateOrUpdatePaymentIntent(basketId);
            if (CustomerBasket is null) return BadRequest(new ApiResponse(400, "There is a Problem With Your Basket"));
            var ResultMapped = _mapper.Map<CustomerBasket,CustomerBasketDto>(CustomerBasket);
            return Ok(ResultMapped);
        }
    } 
}
