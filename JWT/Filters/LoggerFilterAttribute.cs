using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace JWT.Filters
{
    public class LoggerFilterAttribute : Attribute, IActionFilter
    {
        private Stopwatch sw;
        public void OnActionExecuting(ActionExecutingContext context)
        {
            sw = Stopwatch.StartNew();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
