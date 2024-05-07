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
    public class RFIDTagsController : Controller
    {
        private readonly HarjoitteluContext _context;

        public RFIDTagsController(HarjoitteluContext context)
        {
            _context = context;
        }

        // GET: RFIDTags
        public async Task<IActionResult> Index()
        {
            // Jos käyttää joinia, niin nullit karsiutuu pois.
            /*var q = from tag in _context.Tags
                    join user in _context.Users
                    on tag.User_Id equals user.Id
                    select new RFIDTagViewModel { Tag_Id = tag.Tag_Id, UserName = user.Name };
            */

            // Tuo esim. nuissa tägeissä käytetty .Include()-metodi varmaan ajaa saman asian
            // ja varmaan tekee sen jollakin fiksummalla tavalla. No, tulipa tehtyä,
            // kun en vielä semmoisesta tiennyt. Tiiä sitten nuista nulleista...
            var users = await _context.Users.ToListAsync();
            var output = new List<RFIDTagViewModel>();
            foreach (var tag in _context.Tags)
            {
                var match = users.Find(u => u.Id == tag.User_Id);
                if ( match is not null )
                    output.Add(new RFIDTagViewModel { Tag_Id = tag.Tag_Id, Rfid_Id = tag.Rfid_Id, Serial = tag.Serial, UserName = match.Name });
                else
                    output.Add(new RFIDTagViewModel { Tag_Id = tag.Tag_Id, Rfid_Id = tag.Rfid_Id, Serial = tag.Serial, UserName = "" });
            }

            return View(output.OrderBy(m => m.UserName).ThenBy(m => m.Serial));
        }

        /*
        // GET: RFIDTags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rFIDTag = await _context.Tags
                .FirstOrDefaultAsync(m => m.Tag_Id == id);
            if (rFIDTag == null)
            {
                return NotFound();
            }

            //return View(rFIDTag);
            var name = (await _context.Users.FirstOrDefaultAsync(u => u.Id == rFIDTag.User_Id))?.Name;
            var vm = new RFIDTagViewModel { Tag_Id = (int)id, Rfid_Id = rFIDTag.Rfid_Id, Serial = rFIDTag.Serial, UserName = name };
            return View(vm);
        }
        */

        // GET: RFIDTags/Create
        public async Task<IActionResult> Create()
        {
            RFIDTagAssignModel model = new RFIDTagAssignModel { students = await GetTaglessStudentsSelection() };
            return View(model);
        }

        // POST: RFIDTags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Tag_Id,Rfid_Id,Serial,StudentID")] RFIDTagAssignModel rFIDTagvm)
        {
            if (_context.Tags.Any(e => e.Rfid_Id == rFIDTagvm.Rfid_Id)) // codes must be unique to be able to tell users apart
            {
                ModelState.TryAddModelError("Rfid_Id", "That RFID code already exists.");
            }
            else if ( ModelState.IsValid )
            {
                var rFIDTag = new RFIDTag { Tag_Id = rFIDTagvm.Tag_Id, Rfid_Id = rFIDTagvm.Rfid_Id, Serial = rFIDTagvm.Serial, User_Id = rFIDTagvm.StudentID };
                _context.Add(rFIDTag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(rFIDTagvm);
        }

        // GET: RFIDTags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rFIDTag = await _context.Tags.FindAsync(id);
            if (rFIDTag == null)
            {
                return NotFound();
            }

            var options = await GetTaglessStudentsSelection(rFIDTag.User_Id);
            var vm = new RFIDTagAssignModel { students = options, Tag_Id = (int)id, Rfid_Id=rFIDTag.Rfid_Id, Serial=rFIDTag.Serial };
            return View(vm);
        }

        // POST: RFIDTags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Tag_Id,Rfid_Id,Serial,StudentID")] RFIDTagAssignModel rFIDTagvm)
        {
            if (id != rFIDTagvm.Tag_Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                var rFIDTag = new RFIDTag { Tag_Id = rFIDTagvm.Tag_Id, Rfid_Id = rFIDTagvm.Rfid_Id, Serial = rFIDTagvm.Serial, User_Id = rFIDTagvm.StudentID };
                try
                {
                    _context.Update(rFIDTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RFIDTagExists(rFIDTag.Tag_Id))
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
            return View(rFIDTagvm);
        }

        // GET: RFIDTags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rFIDTag = await _context.Tags
                .FirstOrDefaultAsync(m => m.Tag_Id == id);
            if (rFIDTag == null)
            {
                return NotFound();
            }

            return View(rFIDTag);
        }

        // POST: RFIDTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rFIDTag = await _context.Tags.FindAsync(id);
            if (rFIDTag != null)
            {
                _context.Tags.Remove(rFIDTag);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RFIDTagExists(int id)
        {
            return _context.Tags.Any(e => e.Tag_Id == id);
        }

        /// <summary>
        /// Obtain a List of students/users who don't yet have a tag assigned.<br />
        /// The optional parameter id denotes the user to be placed at the front of the
        /// List in case trying to assign an already taken tag.
        /// </summary>
        /// <param name="id">This one goes first</param>
        /// <returns></returns>
        private async Task<List<SelectListItem>> GetTaglessStudentsSelection(int? id = null)
        {
            var takenIds = await _context.Tags.Where(t => t.User_Id != null).Select(t => t.User_Id).ToListAsync();
            List<SelectListItem> options = new List<SelectListItem>();

            foreach (var user in _context.Users.OrderBy(x => x.Name))
            {
                if (!takenIds.Contains(user.Id))
                    options.Add(new SelectListItem { Value = user.Id.ToString(), Text = user.Name });
                else if (id != null && id == user.Id)
                    options.Insert(0, new SelectListItem { Value = user.Id.ToString(), Text = user.Name });
            }

            return options;
        }
    }
}
