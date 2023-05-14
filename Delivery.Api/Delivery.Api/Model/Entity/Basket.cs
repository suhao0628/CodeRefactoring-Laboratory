using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Api.Model.Entity
{
    public class Basket //Follow the user
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public Guid DishId { get; set; }

        [ForeignKey("DishesId")]
        public Dish Dish { get; set; }

        [Required]
        public int Amount { get; set; }
    }
}
