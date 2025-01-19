using Microsoft.AspNetCore.Mvc;
using System;

namespace OnlineShoppingPlatformApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("divide-by-zero")]
        public IActionResult TestDivideByZero()
        {
            int divisor = 0;
            var result = 5 / divisor; // DivideByZeroException
            return Ok(result);
        }

        [HttpGet("null-reference")]
        public IActionResult TestNullReference()
        {
            string nullString = null;
            var length = nullString.Length; // NullReferenceException
            return Ok(length);
        }

        [HttpGet("argument")]
        public IActionResult TestArgumentException()
        {
            throw new ArgumentException("Bu bir test argüman hatasıdır.");
        }

        [HttpGet("custom")]
        public IActionResult TestCustomException()
        {
            throw new InvalidOperationException("Bu bir test özel hata mesajıdır.");
        }
    }
}