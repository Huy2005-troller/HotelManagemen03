using HotelManagement.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace HotelManagement.Controllers
{
    public class ReportController : Controller
    {
        private readonly IRepository _repo;
        public ReportController(IRepository repo)
        {
            _repo = repo;
        }
        // tổng số lượng phòng, loại, dịch vụ
        [HttpGet]
        [Route("api/Report/GetTotalCounts")]
        public JsonResult GetTotalCounts()
        {
            var totalRoom = _repo.getPhongByLoaiPhong(null).Count();
            var totalRoomType = _repo.getLoaiPhong.Count();
            var totalService = _repo.getDichvu.Count();
            return Json(new { totalRoom, totalRoomType, totalService });
        }
        // Trang báo cáo tổng hợp (mặc định theo tg)
        public IActionResult QLReport()
        {
            return View("~/Views/Admin/QLReport.cshtml");
        }
        // báo cáo theo phòng,loại phòng, dịch vụ
        public IActionResult QLReportByType()
        {
            return View("~/Views/Admin/QLReportByType.cshtml");
        }
        //Thêm API lấy danh sách năm có hóa đơn:
        public JsonResult GetAvailableYears()
        {
            var years = _repo.GetHoaDon
                .Where(hd => hd.NgayIn.HasValue)
                .Select(hd => hd.NgayIn.Value.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();
            return Json(years);
        }
        // API: Doanh thu theo tháng (Bar chart)
        public JsonResult GetRevenueByMonth(int year)
        {
            var data = _repo.GetHoaDon
                .Where(hd => hd.NgayIn.HasValue && hd.NgayIn.Value.Year == year)
                .GroupBy(hd => hd.NgayIn.Value.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(x => x.TongTien) })
                .ToList();

            // Tạo đủ 12 tháng
            var labels = Enumerable.Range(1, 12).Select(m => $"Tháng {m}").ToList();
            var values = Enumerable.Range(1, 12)
                .Select(m => data.FirstOrDefault(x => x.Month == m)?.Total ?? 0)
                .ToList();

            return Json(new { labels, values });
        }

        public JsonResult GetRevenueByQuarter(int year)
        {
            var data = _repo.GetHoaDon
                .Where(hd => hd.NgayIn.HasValue && hd.NgayIn.Value.Year == year)
                .GroupBy(hd => ((hd.NgayIn.Value.Month - 1) / 3) + 1)
                .Select(g => new { Quarter = g.Key, Total = g.Sum(x => x.TongTien) })
                .ToList();

            // Tạo đủ 4 quý
            var labels = Enumerable.Range(1, 4).Select(q => $"Quý {q}").ToList();
            var values = Enumerable.Range(1, 4)
                .Select(q => data.FirstOrDefault(x => x.Quarter == q)?.Total ?? 0)
                .ToList();

            return Json(new { labels, values });
        }

        // API: Doanh thu từng năm (Bar chart)
        public JsonResult GetRevenueByYear()
        {
            var data = _repo.GetHoaDon
                .Where(hd => hd.NgayIn.HasValue)
                .GroupBy(hd => hd.NgayIn.Value.Year)
                .Select(g => new { Year = g.Key, Total = g.Sum(x => x.TongTien) })
                .OrderBy(x => x.Year)
                .ToList();

            var labels = data.Select(x => x.Year.ToString()).ToList();
            var values = data.Select(x => x.Total).ToList();

            return Json(new { labels, values });
        }
        // trang lọc theo loại -----------------------------------------------------------------------------
        // Pie chart: Doanh thu từ tiền phòng theo từng loại phòng
        public JsonResult GetRevenueByRoomType(int year, int month = 0)
        {
            var hoaDons = _repo.GetHoaDonFull()
                .Where(hd => hd.NgayIn.HasValue && hd.NgayIn.Value.Year == year)
                .ToList();

            if (month > 0)
                hoaDons = hoaDons.Where(hd => hd.NgayIn.Value.Month == month).ToList();

            var data = hoaDons
                .Where(hd => hd.MaOrderPhongNavigation != null
                          && hd.MaOrderPhongNavigation.NgayDen.HasValue
                          && hd.MaOrderPhongNavigation.NgayDi.HasValue
                          && hd.MaOrderPhongNavigation.MaPhongNavigation != null
                          && hd.MaOrderPhongNavigation.MaPhongNavigation.MaLoaiPhongNavigation != null)
                .Select(hd => new
                {
                    SoNgay = (hd.MaOrderPhongNavigation.NgayDi.Value - hd.MaOrderPhongNavigation.NgayDen.Value).Days,
                    GiaPhong = hd.MaOrderPhongNavigation.MaPhongNavigation.MaLoaiPhongNavigation.GiaPhong,
                    LoaiPhong = hd.MaOrderPhongNavigation.MaPhongNavigation.MaLoaiPhongNavigation.TenLoaiPhong
                })
                .Where(x => x.SoNgay > 0)
                .GroupBy(x => x.LoaiPhong)
                .Select(g => new
                {
                    RoomType = g.Key,
                    Total = g.Sum(x => x.SoNgay * x.GiaPhong)
                })
                .Where(x => x.Total > 0) // Chỉ lấy loại phòng có phát sinh doanh thu
                .ToList();

            var labels = data.Select(x => x.RoomType).ToList();
            var values = data.Select(x => x.Total).ToList();

            return Json(new { labels, values });
        }

        // Pie chart: Doanh thu từ tiền phòng (chia theo tháng)
        public JsonResult GetRevenueByRoom(int year,int month = 0)
        {
            try
            {
                var hoaDons = _repo.GetHoaDonFull()
        .Where(hd => hd.NgayIn.HasValue && hd.NgayIn.Value.Year == year)
                .ToList();

                if (month > 0)
                    hoaDons = hoaDons.Where(hd => hd.NgayIn.Value.Month == month).ToList();


                var data = hoaDons
                    .Where(hd => hd.MaOrderPhongNavigation != null
                              && hd.MaOrderPhongNavigation.NgayDen.HasValue
                              && hd.MaOrderPhongNavigation.NgayDi.HasValue
                              && hd.MaOrderPhongNavigation.MaPhongNavigation != null
                              && hd.MaOrderPhongNavigation.MaPhongNavigation.MaLoaiPhongNavigation != null)
                    .Select(hd => new
                    {
                        SoNgay = (hd.MaOrderPhongNavigation.NgayDi.Value - hd.MaOrderPhongNavigation.NgayDen.Value).Days,
                        GiaPhong = hd.MaOrderPhongNavigation.MaPhongNavigation.MaLoaiPhongNavigation.GiaPhong,
                        TenPhong = hd.MaOrderPhongNavigation.MaPhongNavigation.TenPhong
                    })
                    .Where(x => x.SoNgay > 0)
                    .GroupBy(x => x.TenPhong)
                    .Select(g => new
                    {
                        Room = g.Key,
                        Total = g.Sum(x => x.SoNgay * x.GiaPhong)
                    })
                    .ToList();

                var labels = data.Select(x => x.Room).ToList();
                var values = data.Select(x => x.Total).ToList();

                return Json(new { labels, values });
            }
            catch (Exception ex)
            {
                // Log lỗi ra file hoặc console để debug
                System.IO.File.WriteAllText("error_log.txt", ex.ToString());
                // Trả về JSON báo lỗi cho client
                return Json(new { labels = new string[0], values = new int[0], error = ex.Message });
            }
        }

        // Pie chart: Doanh thu từ dịch vụ (chia theo tên dịch vụ)
        public JsonResult GetRevenueByService(int year, int month = 0)
        {
            var hoaDons = _repo.GetHoaDonFull()
                .Where(hd => hd.NgayIn.HasValue && hd.NgayIn.Value.Year == year)
                .ToList();

            if (month > 0)
                hoaDons = hoaDons.Where(hd => hd.NgayIn.Value.Month == month).ToList();

            var data = hoaDons
                .SelectMany(hd => hd.MaOrderPhongNavigation.OrderPhongDichVus)
                .GroupBy(dv => dv.MaDichVuNavigation.TenDichVu)
                .Select(g => new { Service = g.Key, Total = g.Sum(x => x.SoLuong * x.DonGia) })
                .Where(x => x.Total > 0)
                .ToList();

            var labels = data.Select(x => x.Service).ToList();
            var values = data.Select(x => x.Total).ToList();

            return Json(new { labels, values });
        }
    }
}