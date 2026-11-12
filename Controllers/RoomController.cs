using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthOps_Project.Data;
using HealthOps_Project.Models;

namespace HealthOps_Project.Controllers
{

    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms
                //.Where(r => r.IsAvailable) // Show only active rooms
                .ToListAsync();
            return View(rooms);
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == id && r.IsAvailable);

            if (room == null) return NotFound();
            return View(room);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room admission)
        {
            try
            {
                _context.Add(admission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Could not perform action please try again...");
            }

            //ViewBag.EmployeeList = new SelectList(_context.Beds, "BedId", "Bed", admission.Bed);
            //// If validation fails, repopulate dropdowns and return view
            //ViewBag.PatientId = new SelectList(_context.Patients, "PatientId", "FirstName", admission.PatientId);
            //ViewBag.WardId = new SelectList(_context.Wards, "WardId", "Name", admission.WardId);
            //ViewBag.BedId = new SelectList(_context.Beds, "BedId", "BedNumber", admission.BedId);

            return View(admission);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null || !room.IsAvailable) return NotFound();

            return View(room);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.RoomId) return NotFound();


            try
            {
                _context.Update(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Rooms.Any(p => p.RoomId == id))
                    return NotFound();
                else
                    throw;
            }


            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == id && r.IsAvailable);

            if (room == null) return NotFound();
            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            room.IsAvailable = false; // Soft delete
            _context.Update(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }



        // GET: Rooms/EditUnavailable/5
        public async Task<IActionResult> EditUnavailable(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null || room.IsAvailable) return NotFound(); // Only unavailable rooms

            return View(room);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUnavailable(int id, Room room)
        {
            if (id != room.RoomId) return NotFound();


            try
            {
                _context.Update(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Rooms.Any(p => p.RoomId == id))
                    return NotFound();
                else
                    throw;
            }


            return View(room);
        }

        // POST: Rooms/EditUnavailable/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditUnavailable(int id, Room room)
        //{
        //    if (id != room.RoomId) return NotFound();

        //    if (ModelState.IsValid)
        //    {
        //        _context.Update(room);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(room);
        //}

        // GET: Rooms/DetailsUnavailable/5
        public async Task<IActionResult> DetailsUnavailable(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FirstOrDefaultAsync(m => m.RoomId == id && !m.IsAvailable);
            if (room == null) return NotFound();

            return View(room);
        }

        // GET: Rooms/DeleteUnavailable/5
        public async Task<IActionResult> DeleteUnavailable(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FirstOrDefaultAsync(m => m.RoomId == id && !m.IsAvailable);
            if (room == null) return NotFound();

            return View(room);
        }

        // POST: Rooms/DeleteUnavailable/5
        [HttpPost, ActionName("DeleteUnavailable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUnavailableConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }








    }


    //public class RoomsController : Controller
    //{
    //    private readonly ApplicationDbContext _db;
    //    public RoomsController(ApplicationDbContext db) { _db = db; }

    //    public async Task<IActionResult> Index() => View(await _db.Set<Room>().ToListAsync());
    //    public IActionResult Create() => View();
    //    [HttpPost][ValidateAntiForgeryToken]
    //    public async Task<IActionResult> Create(Room model){ if(!ModelState.IsValid) return View(model); _db.Add(model); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
    //    public async Task<IActionResult> Edit(int id){ var m = await _db.Set<Room>().FindAsync(id); if(m==null) return NotFound(); return View(m); }
    //    [HttpPost][ValidateAntiForgeryToken]
    //    public async Task<IActionResult> Edit(Room model){ if(!ModelState.IsValid) return View(model); _db.Update(model); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
    //    public async Task<IActionResult> Delete(int id){ var m = await _db.Set<Room>().FindAsync(id); if(m==null) return NotFound(); return View(m); }
    //    [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
    //    public async Task<IActionResult> DeleteConfirmed(int id){ var m = await _db.Set<Room>().FindAsync(id); if(m!=null){ _db.Remove(m); await _db.SaveChangesAsync(); } return RedirectToAction(nameof(Index)); }
    //    public async Task<IActionResult> Details(int id){ var m = await _db.Set<Room>().FindAsync(id); if(m==null) return NotFound(); return View(m); }
    //}
}
