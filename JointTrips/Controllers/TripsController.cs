using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JointTrips.Models;
using Microsoft.AspNetCore.Identity;

namespace JointTrips.Controllers
{
    public class TripsController : Controller
    {
        private readonly JointTripsContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TripsController(JointTripsContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Trips
        public async Task<IActionResult> Index()
        {
            var jointTripsContext = _context.Trips
                .Include(t => t.Owners)
                .Include(t => t.Participants)
                .Where(t => t.StartDate > DateTime.Now);
            return View(await jointTripsContext.ToListAsync());
        }

        // GET: Trips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.Owners)
                .Include(t => t.Participants)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // GET: Trips/Create
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,StartDate,EndDate,Price,Location,Capacity,Description")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }
                trip.Owners.Add(user);
                _context.Add(trip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trip);
        }

        // GET: Trips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }
            if (!trip.Owners.Any(u => u.Id == _userManager.GetUserId(User)))
            {
                return Forbid();
            }
            return View(trip);
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,StartDate,EndDate,Price,Location,Capacity,Description")] Trip trip)
        {
            if (id != trip.Id)
            {
                return NotFound();
            }

            foreach (var state in ModelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            var originalTrip = await _context.Trips.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (originalTrip == null)
            {
                return NotFound();
            }

            if (!trip.Owners.Any(u => u.Id == _userManager.GetUserId(User)))
            {
                return Forbid();
            }
            trip.Owners = originalTrip.Owners;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripExists(trip.Id))
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
            return View(trip);
        }

        // GET: Trips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.Owners)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

            if (!trip.Owners.Any(u => u.Id == _userManager.GetUserId(User)))
            {
                return Forbid();
            }
            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }
            if (!trip.Owners.Any(u => u.Id == _userManager.GetUserId(User)))
            {
                return Forbid();
            }
            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> join(int id)
        {
            var trip = await _context.Trips
                .Include(t => t.Participants)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (trip == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            if (!trip.Owners.Any(u => u.Id == _userManager.GetUserId(User)))
            {
                return Forbid();
            }

            if (trip.Participants.Any(u => u.Id == userId))
            {
                TempData["Message"] = "You have already registered for this trip.";
                return RedirectToAction(nameof(Details), new { id = trip.Id });
            }

            if (trip.Participants.Count >= trip.Capacity)
            {
                TempData["Message"] = "The trip is already full.";
                return RedirectToAction(nameof(Details), new { id = trip.Id });
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            trip.Participants.Add(user);
            await _context.SaveChangesAsync();
            TempData["Message"] = "You have successfully registered for the trip.";
            return RedirectToAction(nameof(Details), new { id = trip.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Leave(int id)
        {
            var trip = await _context.Trips
                .Include(t => t.Participants)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            var participant = trip.Participants.FirstOrDefault(p => p.Id == currentUserId);
            if (participant == null)
            {
                TempData["Message"] = "You are not registered for this trip.";
                return RedirectToAction(nameof(Details), new { id = trip.Id });
            }

            // Remove the user from the trip's participants.
            trip.Participants.Remove(participant);
            await _context.SaveChangesAsync();

            TempData["Message"] = "You have successfully unregistered from the trip.";
            return RedirectToAction(nameof(Details), new { id = trip.Id });
        }
        // GET: Trips/Participants/5
        public async Task<IActionResult> Participants(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.Owners)
                .Include(t => t.Participants)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trip == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (!trip.Owners.Any(u => u.Id == _userManager.GetUserId(User)))
            {
                return Forbid();
            }

            return View(trip);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrantOwnership(int tripId, string userId)
        {
            var trip = await _context.Trips
                .Include(t => t.Owners)
                .Include(t => t.Participants)
                .FirstOrDefaultAsync(t => t.Id == tripId);
            if(trip == null)
            {
                return NotFound();
            }
            var currentUserId = _userManager.GetUserId(User);
            if (!trip.Owners.Any(u => u.Id == currentUserId))
            {
                return Forbid();
            }
            if(currentUserId == userId)
            {
                TempData["Message"] = "You cannot change your own ownership.";
                return RedirectToAction(nameof(Participants), new { id = tripId });
            }

            var userToGrant = await _userManager.FindByIdAsync(userId);
            if (userToGrant == null)
            {
                return NotFound();
            }

            if(!trip.Owners.Any(u => u.Id == userId))
            {
                trip.Owners.Add(userToGrant);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Ownership granted.";
            }
            return RedirectToAction(nameof(Participants), new { id = tripId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeOwnership(int tripId, string userId)
        {
            var trip = await _context.Trips
                .Include(t => t.Owners)
                .Include(t => t.Participants)
                .FirstOrDefaultAsync(t => t.Id == tripId);
            if(trip == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (!trip.Owners.Any(u => u.Id == currentUserId))
            {
                return Forbid();
            }

            if(currentUserId == userId)
            {
                TempData["Message"] = "You cannot change your own ownership.";
                return RedirectToAction(nameof(Participants), new { id = tripId });
            }
            if (trip.Owners.Count <= 1)
            {
                TempData["Message"] = "A trip must have at least one owner.";
                return RedirectToAction(nameof(Participants), new { id = tripId });
            }

            var userToRevoke = await _userManager.FindByIdAsync(userId);
            if (userToRevoke == null)
            {
                return NotFound();
            }
            
            if(trip.Owners.Any(u => u.Id == userId))
            {
                trip.Owners.Remove(userToRevoke);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Ownership revoked.";
            }
            return RedirectToAction(nameof(Participants), new { id = tripId });
        }
    }
}