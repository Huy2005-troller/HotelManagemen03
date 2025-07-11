using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.Models.Authentication;
using HotelManagement.DataAccess;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using HotelManagement.ViewModels;
using Newtonsoft.Json;
namespace HotelManagement.Controllers
{
    public class RoomController : Controller
    {
        private IRepository repo;
        IHttpContextAccessor accessor;
        public RoomController(IRepository repo, IHttpContextAccessor accessor)
        {
            this.repo = repo;
            this.accessor = accessor;
        }

        LoaiPhongPhongTrangThaiPhong treetable = new LoaiPhongPhongTrangThaiPhong();

        [HttpGet]

        public IActionResult Index(string loaiphong = null, string trangthaiphong = null, bool error = true)
        {
            if (loaiphong == null && trangthaiphong == null) treetable.phongs = repo.getPhongByLoaiPhong(null);
            else if (loaiphong == null) treetable.phongs = repo.getPhongByMaTrangThai(trangthaiphong);
            else if (trangthaiphong == null) treetable.phongs = repo.getPhongByLoaiPhong(loaiphong);

            treetable.trangthaiphongs = repo.getTrangThaiPhong;
            treetable.loaiphongs = repo.getLoaiPhong;
            treetable.dichvus = repo.getDichvu;
            treetable.error = error;
            if (accessor.HttpContext.Session.GetString("UserName") != null)
            {
                treetable.Person = repo.getPersonByUserName(accessor.HttpContext.Session.GetString("UserName"));
            }
            return View(treetable);
        }

        [Authentication]
        public IActionResult datPhongVaDichVu(string hoten,
            int tuoi,
            int gioitinh,
            string cccd,
            string sdt,
            DateTime ngayden,
            DateTime ngaydi,
            string maphong,
            string selectedServiceIds,
            string servicePrice,
            string selectedQuantities)
        {
            if (string.IsNullOrEmpty(cccd) || string.IsNullOrEmpty(sdt) || ngayden == DateTime.MinValue || ngaydi == DateTime.MinValue)
            {
                return RedirectToAction("Index", new { error = false });
            }
            Person person = new Person();
            if (accessor.HttpContext.Session.GetString("UserName") != null)
            {
                person = repo.getPersonByUserName(accessor.HttpContext.Session.GetString("UserName"));
            }
            else
            {
                person = new Person
                {
                    PersonId = cccd,
                    HoTen = hoten,
                    Tuoi = tuoi,
                    GioiTinh = gioitinh,
                    Sdt = sdt,
                    Cccd = cccd
                };
            }

            string maorderphong = repo.createOrderPhongId();
            OrderPhong orderphong = new OrderPhong
            {
                MaOrderPhong = maorderphong,
                NgayDen = ngayden,
                NgayDi = ngaydi,
                PersonId = cccd,
                MaPhong = maphong,
                Person = person,
                TrangThaiThanhToan = 0,
                MaPhongNavigation = repo.getPhongByMaPhong(maphong)
            };


            //đầu tiên add order phòng
            repo.addOrderPhong(orderphong);

            //tiếp theo add order phòng và danh sách dịch vụ của order phòng đó
            if (selectedServiceIds != null && selectedQuantities != null && servicePrice != null)
            {
                List<string> madichvu = selectedServiceIds.Split(',').ToList();
                List<int> soLuongMoiDichVu = selectedQuantities.Split(",").Select(int.Parse).ToList();
                List<float> giaMoiDichVu = servicePrice.Split(",").Select(float.Parse).ToList();

                List<OrderPhongDichVu> orderphongdichvu = new List<OrderPhongDichVu>();
                for (int i = 0; i < madichvu.Count(); i++)
                {
                    orderphongdichvu.Add(new OrderPhongDichVu
                    {
                        MaOrderPhong = maorderphong,
                        MaDichVu = madichvu[i],
                        SoLuong = soLuongMoiDichVu[i],
                        DonGia = giaMoiDichVu[i]
                    });
                }

                repo.addOrderPhongDichVu(orderphongdichvu);
            }


            //cuối cùng update trạng thái phòng là đăng thuê

            //người đăng kí phòng là user thì mã trạng thái phòng là đặt trước,nếu là admin thì trạng thái đang thuê
            if (accessor.HttpContext.Session.GetString("UserName") != null)
            {
                repo.updateTrangThaiPhong(maphong, "MTT3");
            }
            else repo.updateTrangThaiPhong(maphong, "MTT2");

            return RedirectToAction("Index", "Room", new { error = true });
        }

