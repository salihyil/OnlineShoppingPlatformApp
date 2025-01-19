using OnlineShoppingPlatformApp.Business.Operations.User;
using OnlineShoppingPlatformApp.Business.Operations.User.Dtos;
using OnlineShoppingPlatformApp.WebApi.Jwt;
using OnlineShoppingPlatformApp.WebApi.Models;
using OnlineShoppingPlatformApp.WebApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShoppingPlatformApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateModel]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            // builder.Services.AddScoped<IUserService, UserManager>(); sayesinde 
            // userService parametresine UserManager instance'ı gelecek.
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // burda verilerimiz var bunları nasıl taşıcaz? methodlar aracılığıyla. 
            // Bunu methodun parametresi yapıcaz. 
            // veriler böylece diğer katmana gidebilecek.
            var addUserDto = new AddUserDto
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                PhoneNumber = request.PhoneNumber
            };

            // dependency injection ile user service'e erişim sağladık. constructor injection ile.
            // business katmana yolladık verileri
            var result = await _userService.AddUser(addUserDto);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var loginUserDto = new LoginUserDto
            {
                Email = request.Email,
                Password = request.Password
            };
            var result = _userService.LoginUser(loginUserDto);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            // bilgier doğru ise- > token üret jwt
            var user = result.Data;

            // injection olayını constructor injection ile yapmadık. property injection ile yaptık.
            //HttpContext nedir? http response ve request yaşam döngünüz boyunca tüm bilgileri tutar.
            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var token = JwtHelper.GenerateJwtToken(new JwtDto
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                SecretKey = configuration["Jwt:SecretKey"],
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
                ExpireMinutes = int.Parse(configuration["Jwt:ExpireMinutes"])
            });

            return Ok(new LoginResponse { Message = "Giriş başarılı", Token = token });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok("Başarılı");
        }
    }
}