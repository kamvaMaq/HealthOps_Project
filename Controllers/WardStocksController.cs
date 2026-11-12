using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using HealthOps_Project.Data;

namespace HealthOps_Project.Controllers
{
    [Authorize(Roles = "Admin,StockManager")]
    public class WardStocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WardStocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WardStocks
        public async Task<IActionResult> Index()
        {
            var wardStocks = await _context.WardStocks
                .Include(w => w.Consumable)
                .OrderBy(w => w.WardName)
                .ThenBy(w => w.Consumable.Name)
                .ToListAsync();
            return View(wardStocks);
        }

        // GET: WardStocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Ward stock ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var wardStock = await _context.WardStocks
                .Include(w => w.Consumable)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (wardStock == null)
            {
                TempData["Error"] = "Ward stock not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(wardStock);
        }

        // GET: WardStocks/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var consumables = await _context.Consumables
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                if (!consumables.Any())
                {
                    TempData["Warning"] = "No consumables available. Please add consumables first.";
                }

                ViewData["ConsumableId"] = new SelectList(consumables, "ConsumableId", "Name");
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading form: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: WardStocks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WardStock wardStock)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(wardStock);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Ward stock created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                // If model state is invalid, reload consumables
                var consumables = await _context.Consumables
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                ViewData["ConsumableId"] = new SelectList(consumables, "ConsumableId", "Name", wardStock.ConsumableId);
                return View(wardStock);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating ward stock: {ex.Message}";

                var consumables = await _context.Consumables
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                ViewData["ConsumableId"] = new SelectList(consumables, "ConsumableId", "Name", wardStock.ConsumableId);
                return View(wardStock);
            }
        }

        // GET: WardStocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Ward stock ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var wardStock = await _context.WardStocks.FindAsync(id);
            if (wardStock == null)
            {
                TempData["Error"] = "Ward stock not found.";
                return RedirectToAction(nameof(Index));
            }

            var consumables = await _context.Consumables
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewData["ConsumableId"] = new SelectList(consumables, "ConsumableId", "Name", wardStock.ConsumableId);
            return View(wardStock);
        }

        // POST: WardStocks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WardStock wardStock)
        {
            if (id != wardStock.Id)
            {
                TempData["Error"] = "Ward stock ID mismatch.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wardStock);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Ward stock updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WardStockExists(wardStock.Id))
                    {
                        TempData["Error"] = "Ward stock not found.";
                        return RedirectToAction(nameof(Index));
                    }
                    throw;
                }
            }

            var consumables = await _context.Consumables
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewData["ConsumableId"] = new SelectList(consumables, "ConsumableId", "Name", wardStock.ConsumableId);
            return View(wardStock);
        }

        // GET: WardStocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Ward stock ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var wardStock = await _context.WardStocks
                .Include(w => w.Consumable)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (wardStock == null)
            {
                TempData["Error"] = "Ward stock not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(wardStock);
        }

        // POST: WardStocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wardStock = await _context.WardStocks.FindAsync(id);
            if (wardStock == null)
            {
                TempData["Error"] = "Ward stock not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.WardStocks.Remove(wardStock);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Ward stock deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: WardStocks/LowStock
        public async Task<IActionResult> LowStock()
        {
            var lowStock = await _context.WardStocks
                .Include(w => w.Consumable)
                .Where(w => w.QuantityOnHand < 10)
                .OrderBy(w => w.QuantityOnHand)
                .ThenBy(w => w.WardName)
                .ToListAsync();

            return View(lowStock);
        }

        private bool WardStockExists(int id)
        {
            return _context.WardStocks.Any(e => e.Id == id);
        }
    }
}
