using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice.Filters;
using Practice.Models;

namespace Practice.Controllers
{
    [AdminAccessFilter]
    public class TagCategoryController : Controller
    {
        private readonly PracticeContext _context;

        public TagCategoryController(PracticeContext context)
        {
            _context = context;
        }

        // GET: TagCategory
        public async Task<IActionResult> Index()
        {
            return View(await _context.TagCategories.ToListAsync());
        }

        // GET: TagCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tagCategory = await _context.TagCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tagCategory == null)
            {
                return NotFound();
            }

            return View(tagCategory);
        }

        // GET: TagCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TagCategory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] TagCategory tagCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tagCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tagCategory);
        }

        // GET: TagCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tagCategory = await _context.TagCategories.FindAsync(id);
            if (tagCategory == null)
            {
                return NotFound();
            }
            return View(tagCategory);
        }

        // POST: TagCategory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] TagCategory tagCategory)
        {
            if (id != tagCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tagCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagCategoryExists(tagCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tagCategory);
        }

        // GET: TagCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tagCategory = await _context.TagCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tagCategory == null)
            {
                return NotFound();
            }

            return View(tagCategory);
        }

        // POST: TagCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tagCategory = await _context.TagCategories.FindAsync(id);
            _context.TagCategories.Remove(tagCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TagCategoryExists(int id)
        {
            return _context.TagCategories.Any(e => e.Id == id);
        }
    }
}
