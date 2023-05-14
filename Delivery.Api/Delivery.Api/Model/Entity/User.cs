using System.ComponentModel.DataAnnotations;
using Delivery.Api.Model.Enum;

namespace Delivery.Api.Model.Entity
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public string FullName { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(1)]
        public string Password { get; set; }

        public string? Address { get; set; }

        [DataType(DataType.DateTime)]
        public string? BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        //[DataType(DataType.PhoneNumber)]
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
