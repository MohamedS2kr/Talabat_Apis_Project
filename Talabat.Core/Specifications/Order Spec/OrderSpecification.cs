using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;

namespace Talabat.Core.Specifications.Order_Spec
{
    public class OrderSpecification : BaseSpecifications<Entities.Order.Order>
    {
        public OrderSpecification(string email) : base(O => O.BuyerEmail == email)
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DeliveryMethod);
            AddOrderByDesc(O => O.OrderDate);

        }
        public OrderSpecification(string email , int orderId) : base (O => O.BuyerEmail == email && O.Id == orderId)
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DeliveryMethod);

        }

    }
}
