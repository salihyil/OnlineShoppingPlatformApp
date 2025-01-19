using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingPlatformApp.Business.Operations.User;
using OnlineShoppingPlatformApp.Business.Operations.User.Dtos;
using OnlineShoppingPlatformApp.Data.Enums;
using OnlineShoppingPlatformApp.WebApi.Filters;
using OnlineShoppingPlatformApp.WebApi.Jwt;
using OnlineShoppingPlatformApp.WebApi.Models;

namespace OnlineShoppingPlatformApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateModel]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private int UserId => int.Parse(HttpContext.User.Claims.First(c => c.Type == JwtCustomClaimNames.Id).Value);

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        // Amacı: Kullanıcı sadece kendi profil bilgilerini görebilir
        public async Task<IActionResult> GetProfile()
        {
            var result = await _userService.GetUserById(UserId);

            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        [HttpPut("profile")]
        // Amacı: Kullanıcı kendi profil bilgilerini güncelleyebilir
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request)
        {
            var updateUserRequest = new UpdateUserDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                BirthDate = request.BirthDate,
            };
            var result = await _userService.UpdateUser(UserId, updateUserRequest);

            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result);
        }

        [HttpPut("change-password")]
        // Amacı: Kullanıcı kendi şifresini değiştirebilir
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var changePasswordDto = new ChangePasswordDto
            {
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword
            };
            var result = await _userService.ChangeUserPassword(UserId, changePasswordDto);

            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result);
        }

        [HttpGet("all")]
        [Authorize(Roles = nameof(UserType.Admin))]
        // admin olmayan görmek isterse 401 hatası verir.
        // Amacı: Admin tüm kullanıcıların listesini görebilir
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsers();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = nameof(UserType.Admin))]
        // Amacı: Adminler ID vererek herhangi bir kullanıcının bilgilerine erişebilir
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserById(id);

            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        [HttpPatch("{id}/role")]
        [Authorize(Roles = nameof(UserType.Admin))]
        // Amacı: Admin kullanıcıların rollerini değiştirebilir (örn: normal kullanıcıyı admin yapabilir)
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleRequest request)
        {
            var updateUserRoleDto = new UpdateUserRoleDto
            {
                NewRole = request.NewRole
            };

            var result = await _userService.UpdateUserRole(id, updateUserRoleDto.NewRole);

            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserType.Admin))]
        // Amacı: Adminler kullanıcıları silebilir
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);

            if (!result.IsSuccess)
            {
                if (result.Message == "Yetkiniz yok.")
                    return StatusCode(403, result);

                return NotFound(result);
            }

            return Ok(result);
        }
    }
}