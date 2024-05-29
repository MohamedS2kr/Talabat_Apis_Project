using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Services
{
    public class PaymentService : IPaymentServices
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService( IConfiguration configuration ,
                               IBasketRepository basketRepository ,
                               IUnitOfWork unitOfWork )
        {
            _configuration = configuration;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId)
        {
            //1. Secret Key -> StripConfiguration.ApiKEy فمحتاجين نجيبه وندية لل Strip وده الي من خلاله هكلم ال 
            StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];
            /* 2. Create PaymentIntent : 
             * Basket فبكده لازم نروح نجيب ال Basket بتاع ال Amount علي حسب ال  Create بيتعملها 
             * Amount = subtotal(عدد المنتج في سعره) + ShippingPrice (تكلفة الشحن) 
                                                       Cost علشان اعرف اجيب من جواه ال  DeliveryMethod وده هيخليني اروح اجيب ال
             *
             *
             */

            //2.1 Get Basket ->basketRepository وعلشان اجيبه لازم اجيبه من 
            var Basket =await _basketRepository.GetBasketAsync(BasketId);
            if (Basket is null) return null;

            // 2.2 Get DeliveryMethod ->Shipping بتاع ال  Cost علشان محتاجين نجيب ال
            var ShippingPrice = 0M; 
            if (Basket.DeliveryMethodId.HasValue)
            {
                var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(Basket.DeliveryMethodId.Value);
                ShippingPrice = DeliveryMethod.Cost;
            }
            if (Basket.Items.Count > 0)
            {
                foreach (var item in Basket.Items)
                {
                    var Product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    if(item.Price != Product.Price)
                        item.Price = Product.Price;
                }
            }
            
            // 2.3 Get SubTotal
            var SubTotal = Basket.Items.Sum(item => item.Price * item.Quantity);

            //2.4 Create PaymentIntent 
            //2.4.1 Create Object Service -> PaymentIntentService() --Create , Update دي هيا الي جواها كل الي انا عايزه زي  Serviceعلشان ال
            var Service = new PaymentIntentService();
            PaymentIntent paymentIntent;
            if (string.IsNullOrEmpty(Basket.PaymentIntentId))// Create
            {
                // Service.CreateAsync -> PaymentIntent و بترجعلي حاجه من نوع الPaymentIntentCreateOptions بتاخد من حاجه من نوه 
                var Option = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(SubTotal * 100 + ShippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() {"card"}
                };

                paymentIntent = await Service.CreateAsync(Option);
             
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else //Update
            {
                var Options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(SubTotal * 100 + ShippingPrice * 100),
                };
                paymentIntent = await Service.UpdateAsync(Basket.PaymentIntentId, Options); 
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = paymentIntent.ClientSecret;
            }
            await _basketRepository.UpdateBasketAsync(Basket);
            return Basket;
        }
    }
}
