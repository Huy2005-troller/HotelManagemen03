using HotelManagement.Models;

namespace HotelManagement.ViewModels
{
    public class LoaiPhongPhongTrangThaiPhong
    {
        public IEnumerable<LoaiPhong> loaiphongs { get; set; }
        public IEnumerable<Phong> phongs { get; set; }
        public IEnumerable<TrangThaiPhong> trangthaiphongs { get; set; }
        public IEnumerable<DichVu> dichvus { get; set; }
        public Person Person { get; set; } = null;
        public bool error { get; set; } = true;
    }
}
