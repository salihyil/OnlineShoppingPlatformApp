using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingPlatformApp.Business.Operations.User.Dtos;
using OnlineShoppingPlatformApp.Business.Types;
using OnlineShoppingPlatformApp.Data.Enums;

namespace OnlineShoppingPlatformApp.Business.Operations.User
{
    public interface IUserService
    {
        // ServiceMessage -> bir ekleme işlemi yapılacak bool / string / T? dönebilir.
        Task<ServiceMessage> AddUser(AddUserDto user); // async çünkü unit of work kullanıcaz.
        ServiceMessage<UserInfoDto> LoginUser(LoginUserDto user);
        Task<ServiceMessage<UserInfoDto>> GetUserById(int id);
        Task<ServiceMessage> UpdateUser(int id, UpdateUserDto update);
        Task<ServiceMessage> UpdateUserRole(int id, UserType role);
        Task<ServiceMessage> ChangeUserPassword(int id, ChangePasswordDto changePassword);
        Task<ServiceMessage<List<UserInfoDto>>> GetAllUsers();
        Task<ServiceMessage> DeleteUser(int id);
    }
}