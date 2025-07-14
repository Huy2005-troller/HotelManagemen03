using HotelManagement.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public class AutoCheckoutBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public AutoCheckoutBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var repo = scope.ServiceProvider.GetRequiredService<IRepository>();
                        var today = DateTime.Now.Date;
                        // Lấy các order hết hạn (ngày đi < hôm nay, chưa thanh toán)
                        var orders = repo.getOrderPhongByMaPhong(null)
                            .Where(o => o.TrangThaiThanhToan == 0 && o.NgayDi < today)
                            .ToList();
                        foreach (var order in orders)
                        {
                            // Tạo hóa đơn nếu chưa có
                            var maHoaDon = repo.createHoaDonId();
                            var hd = new Models.HoaDon
                            {
                                MaHoaDon = maHoaDon,
                                NgayIn = DateTime.Now,
                                TongTien = 0, // Có thể tính lại tổng tiền nếu cần
                                MaOrderPhong = order.MaOrderPhong,
                            };
                            repo.addHoaDon(hd);
                            // Đổi trạng thái phòng sang trống
                            repo.updateTrangThaiPhong(order.MaPhong, "MTT1");
                            // Đánh dấu order đã thanh toán
                            repo.updateTrangThaiOrderPhong(order.MaOrderPhong);
                        }
                    }
                }
                catch { /* Ghi log nếu cần */ }
                // Chạy mỗi ngày 1 lần (test thì để 1 phút)
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                // await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // test nhanh
            }
        }
    }
} 