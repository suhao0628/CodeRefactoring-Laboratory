using System.ComponentModel.DataAnnotations;
using Delivery.Api.Model.Enum;

namespace Delivery.Api.Model.Dto
{
    public class DishDto
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public double Price{ get; set; }

        public string? Image { get; set; }

        public bool Vegetarian { get; set; }

        public double Rating { get; set; }

        public DishCategory Category { get; set; }

    }
}
