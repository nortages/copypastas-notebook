using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Practice.Filters
{
    public class HandleRoleFilter : IActionFilter
    {
        private readonly PracticeContext _dbContext;

        public HandleRoleFilter(PracticeContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Do something before the action executes.
            var controller = (Controller)context.Controller;
            var role = context.HttpContext.Session.GetString("Role");
            controller.ViewBag.Role = string.IsNullOrEmpty(role) ? "Guest" : role;
            controller.ViewBag.IsAdmin = role == "Admin";
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }
    }
}