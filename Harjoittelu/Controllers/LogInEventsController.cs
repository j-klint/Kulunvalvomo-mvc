using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Harjoittelu.Data;
using Harjoittelu.Models;

namespace Harjoittelu.Controllers
{
    public class LogInEventsController : Controller
    {
        private readonly HarjoitteluContext _context;

        public LogInEventsController(HarjoitteluContext context)
        {
            _context = context;
        }

        // GET: LogInEvents
        public async Task<IActionResult> Index()
        {
            var harjoitteluContext = _context.Loggings.Include(l => l.User_).OrderBy(x => x.Date);
            return View(await harjoitteluContext.ToListAsync());
        }

        /*
        // GET: LogInEvents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logInEvent = await _context.Loggings
                .Include(l => l.User_)
                .FirstOrDefaultAsync(m => m.Event_Id == id);
            if (logInEvent == null)
            {
                return NotFound();
            }

            return View(logInEvent);
        }
        */

        // GET: LogInEvents/Create
        public IActionResult Create()
        {
            ViewData["User_ID"] = new SelectList(_context.Users.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        // POST: LogInEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("User_ID,Date,New_status")] LogInEvent logInEvent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(logInEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["User_ID"] = new SelectList(_context.Users, "Id", "Name", logInEvent.User_ID);
            return View(logInEvent);
        }

        // GET: LogInEvents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logInEvent = await _context.Loggings.FindAsync(id);
            if (logInEvent == null)
            {
                return NotFound();
            }
            ViewData["User_ID"] = new SelectList(_context.Users, "Id", "Name", logInEvent.User_ID);
            return View(logInEvent);
        }

        // POST: LogInEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Event_Id,User_ID,Date,New_status")] LogInEvent logInEvent)
        {
            if (id != logInEvent.Event_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(logInEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LogInEventExists(logInEvent.Event_Id))
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
            ViewData["User_ID"] = new SelectList(_context.Users, "Id", "Name", logInEvent.User_ID);
            return View(logInEvent);
        }

        // GET: LogInEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logInEvent = await _context.Loggings
                .Include(l => l.User_)
                .FirstOrDefaultAsync(m => m.Event_Id == id);
            if (logInEvent == null)
            {
                return NotFound();
            }

            return View(logInEvent);
        }

        // POST: LogInEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var logInEvent = await _context.Loggings.FindAsync(id);
            if (logInEvent != null)
            {
                _context.Loggings.Remove(logInEvent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LogInEventExists(int id)
        {
            return _context.Loggings.Any(e => e.Event_Id == id);
        }
    }
}
