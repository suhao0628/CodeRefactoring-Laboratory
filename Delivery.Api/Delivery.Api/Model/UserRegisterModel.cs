using System.ComponentModel.DataAnnotations;
using Delivery.Api.Model.Enum;

namespace Delivery.Api.Model
{
    public class UserRegisterModel
    {
        [Required]
        [MinLength(1)]
        public string FullName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        //[DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string? Address { get; set; }

        [DataType(DataType.DateTime)]
        public string? BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Phone]
        //[DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
    }
}
