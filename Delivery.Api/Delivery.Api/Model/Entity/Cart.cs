using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Delivery.Api.Model.Entity;

namespace Delivery.Api.Model	
{
    public class Cart //Follow the order
    {
        [Key]
		public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        [ForeignKey("OrderId")]
		public Order Order { get; set; }

        public Guid DishId { get; set; }

        [ForeignKey("DishesId")]
        public Dish Dish { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int TotalPrice { get; set; }

        [Required]
        public int Amount { get; set; }

        public string? Image { get; set; }

    }
}
