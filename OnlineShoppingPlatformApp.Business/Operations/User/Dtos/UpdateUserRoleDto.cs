using OnlineShoppingPlatformApp.Data.Enums;

namespace OnlineShoppingPlatformApp.Business.Operations.User.Dtos
{
    public class UpdateUserRoleDto
    {
        public UserType NewRole { get; set; }
    }
}