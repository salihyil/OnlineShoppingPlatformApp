using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;

namespace OnlineShoppingPlatformApp.WebApi.Filters
{
    public class TimeControlFilter : ActionFilterAttribute
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        private DateTime _actionStartTime;

        // 1. İlk çalışan method - Action method çalışmadan ÖNCE
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _actionStartTime = DateTime.Now;
            var timeOfDay = DateTime.Now.TimeOfDay;

            if (timeOfDay >= TimeSpan.Parse(StartTime, CultureInfo.InvariantCulture) && timeOfDay <= TimeSpan.Parse(EndTime, CultureInfo.InvariantCulture))
            {
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = new ContentResult()
                {
                    Content = $"Bu işlem sadece {StartTime} ile {EndTime} arasında yapılabilir.",
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }

        // 2. İkinci çalışan method - Action method çalıştıktan SONRA
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var executionTime = DateTime.Now - _actionStartTime;
            Console.WriteLine($"Action çalışma süresi: {executionTime.TotalMilliseconds}ms");

            if (context.Exception != null)
            {
                Console.WriteLine($"Action hata ile sonuçlandı: {context.Exception.Message}");
            }
        }

        // 3. Üçüncü çalışan method - Result (sonuç) execute edilmeden ÖNCE
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine($"Result tipi: {context.Result.GetType().Name} çalıştırılmak üzere");
            base.OnResultExecuting(context);
        }

        // 4. Dördüncü çalışan method - Result execute edildikten SONRA
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine("Result çalıştırıldı");
            base.OnResultExecuted(context);
        }
    }
}