using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingPlatformApp.Business.Operations.Setting;
using OnlineShoppingPlatformApp.Data.Enums;
using OnlineShoppingPlatformApp.WebApi.Filters;

namespace OnlineShoppingPlatformApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [TimeControlFilter(StartTime = "09:00", EndTime = "19:00")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingService _settingService;
        public SettingsController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpPatch]
        [Authorize(Roles = nameof(UserType.Admin))]
        public async Task<IActionResult> ToggleMaintenance()
        {
            await _settingService.ToggleMaintenance();
            return Ok();
        }
    }
}