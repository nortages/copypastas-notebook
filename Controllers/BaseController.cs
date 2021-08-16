using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Practice.Models;

namespace Practice.Controllers
{
    public class BaseController : Controller
    {
        private PracticeContext Context { get; }

        public BaseController(PracticeContext context)
        {
            Context = context;
        }
        
        public new async Task OnActionExecutionAsync(ActionExecutingContext context, 
            ActionExecutionDelegate next)
        {
            // logic before action goes here
            Console.WriteLine("HERE");
            ViewBag.Labels = await Context.Tags.ToListAsync();

            await next(); // the actual action

            // logic after the action goes here
        }
    }
}