using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using HealthOps_Project.Models;
using HealthOps_Project.Services;
using HealthOps_Project.Data;

namespace HealthOps_Project.Controllers
{
    [Authorize(Roles = "Admin,StockManager,ScriptManager")]
    public class ConsumablesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsumablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Consumables
        public async Task<IActionResult> Index()
        {
            return View(await _context.Consumables.ToListAsync());
        }

        // GET: Consumables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var consumable = await _context.Consumables.FindAsync(id);
            if (consumable == null) return NotFound();
            return View(consumable);
        }

        // GET: Consumables/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Consumables/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsumableId,Name,Description,Unit")] Consumable consumable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consumable);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Consumable created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(consumable);
        }

        // GET: Consumables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var consumable = await _context.Consumables.FindAsync(id);
            if (consumable == null) return NotFound();
            return View(consumable);
        }

        // POST: Consumables/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ConsumableId,Name,Description,Unit")] Consumable consumable)
        {
            if (id != consumable.ConsumableId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consumable);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Consumable updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsumableExists(consumable.ConsumableId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(consumable);
        }

        // GET: Consumables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var consumable = await _context.Consumables.FindAsync(id);
            if (consumable == null) return NotFound();
            return View(consumable);
        }

        // POST: Consumables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumable = await _context.Consumables.FindAsync(id);
            _context.Consumables.Remove(consumable);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Consumable deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool ConsumableExists(int id) => _context.Consumables.Any(e => e.ConsumableId == id);
    }
}