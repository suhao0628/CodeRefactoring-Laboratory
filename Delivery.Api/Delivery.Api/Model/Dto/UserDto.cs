using System.ComponentModel.DataAnnotations;
using Delivery.Api.Model.Enum;

namespace Delivery.Api.Model.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public string FullName { get; set; }

        [DataType(DataType.DateTime)]
        public string? BirthDate { get; set; }
 
        [Required]
        public Gender Gender { get; set; }

        public string? Address { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        //[DataType(DataType.PhoneNumber)]
        [Phone]
        public  string? PhoneNumber { get; set; }

    }
}
