using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Api.Model.Entity
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public DateTime DeliveryTime { get; set; }

        [Required]
        public DateTime OrderTime { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Required]
        public int Price { get; set; }

        //[Required]
        [MinLength(1)]
        public string? Address { get; set; }
    }
}
