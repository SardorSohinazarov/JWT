using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace JWT.Filters
{
    public class MemCacheFilterAttribute : Attribute, IResourceFilter
    {
        private readonly IMemoryCache _memoryCache;

        public MemCacheFilterAttribute()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var cachedData = _memoryCache.Get(context.HttpContext.Request.Path);

            if (cachedData != null)
            {
                context.Result = cachedData as IActionResult;
                return;
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            _memoryCache.Set(context.HttpContext.Request.Path, context.Result);
        }

    }
}
