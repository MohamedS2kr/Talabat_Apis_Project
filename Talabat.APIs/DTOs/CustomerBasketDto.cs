using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using Talabat.Core.Entities;

namespace Talabat.APIs.DTOs
{
    public class CustomerBasketDto
    {
        [Required]
        public string Id { get; set; }
        public List<BasketItem> Items { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public int? DeliveryMethodId { get; set; }

    }
}
