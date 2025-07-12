using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Models;
using Microsoft.AspNetCore.Http;

namespace HotelManagement.Controllers
{
    public class XeController : Controller
    {
        private readonly HotelContext _context;

        public XeController(HotelContext context)
        {
            _context = context;
        }

        // GET: Xe
        public async Task<IActionResult> Index(string loaiXe, string trangThaiXe)
        {
            var xeQuery = _context.Xes.Include(x => x.LoaiXe).Include(x => x.TrangThaiXe).AsQueryable();
            if (!string.IsNullOrEmpty(loaiXe))
            {
                int lxId = int.Parse(loaiXe);
                xeQuery = xeQuery.Where(x => x.LoaiXeId == lxId);
            }
            if (!string.IsNullOrEmpty(trangThaiXe))
            {
                int ttxId = int.Parse(trangThaiXe);
                xeQuery = xeQuery.Where(x => x.TrangThaiXeId == ttxId);
            }
            var loaiXeList = _context.LoaiXes.Select(lx => new SelectListItem { Value = lx.Id.ToString(), Text = lx.TenLoaiXe }).ToList();
            var trangThaiXeList = _context.TrangThaiXes.Select(ttx => new SelectListItem { Value = ttx.Id.ToString(), Text = ttx.TenTrangThai }).ToList();
            ViewBag.LoaiXeList = loaiXeList;
            ViewBag.TrangThaiXeList = trangThaiXeList;
            ViewBag.SelectedLoaiXe = loaiXe;
            ViewBag.SelectedTrangThaiXe = trangThaiXe;
            return View(await xeQuery.ToListAsync());
        }

        // GET: Xe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Xes == null)
            {
                return NotFound();
            }

            var xe = await _context.Xes
                .Include(x => x.LoaiXe)
                .Include(x => x.TrangThaiXe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xe == null)
            {
                return NotFound();
            }

            return View(xe);
        }

        // GET: Xe/Create
        public IActionResult Create()
        {
            ViewData["LoaiXeId"] = new SelectList(_context.LoaiXes, "Id", "Id");
            ViewData["TrangThaiXeId"] = new SelectList(_context.TrangThaiXes, "Id", "Id");
            return View();
        }

        // POST: Xe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenXe,BienSo,GiaTien1Km,TenTaiXe,CCCDTaiXe,SDTTaiXe,LoaiXeId,TrangThaiXeId")] Xe xe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(xe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LoaiXeId"] = new SelectList(_context.LoaiXes, "Id", "Id", xe.LoaiXeId);
            ViewData["TrangThaiXeId"] = new SelectList(_context.TrangThaiXes, "Id", "Id", xe.TrangThaiXeId);
            return View(xe);
        }

        // GET: Xe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Xes == null)
            {
                return NotFound();
            }

            var xe = await _context.Xes.FindAsync(id);
            if (xe == null)
            {
                return NotFound();
            }
            ViewData["LoaiXeId"] = new SelectList(_context.LoaiXes, "Id", "Id", xe.LoaiXeId);
            ViewData["TrangThaiXeId"] = new SelectList(_context.TrangThaiXes, "Id", "Id", xe.TrangThaiXeId);
            return View(xe);
        }

        // POST: Xe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenXe,BienSo,GiaTien1Km,TenTaiXe,CCCDTaiXe,SDTTaiXe,LoaiXeId,TrangThaiXeId")] Xe xe)
        {
            if (id != xe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(xe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XeExists(xe.Id))
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
            ViewData["LoaiXeId"] = new SelectList(_context.LoaiXes, "Id", "Id", xe.LoaiXeId);
            ViewData["TrangThaiXeId"] = new SelectList(_context.TrangThaiXes, "Id", "Id", xe.TrangThaiXeId);
            return View(xe);
        }

        // GET: Xe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Xes == null)
            {
                return NotFound();
            }

            var xe = await _context.Xes
                .Include(x => x.LoaiXe)
                .Include(x => x.TrangThaiXe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xe == null)
            {
                return NotFound();
            }

            return View(xe);
        }

        // POST: Xe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Xes == null)
            {
                return Problem("Entity set 'HotelContext.Xes'  is null.");
            }
            var xe = await _context.Xes.FindAsync(id);
            if (xe != null)
            {
                _context.Xes.Remove(xe);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult GetCurrentUserInfo()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
                return Json(new { hoTen = "", cccd = "", sdt = "" });

            var person = _context.People.FirstOrDefault(p => p.PersonId == userName || p.HoTen == userName);
            if (person == null)
                return Json(new { hoTen = "", cccd = "", sdt = "" });

            return Json(new { hoTen = person.HoTen, cccd = person.Cccd, sdt = person.Sdt });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatXe(HoaDonDatXe model)
        {
            // Lấy xe
            var xe = await _context.Xes.Include(x => x.LoaiXe).Include(x => x.TrangThaiXe).FirstOrDefaultAsync(x => x.Id == model.XeId);
            if (xe == null)
                return NotFound();

            // Lưu hóa đơn đặt xe
            model.TenXe = xe.TenXe;
            model.BienSo = xe.BienSo;
            model.TenTaiXe = xe.TenTaiXe;
            model.CCCDTaiXe = xe.CCCDTaiXe;
            model.SDTTaiXe = xe.SDTTaiXe;
            model.DonGia = xe.GiaTien1Km;
            model.ThoiGianTao = DateTime.Now;
            // Tính tổng tiền
            model.TongTien = (decimal)model.KhoangCach * model.DonGia;
            _context.HoaDonDatXes.Add(model);

            // Cập nhật trạng thái xe thành "Không có sẵn"
            var trangThaiKhongCoSan = await _context.TrangThaiXes.FirstOrDefaultAsync(t => t.TenTrangThai.ToLower() == "không có sẵn");
            if (trangThaiKhongCoSan != null)
            {
                xe.TrangThaiXeId = trangThaiKhongCoSan.Id;
            }
            await _context.SaveChangesAsync();

            // Chuyển sang trang in vé xe
            return RedirectToAction("InVeXe", new { id = model.Id });
        }

        public async Task<IActionResult> InVeXe(int id)
        {
            var hoadon = await _context.HoaDonDatXes.FirstOrDefaultAsync(h => h.Id == id);
            if (hoadon == null) return NotFound();
            return View(hoadon);
        }

        private bool XeExists(int id)
        {
          return _context.Xes.Any(e => e.Id == id);
        }
    }
}
