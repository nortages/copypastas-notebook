using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using X.PagedList;
using Practice.Models;

namespace Practice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PracticeContext _context;

        public HomeController(ILogger<HomeController> logger, PracticeContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(string password)
        {
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
            if (adminPassword != password) return View();
            HttpContext.Session.SetString("Role", "Admin");
            
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.SetString("Role", "Guest");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index(
            string searchString,
            int[] includedTagIds,
            int[] excludedTagIds, int page = 1)
        {
            searchString = searchString?.Trim();
            
            ViewBag.SearchString = searchString;
            ViewBag.IncludedTagIds = includedTagIds.ToList();
            ViewBag.ExcludedTagIds = excludedTagIds.ToList();
            ViewBag.TagCategories = await _context.TagCategories.ToListAsync();
            
            IQueryable<Record> records = _context.Records;
            
            records = records.Where(r => r.RecordTags.All(rt => !_context.TagCategories.Contains(rt.Tag.Category) || _context.Tags.Where(t => includedTagIds.Contains(t.Id)).Contains(rt.Tag)));
            
            if (!string.IsNullOrEmpty(searchString))
            {
                var pattern = $"%{searchString}%";
                records = records.Where(r => EF.Functions.ILike(r.Text, pattern));
            }
            
            if (includedTagIds.Length > 0)
                records = records.Where(r => _context.Tags.Where(t => includedTagIds.Contains(t.Id)).All(i => r.RecordTags.Any(rl => rl.TagId == i.Id)));
            
            if (excludedTagIds.Length > 0)
                records = records.Where(r => r.RecordTags.All(rl => !excludedTagIds.Contains(rl.TagId)));
            
            records = records.OrderByDescending(o => o.Id);
            
            const int pageSize = 18;
            
            return View(records.ToPagedList(page, ViewBag.IsAdmin ? pageSize - 1 : pageSize));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [HttpPost]
        public async Task<IActionResult> Add(Record newRecord, int[] RecordLabels)
        {   
            try
            {
                _context.Update(newRecord);
                await _context.SaveChangesAsync();
                
                foreach (var recordLabelId in RecordLabels)
                {
                    newRecord.RecordTags.Add(new RecordTag()
                    {
                        RecordId = newRecord.Id,
                        TagId = recordLabelId
                    });
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}