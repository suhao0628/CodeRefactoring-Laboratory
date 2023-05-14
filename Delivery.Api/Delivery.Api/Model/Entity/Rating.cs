using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Api.Model.Entity
{
    public class Rating
    {
        public Guid Id { get; set; }
        public Guid DishId { get; set; }
        [Required]
        [ForeignKey("DishId")]
        public Guid UserId { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public User User { get; set; }

        public Dish Dish { get; set; }
        public int RatingScore { get; set; }



    }
}
