using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Harjoittelu.Data;
using Harjoittelu.Models;
using Microsoft.Data.SqlClient;

namespace Harjoittelu.Controllers
{
    public class StudentsController : Controller
    {
        private readonly HarjoitteluContext _context;

        public StudentsController(HarjoitteluContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.OrderBy(u => u.Name).ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            var vm = new StudentViewModel(student);
            return View(vm);
        }

        public async Task<IActionResult> Hours(int? id, DateTime? from, DateTime? to)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            var vm = new StudentViewModel(student);

            if (from == null && to == null)
            {
                vm.time = null;
            }
            else
            {
                if (from == null)
                    from = new DateTime(1900, 1, 1);

                if (to == null)
                    to = new DateTime(3000, 12, 31);

                var q = await _context.Loggings
                    .Where(x => (x.User_ID == id && x.Date.Date >= from && x.Date.Date <= to))
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                vm.time = CalcTimeAccumulated(q);
                vm.Events = q;
            }

            return View(vm);
        }

        TimeSpan CalcTimeAccumulated(List<LogInEvent> q)
        {
            TimeSpan time = new TimeSpan(0);
            DateTime start = new();
            bool loggedIn = false;

            foreach (var entry in q)
            {
                var status = (Student.Status_t)entry.New_status;
                var date = (DateTime)entry.Date;

                if ((status & Student.Status_t.LoggedIn) != 0) // log in event
                {
                    if (!loggedIn) // the first one after a log out
                    {
                        start = date;
                        loggedIn = true;
                    }
                }
                else if (loggedIn) // log out event
                {
                    time += date - start;
                    loggedIn = false;
                }
            }

            return time;
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View(new Student());
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Country,Status")] Student student)
        {
            if (ModelState.IsValid)
            {
                student.Status = 0;
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Users.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Country,Status")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Users.FindAsync(id);
            if (student != null)
            {
                _context.Users.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        public async Task<IActionResult> LogIn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            if ( ((Student.Status_t)student.Status & Student.Status_t.LoggedIn) == 0 )
            {
                var newStatus = student.Status | (byte)(Student.Status_t.LoggedIn | Student.Status_t.LoggedByAdmin);
                await _context.Database.ExecuteSqlAsync($"EXEC Update_Status {id}, {newStatus};");
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> LogOut(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            if ( ((Student.Status_t)student.Status & Student.Status_t.LoggedIn) != 0 )
            {
                var newStatus = (student.Status | (byte)Student.Status_t.LoggedByAdmin) & (~(byte)Student.Status_t.LoggedIn);
                await _context.Database.ExecuteSqlAsync($"EXEC Update_Status {id}, {newStatus};");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