        [AdminOrNhanVienAuthentication]
        [Route("[controller]/[action]/{maphong}/{successOrFail?}")]
        public IActionResult thanhToan(string maphong, string successOrFail = "0")
        {
            var order = repo.getOrderPhongByMaPhong(maphong).FirstOrDefault(od => od.TrangThaiThanhToan == 0);
            if (order == null) return NotFound();
            var allDichVus = repo.getDichvu.ToList();
            // Đọc lại đồ hỏng từ TempData nếu có
            List<DoHongViewModel> doHongs = new List<DoHongViewModel>();
            if (TempData["DoHongs"] != null)
            {
                try
                {
                    doHongs = JsonConvert.DeserializeObject<List<DoHongViewModel>>(TempData["DoHongs"].ToString());
                }
                catch { }
            }
            return View("thanhToan", new ThanhToanViewModel
            {
                Order = order,
                AllDichVus = allDichVus,
                SelectedDichVus = new List<SelectedDichVuViewModel>(),
                DoHongs = doHongs
            });
        }
                [HttpPost]
        [AdminOrNhanVienAuthentication]
        public IActionResult updateThanhToan(
    string maorder,
    string maphong,
    string selectedServiceIds,
    string servicePrice,
    string selectedQuantities,
    string doHongData,
    DateTime? NgayDen,
    DateTime? NgayDi,
    string action
)
        {
            // Debug: Log dữ liệu nhận được
            Console.WriteLine($"updateThanhToan - maorder: {maorder}");
            Console.WriteLine($"updateThanhToan - maphong: {maphong}");
            Console.WriteLine($"updateThanhToan - selectedServiceIds: {selectedServiceIds}");
            Console.WriteLine($"updateThanhToan - servicePrice: {servicePrice}");
            Console.WriteLine($"updateThanhToan - selectedQuantities: {selectedQuantities}");
            Console.WriteLine($"updateThanhToan - doHongData: {doHongData}");
            Console.WriteLine($"updateThanhToan - NgayDen: {NgayDen}");
            Console.WriteLine($"updateThanhToan - NgayDi: {NgayDi}");

            // Cập nhật ngày đến/ngày đi cho order
            var order = repo.getOrderPhongByMaOrder(maorder);
            if (order != null && NgayDen.HasValue && NgayDi.HasValue)
            {
                order.NgayDen = NgayDen.Value;
                order.NgayDi = NgayDi.Value;
                repo.updateOrderPhong(order);
            }

            // Xoá toàn bộ dịch vụ cũ trong order
            repo.deleteAllDichVuTrongOrder(maorder);

            // Thêm lại dịch vụ mới theo số lượng người dùng nhập
            if (!string.IsNullOrEmpty(selectedServiceIds) && !string.IsNullOrEmpty(selectedQuantities) && !string.IsNullOrEmpty(servicePrice))
            {
                List<string> madichvu = selectedServiceIds.Split(',').ToList();
                // Parse số lượng với validation
                List<int> soLuongMoiDichVu = new List<int>();
                try
                {
                    soLuongMoiDichVu = selectedQuantities.Split(",").Select(int.Parse).ToList();
                }
                catch
                {
                    TempData["ErrorMessage"] = "Dữ liệu số lượng không hợp lệ!";
                    return RedirectToAction("thanhToan", "Room", new { maphong = maphong });
                }
                // Parse giá với validation
                List<float> giaMoiDichVu = new List<float>();
                try
                {
                    giaMoiDichVu = servicePrice.Split(",").Select(float.Parse).ToList();
                }
                catch
                {
                    TempData["ErrorMessage"] = "Dữ liệu giá không hợp lệ!";
                    return RedirectToAction("thanhToan", "Room", new { maphong = maphong });
                }
                // Kiểm tra độ dài danh sách
                if (madichvu.Count != soLuongMoiDichVu.Count || madichvu.Count != giaMoiDichVu.Count)
                {
                    TempData["ErrorMessage"] = "Dữ liệu không đồng bộ!";
                    return RedirectToAction("thanhToan", "Room", new { maphong = maphong });
                }
                List<OrderPhongDichVu> danhSachMoi = new List<OrderPhongDichVu>();
                for (int i = 0; i < madichvu.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(madichvu[i]) && soLuongMoiDichVu[i] > 0 && giaMoiDichVu[i] >= 0) // chỉ thêm nếu mã dịch vụ không rỗng, số lượng > 0 và giá >= 0
                    {
                        danhSachMoi.Add(new OrderPhongDichVu
                        {
                            MaOrderPhong = maorder,
                            MaDichVu = madichvu[i],
                            SoLuong = soLuongMoiDichVu[i],
                            DonGia = giaMoiDichVu[i]
                        });
                    }
                }
                // Lưu lại danh sách mới
                repo.addOrderPhongDichVu(danhSachMoi);
            }

