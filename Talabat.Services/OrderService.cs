using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.Interfaces;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Core.Specifications.Order_Spec;
using Talabat.Repository.Repositories;

namespace Talabat.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;
        

        public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork)               
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int DeliveryMethodId, Address ShippingAddress)
        {
            //1.Get Basket From Basket Repo
            var Basket = await _basketRepository.GetBasketAsync(basketId);
            // 2.Get Selected Items at Basket From Product Repo
            var OrderItems =new List<OrderItem>();
            if(Basket?.Items.Count > 0)
            {
                foreach (var item in OrderItems)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    var ProductItemOrder = new ProductItemOrdered(product.Id, product.Name,product.PictureUrl);
                    var orderItem = new OrderItem(ProductItemOrder , item.price , item.Quantity);
                    OrderItems.Add(orderItem);
                }
            }
            // 3.Calculate SubTotal
            var SubTotal = OrderItems.Sum(item => item.price * item.Quantity);
            // 4.Get Delivery Method From DeliveryMethod Repo
            var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(DeliveryMethodId);
            // 5.Create Order
            var Order = new Order(buyerEmail , ShippingAddress , DeliveryMethod ,OrderItems,SubTotal);
            // 6.Add Order Locally
             await _unitOfWork.Repository<Order>().AddAsync(Order);
            // 7.Save Order To Database[ToDo]
            var result =  await _unitOfWork.CompleteAsync();
            if (result <= 0 ) return null; 
            return Order;
        }

        public async Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId)
        {
            var spec = new OrderSpecification(buyerEmail,orderId);
            var Orders = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);
            return Orders;
        }

        public Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecification(buyerEmail);
            var Orders = _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
            return Orders;
        }


    }
}
