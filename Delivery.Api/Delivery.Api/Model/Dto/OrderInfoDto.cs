using System.ComponentModel.DataAnnotations;
using Delivery.Api.Model.Enum;

namespace Delivery.Api.Model.Dto
{
    public class OrderInfoDto
    {
        public Guid Id { get; set; }

        [Required]
        public DateTime DeliveryTime { get; set; }

        [Required]
        public DateTime OrderTime { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Required]
        public double Price { get; set; }
    }
}
