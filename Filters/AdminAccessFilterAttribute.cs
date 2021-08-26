using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Practice.Filters
{
    public class AdminAccessFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var controller = (Controller)context.Controller;
            if (!controller.ViewBag.IsAdmin)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}