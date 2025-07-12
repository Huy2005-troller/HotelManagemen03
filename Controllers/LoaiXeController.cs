using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Models;

namespace HotelManagement.Controllers
{
    public class LoaiXeController : Controller
    {
        private readonly HotelContext _context;

        public LoaiXeController(HotelContext context)
        {
            _context = context;
        }

        // GET: LoaiXe
        public async Task<IActionResult> Index()
        {
              return View(await _context.LoaiXes.ToListAsync());
        }

        // GET: LoaiXe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LoaiXes == null)
            {
                return NotFound();
            }

            var loaiXe = await _context.LoaiXes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loaiXe == null)
            {
                return NotFound();
            }

            return View(loaiXe);
        }

        // GET: LoaiXe/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoaiXe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenLoaiXe")] LoaiXe loaiXe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiXe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiXe);
        }

        // GET: LoaiXe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LoaiXes == null)
            {
                return NotFound();
            }

            var loaiXe = await _context.LoaiXes.FindAsync(id);
            if (loaiXe == null)
            {
                return NotFound();
            }
            return View(loaiXe);
        }

        // POST: LoaiXe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenLoaiXe")] LoaiXe loaiXe)
        {
            if (id != loaiXe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiXe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiXeExists(loaiXe.Id))
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
            return View(loaiXe);
        }

        // GET: LoaiXe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LoaiXes == null)
            {
                return NotFound();
            }

            var loaiXe = await _context.LoaiXes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loaiXe == null)
            {
                return NotFound();
            }

            return View(loaiXe);
        }

        // POST: LoaiXe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LoaiXes == null)
            {
                return Problem("Entity set 'HotelContext.LoaiXes'  is null.");
            }
            var loaiXe = await _context.LoaiXes.FindAsync(id);
            if (loaiXe != null)
            {
                _context.LoaiXes.Remove(loaiXe);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaiXeExists(int id)
        {
          return _context.LoaiXes.Any(e => e.Id == id);
        }
    }
}
