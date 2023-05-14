using System.ComponentModel.DataAnnotations;

namespace Delivery.Api.Model
{
    public class LoginCredentials
    {
        [Required]
        [MinLength(1)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [MinLength(1)]
        public string Password { get; set; }
    }
}
