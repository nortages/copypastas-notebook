using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;
using X.PagedList;
using Practice.Models;

namespace Practice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PracticeContext _context;

        private const int RecordsNumberPerPage = 18;
        private const string RecordExistsMessage = "Такая паста уже существует.";
        private const string RecordNotFoundMessage = "Паста не найдена.";
        private const string RecordTextMustBeNonEmptyMessage = "Текст пасты не может быть пустым.";

        public HomeController(ILogger<HomeController> logger, PracticeContext context)
        {
            _logger = logger;
            _context = context;
        }

        private OneOf<Success, BadRequestObjectResult> EditRecord(Record @record, string text, int[] tagIds)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest(RecordTextMustBeNonEmptyMessage);
            }
            
            @record.Text = text;
            @record.RecordTags?.Clear();
            @record.RecordTags = new List<RecordTag>();
            foreach (var tagId in tagIds)
            {
                @record.RecordTags.Add(new RecordTag()
                {
                    RecordId = @record.Id,
                    TagId = tagId
                });
            }

            return new Success();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(string text, int[] tagIds)
        {
            if (!ModelState.IsValid) return BadRequest("Паста не валидна.");

            if (_context.Records.Any(e => e.Text == text))
            {
                return BadRequest(RecordExistsMessage);
            }
            var newRecord = new Record();
            var result = EditRecord(newRecord, text, tagIds);
            return result.Match<IActionResult>(success =>
            {
                try
                {
                    _context.Add(newRecord);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_context.Records.Any(e => e.Id == newRecord.Id))
                    {
                        return BadRequest(RecordExistsMessage);
                    }
                    throw;
                }
                return Ok();
            }, badRequestResult => badRequestResult);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string text, int[] tagIds)
        {
            var @record = await _context.Records.SingleOrDefaultAsync(r => r.Id == id);
            if (@record == null)
            {
                return NotFound(RecordNotFoundMessage);
            }
            
            var result = EditRecord(@record, text, tagIds);
            return result.Match<IActionResult>(success =>
            {
                try
                {
                    _context.Update(@record);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Records.Any(e => e.Id == @record.Id))
                    {
                        return BadRequest(RecordNotFoundMessage);
                    }
                    throw;
                }
            
                return Ok();
            }, badRequestResult => badRequestResult);
        }
        
        // POST: Record/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var @record = await _context.Records.FindAsync(id);
            _context.Records.Remove(@record);
            await _context.SaveChangesAsync();
            return Ok();
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

        private async Task<IActionResult> CopypastasList(IQueryable<Record> records, int page)
        {
            records = records.OrderByDescending(o => o.Id);
            return View("Index", records.ToPagedList(page, ViewBag.IsAdmin ? RecordsNumberPerPage - 1 : RecordsNumberPerPage));
        }

        public async Task<IActionResult> Copypasta(int id, int page = 1)
        {
            ViewBag.SearchString = "";
            ViewBag.IncludedTagIds = new List<int>();
            ViewBag.ExcludedTagIds = new List<int>();
            
            IQueryable<Record> records = _context.Records;
            records = records.Where(r => r.Id == id);
            return await CopypastasList(records, page);
        }
        
        public async Task<IActionResult> SimilarTo(int id, int page = 1)
        {
            ViewBag.SearchString = "";
            ViewBag.IncludedTagIds = new List<int>();
            ViewBag.ExcludedTagIds = new List<int>();
            
            IQueryable<Record> records = _context.Records;
            var originalRecord = records.Single(r => r.Id == id);
            var similarRecords = originalRecord.SimilarRecords.AsQueryable();
            return await CopypastasList(similarRecords, page);
        }

        public async Task<IActionResult> Index(
            string q,
            int[] inTag,
            int[] exTag,
            int page = 1)
        {
            var searchString = q?.Trim();
            var includedTagIds = inTag;
            var excludedTagIds = exTag;
            
            ViewBag.SearchString = searchString;
            ViewBag.IncludedTagIds = includedTagIds.ToList();
            ViewBag.ExcludedTagIds = excludedTagIds.ToList();
            
            IQueryable<Record> records = _context.Records;
            
            if (string.IsNullOrEmpty(searchString) && _context.Tags.Where(t => includedTagIds.Contains(t.Id)).All(t => t.CategoryId == null))
                records = records.Where(r => r.RecordTags.All(rt => rt.Tag.Category == null));
            
            if (!string.IsNullOrEmpty(searchString))
            {
                var pattern = $"%{searchString}%";
                records = records.Where(r => EF.Functions.ILike(r.Text, pattern));
            }
            
            if (includedTagIds.Length > 0)
                records = records.Where(r => _context.Tags.Where(t => includedTagIds.Contains(t.Id)).All(i => r.RecordTags.Any(rl => rl.TagId == i.Id)));
            
            if (excludedTagIds.Length > 0)
                records = records.Where(r => r.RecordTags.All(rl => !excludedTagIds.Contains(rl.TagId)));
            
            return await CopypastasList(records, page);
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