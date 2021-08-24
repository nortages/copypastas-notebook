﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Practice.Models;

namespace Practice.Filters
{
    public class AddTagsFilter : IActionFilter
    {
        private readonly PracticeContext _dbContext;

        public AddTagsFilter(PracticeContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Do something before the action executes.
            var controller = (Controller)context.Controller;
            controller.ViewBag.Tags = _dbContext.Tags;
            controller.ViewBag.TagCategories = _dbContext.TagCategories.ToList();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }
    }
}