            // Lưu dữ liệu đồ hỏng vào TempData
            List<DoHongViewModel> doHongs = new List<DoHongViewModel>();
            if (!string.IsNullOrEmpty(doHongData))
            {
                try
                {
                    doHongs = JsonConvert.DeserializeObject<List<DoHongViewModel>>(doHongData);
                    TempData["DoHongs"] = JsonConvert.SerializeObject(doHongs);
                    Console.WriteLine($"Đã lưu dữ liệu đồ hỏng: {JsonConvert.SerializeObject(doHongs)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi parse dữ liệu đồ hỏng: {ex.Message}");
                    TempData["ErrorMessage"] = "Dữ liệu đồ hỏng không hợp lệ!";
                }
            }
            else
            {
                Console.WriteLine("Không có dữ liệu đồ hỏng");
            }

            // Lấy lại order mới nhất từ DB để truyền vào view (nếu muốn render lại view thay vì redirect)
            // var updatedOrder = repo.getOrderPhongByMaOrder(maorder);
            // var allDichVus = repo.getDichvu.ToList();
            // return View("thanhToan", new ThanhToanViewModel
            // {
            //     Order = updatedOrder,
            //     AllDichVus = allDichVus,
            //     SelectedDichVus = new List<SelectedDichVuViewModel>()
            // });

            // Quay lại trang thanh toán để tính lại tổng tiền (redirect để luôn lấy dữ liệu mới nhất từ DB)
            if (action == "xacNhanThanhToan")
            {
                return RedirectToAction("addHoadon", "Room", new { maorder = maorder, tongtien = 0, maphong = maphong });
            }
            else
            {
                return RedirectToAction("thanhToan", "Room", new { maphong = maphong });
            }
        }






