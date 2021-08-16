using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice.Models;

namespace Practice.Controllers
{
    public class RecordController : Controller
    {   
        private readonly PracticeContext _context;

        public RecordController(PracticeContext context)
        {
            _context = context;
        }

        // GET: Record
        public async Task<IActionResult> Index()
        {
            return View(await _context.Records.ToListAsync());
        }

        // GET: Record/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @record = await _context.Records
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@record == null)
            {
                return NotFound();
            }

            return View(@record);
        }

        // GET: Record/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Record/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text")] Record @record)
        {
            if (!ModelState.IsValid) return View(@record);
            
            if (_context.Records.Any(e => e.Id == @record.Id || e.Text == @record.Text))
            {
                return BadRequest("Такая паста уже существует");
            }
                
            _context.Add(@record);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Record/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @record = await _context.Records.FindAsync(id);
            if (@record == null)
            {
                return NotFound();
            }
            return View(@record);
        }

        // POST: Record/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string text, int[] tagIds)
        {
            Record @record;

            if (string.IsNullOrEmpty(text))
            {
                return BadRequest("Текст пасты не может быть пустым.");
            }
            
            if (id == null)
            {
                if (_context.Records.Any(e => e.Text == text))
                {
                    return BadRequest("Такая паста уже существует.");
                }
                
                @record = new Record();
            }
            else
            {
                @record = await _context.Records.SingleOrDefaultAsync(r => r.Id == id);
                if (@record == null)
                {
                    return NotFound();
                }
            }
            
            @record.Text = text;

            foreach (var tagId in tagIds)
            {
                if (@record.RecordLabels.SingleOrDefault(rl => rl.TagId == tagId) == null)
                {
                    @record.RecordLabels.Add(new RecordLabel()
                    {
                        RecordId = @record.Id,
                        TagId = tagId
                    });
                }
            }

            foreach (var recordLabel in @record.RecordLabels.ToList())
            {
                if (!tagIds.Contains(recordLabel.TagId))
                {
                    @record.RecordLabels.Remove(recordLabel);
                }
            }

            try
            {
                _context.Update(@record);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecordExists(@record.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            return Ok();
        }

        // GET: Record/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @record = await _context.Records
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@record == null)
            {
                return NotFound();
            }

            return View(@record);
        }

        // POST: Record/Delete/5
        [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @record = await _context.Records.FindAsync(id);
            _context.Records.Remove(@record);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecordExists(int id)
        {
            return _context.Records.Any(e => e.Id == id);
        }
    }
}
