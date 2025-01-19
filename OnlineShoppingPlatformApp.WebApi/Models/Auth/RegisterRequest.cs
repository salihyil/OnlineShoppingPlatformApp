using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatformApp.WebApi.Models
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

    }

}