        [AdminOrNhanVienAuthentication]
        public IActionResult addHoadon(string maorder, string tongtien, string maphong)
        {
            try
            {
                // Debug: Log dữ liệu nhận được
                Console.WriteLine($"addHoadon - maorder: {maorder}");
                Console.WriteLine($"addHoadon - tongtien: {tongtien}");
                Console.WriteLine($"addHoadon - maphong: {maphong}");

                // Lấy thông tin order để tính toán chính xác
                var order = repo.getOrderPhongByMaPhong(maphong).FirstOrDefault(od => od.MaOrderPhong == maorder);
                if (order == null) 
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin đặt phòng!";
                    return RedirectToAction("thanhToan", new { maphong = maphong });
                }

                // Kiểm tra xem order đã thanh toán chưa
                if (order.TrangThaiThanhToan == 1)
                {
                    TempData["ErrorMessage"] = "Đơn hàng này đã được thanh toán!";
                    return RedirectToAction("thanhToan", new { maphong = maphong });
                }

                // Tính tiền phòng
                var ngayden = order.NgayDen;
                var ngaydi = order.NgayDi;
                int soNgay = (ngaydi - ngayden)?.Days ?? 0;
                if (soNgay <= 0)
                {
                    TempData["ErrorMessage"] = "Thông tin ngày đến/đi không hợp lệ!";
                    return RedirectToAction("thanhToan", new { maphong = maphong });
                }
                
                float giaPhong = order.MaPhongNavigation.MaLoaiPhongNavigation.GiaPhong;
                if (giaPhong <= 0)
                {
                    TempData["ErrorMessage"] = "Giá phòng không hợp lệ!";
                    return RedirectToAction("thanhToan", new { maphong = maphong });
                }
                
                float tienPhong = giaPhong * soNgay;
                if (tienPhong < 0)
                {
                    TempData["ErrorMessage"] = "Tiền phòng không hợp lệ!";
                    return RedirectToAction("thanhToan", new { maphong = maphong });
                }

                // Tính tiền dịch vụ từ database
                float tienDichVu = 0;
                if (order.OrderPhongDichVus != null && order.OrderPhongDichVus.Any())
                {
                    tienDichVu = order.OrderPhongDichVus.Sum(odv => 
                    {
                        var soLuong = odv.SoLuong ?? 0;
                        var donGia = odv.DonGia ?? 0;
                        var thanhTien = soLuong * donGia;
                        return thanhTien >= 0 ? thanhTien : 0; // Đảm bảo không âm
                    });
                }

                // Tính tổng tiền trước thuế
                float tongTienTruocThue = tienPhong + tienDichVu;
                if (tienDichVu < 0)
                {
                    TempData["ErrorMessage"] = "Tiền dịch vụ không hợp lệ!";
                    return RedirectToAction("thanhToan", new { maphong = maphong });
                }

                // Đọc TempData đồ hỏng nếu có
                var doHongJson = TempData["DoHongs"]?.ToString();
                List<DoHongViewModel> doHongs = new List<DoHongViewModel>();
                
                if (!string.IsNullOrEmpty(doHongJson))
                {
                    try
                    {
                        doHongs = JsonConvert.DeserializeObject<List<DoHongViewModel>>(doHongJson) ?? new List<DoHongViewModel>();
                    }
                    catch
                    {
                        doHongs = new List<DoHongViewModel>();
                    }
                }

                float tienDoHong = doHongs.Sum(d => d.ThanhTien >= 0 ? d.ThanhTien : 0); // Đảm bảo không âm
                tongTienTruocThue += tienDoHong;
                
                if (tienDoHong < 0)
                {
                    TempData["ErrorMessage"] = "Tiền đồ hỏng không hợp lệ!";
                    return RedirectToAction("thanhToan", new { maphong = maphong });
                }

                // Tính thuế VAT 10%
                float thueVAT = tongTienTruocThue * 0.1f;
                float tongTienSauThue = tongTienTruocThue + thueVAT;

                // Lưu thông tin đồ hỏng vào TempData để hiển thị
                if (doHongs.Any())
                {
                    TempData["DoHongList"] = JsonConvert.SerializeObject(doHongs);
                }

                // Kiểm tra tổng tiền hợp lệ
                if (tongTienSauThue <= 0 || float.IsNaN(tongTienSauThue) || float.IsInfinity(tongTienSauThue))
                {
                    TempData["ErrorMessage"] = "Tổng tiền không hợp lệ!";
                    return RedirectToAction("thanhToan", new { maphong = maphong });
                }
                
                // Kiểm tra tổng tiền không quá lớn (tránh overflow)
                if (tongTienSauThue > 1000000000) // 1 tỷ VNĐ
                {
                    TempData["ErrorMessage"] = "Tổng tiền quá lớn!";
                    return RedirectToAction("thanhToan", new { maphong = maphong });
                }
                
                // Log thông tin tính toán để debug
                Console.WriteLine($"Tính toán hóa đơn cho {maorder}: Tiền phòng={tienPhong}, Tiền dịch vụ={tienDichVu}, Tiền đồ hỏng={tienDoHong}, Tổng trước thuế={tongTienTruocThue}, Thuế VAT={thueVAT}, Tổng sau thuế={tongTienSauThue}");
                Console.WriteLine($"Chi tiết đồ hỏng: {JsonConvert.SerializeObject(doHongs)}");

                var maHoaDon = repo.createHoaDonId();
                if (string.IsNullOrEmpty(maHoaDon))
                {
                    TempData["ErrorMessage"] = "Không thể tạo mã hóa đơn!";
                    return RedirectToAction("thanhToan", new { maphong = maphong });
                }

                var hd = new HoaDon
                {
                    MaHoaDon = maHoaDon,
                    NgayIn = DateTime.Now,
                    TongTien = tongTienSauThue, // Lưu tổng tiền sau thuế
                    MaOrderPhong = maorder,
                };

                bool check = repo.addHoaDon(hd);
                Console.WriteLine($"Kết quả tạo hóa đơn: {check}");

                if (check)
                {
                    try
                    {
                        // Lưu chi tiết đồ hỏng vào database
                        if (doHongs.Any())
                        {
                            var hoaDonDoHongs = doHongs.Select(d => new HoaDonDoHong
                            {
                                MaHoaDon = maHoaDon,
                                TenDoHong = d.TenDoHong,
                                SoLuong = d.SoLuong,
                                DonGia = d.DonGia,
                                ThanhTien = d.ThanhTien
                            }).ToList();

                            // Thêm vào database
                            foreach (var doHong in hoaDonDoHongs)
                            {
                                repo.addHoaDonDoHong(doHong);
                            }
                            Console.WriteLine($"Đã lưu {hoaDonDoHongs.Count} mục đồ hỏng vào database");
                        }

                        repo.updateTrangThaiPhong(maphong, "MTT1");
                        repo.updateTrangThaiOrderPhong(maorder);
                        
                        // Lưu thông tin hóa đơn vào TempData để hiển thị
                        TempData["HoaDonId"] = hd.MaHoaDon;
                        TempData["TongTien"] = tongTienSauThue.ToString("N0");
                        TempData["TongTienTruocThue"] = tongTienTruocThue.ToString("N0");
                        TempData["ThueVAT"] = thueVAT.ToString("N0");
                        TempData["TienPhong"] = tienPhong.ToString("N0");
                        TempData["TienDichVu"] = tienDichVu.ToString("N0");
                        TempData["TienDoHong"] = tienDoHong.ToString("N0");
                        
                        Console.WriteLine($"Hóa đơn tạo thành công: {hd.MaHoaDon}, Tổng tiền sau thuế: {tongTienSauThue}");
                        return View("ThanhToanThanhCong");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi cập nhật trạng thái: {ex.Message}");
                        TempData["ErrorMessage"] = "Lỗi khi cập nhật trạng thái: " + ex.Message;
                        return RedirectToAction("thanhToan", new { maphong = maphong });
                    }
                }
                else
                {
                    Console.WriteLine("Không thể tạo hóa đơn. Có thể đã tồn tại hóa đơn cho đơn hàng này!");
                    TempData["ErrorMessage"] = "Không thể tạo hóa đơn. Có thể đã tồn tại hóa đơn cho đơn hàng này!";
                    return RedirectToAction("thanhToan", new { maphong = maphong });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception trong addHoadon: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("thanhToan", new { maphong = maphong });
            }
        }

     

    }
}
