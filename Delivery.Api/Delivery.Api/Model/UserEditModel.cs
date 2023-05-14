using System.ComponentModel.DataAnnotations;
using Delivery.Api.Model.Enum;

namespace Delivery.Api.Model
{
    public class UserEditModel
    {
        [Required]
        [MinLength(1)]
        public string FullName { get; set; }

        [DataType(DataType.DateTime)]
        public string? BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public string? Address { get; set; }

        [Phone]
        //[DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

    }
}
