using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthOps_Project.Controllers
{
    [Authorize(Roles = "Admin,StockManager")]
    public class ConsumableRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsumableRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ConsumableRequests
        public async Task<IActionResult> Index()
        {
            var requests = await _context.ConsumableRequests
                .Include(r => r.Consumable)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();
            return View(requests);
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var request = await _context.ConsumableRequests
                .Include(r => r.Consumable)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (request == null) return RedirectToAction(nameof(Index));
            return View(request);
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            var consumables = await _context.Consumables.OrderBy(c => c.Name).ToListAsync();
            ViewData["ConsumableId"] = new SelectList(consumables, "ConsumableId", "Name");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsumableId,WardName,QuantityRequested")] ConsumableRequest request)
        {
            if (ModelState.IsValid)
            {
                request.Status = "Pending";
                request.RequestedAt = DateTime.UtcNow;

                _context.Add(request);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Consumable request created successfully!";
                return RedirectToAction(nameof(Index));
            }

            var consumables = await _context.Consumables.OrderBy(c => c.Name).ToListAsync();
            ViewData["ConsumableId"] = new SelectList(consumables, "ConsumableId", "Name", request.ConsumableId);
            return View(request);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var request = await _context.ConsumableRequests.FindAsync(id);
            if (request == null) return RedirectToAction(nameof(Index));

            var consumables = await _context.Consumables.OrderBy(c => c.Name).ToListAsync();
            ViewData["ConsumableId"] = new SelectList(consumables, "ConsumableId", "Name", request.ConsumableId);
            return View(request);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ConsumableId,WardName,QuantityRequested,Status,RequestedAt,ReceivedAt")] ConsumableRequest request)
        {
            if (id != request.Id) return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Consumable request updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ConsumableRequests.Any(e => e.Id == request.Id))
                        return RedirectToAction(nameof(Index));
                    else throw;
                }
            }

            var consumables = await _context.Consumables.OrderBy(c => c.Name).ToListAsync();
            ViewData["ConsumableId"] = new SelectList(consumables, "ConsumableId", "Name", request.ConsumableId);
            return View(request);
        }

        // POST: MarkApproved
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkApproved(int id)
        {
            var request = await _context.ConsumableRequests.FindAsync(id);
            if (request == null) return RedirectToAction(nameof(Index));

            request.Status = "Approved";
            _context.Update(request);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Request approved successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: MarkReceived
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkReceived(int id)
        {
            var request = await _context.ConsumableRequests.FindAsync(id);
            if (request == null) return RedirectToAction(nameof(Index));

            request.Status = "Received";
            request.ReceivedAt = DateTime.UtcNow;
            _context.Update(request);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Request marked as received!";
            return RedirectToAction(nameof(Index));
        }

        // POST: MarkRejected
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRejected(int id)
        {
            var request = await _context.ConsumableRequests.FindAsync(id);
            if (request == null) return RedirectToAction(nameof(Index));

            request.Status = "Rejected";
            _context.Update(request);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Request rejected!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var request = await _context.ConsumableRequests
                .Include(r => r.Consumable)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (request == null) return RedirectToAction(nameof(Index));
            return View(request);
        }

        // POST: DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.ConsumableRequests.FindAsync(id);
            if (request != null)
            {
                _context.ConsumableRequests.Remove(request);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Request deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
