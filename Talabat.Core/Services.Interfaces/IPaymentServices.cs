using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Services.Interfaces
{
    public interface IPaymentServices
    {
        //Function To Create Or Update Payment Intent
        Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId);
    }
}
