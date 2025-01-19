using System.ComponentModel.DataAnnotations;
using OnlineShoppingPlatformApp.Data.Enums;

namespace OnlineShoppingPlatformApp.WebApi.Models
{
    public class UpdateUserRoleRequest
    {
        [Required(ErrorMessage = "User role is required")]
        [EnumDataType(typeof(UserType), ErrorMessage = "Invalid user role")]
        public UserType NewRole { get; set; }
    }
}