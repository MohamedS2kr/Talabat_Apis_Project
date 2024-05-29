using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;

namespace Talabat.APIs.Controllers
{

    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;

        public BasketController(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }
        [HttpGet]//Get : /api/basket?id=11
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);
            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateOrCreateBasket(CustomerBasket basket)
        {
            var createAndUpdateBasket = await _basketRepository.UpdateBasketAsync(basket);
            if (createAndUpdateBasket is null) return BadRequest(new ApiResponse(400));
            return Ok(createAndUpdateBasket);
        }
        [HttpDelete]
        public async Task DeleteBasket(string id)
        {
           await _basketRepository.DeleteBasketAsync(id);
        }
    }
}
