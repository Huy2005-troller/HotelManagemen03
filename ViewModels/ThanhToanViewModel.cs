using HotelManagement.Models;
using System.Collections.Generic;
namespace HotelManagement.ViewModels
{
    public class ThanhToanViewModel
    {
        public OrderPhong Order { get; set; }
        public List<DichVu> AllDichVus { get; set; }
        public List<SelectedDichVuViewModel> SelectedDichVus { get; set; }
        // Thêm mới:
        public List<DoHongViewModel> DoHongs { get; set; } = new List<DoHongViewModel>();
        
        // Thêm thuế VAT
        public float TongTienPhong { get; set; }
        public float TongTienDichVu { get; set; }
        public float TongTienDoHong { get; set; }
        public float TongTienTruocThue { get; set; }
        public float ThueVAT { get; set; }
        public float TongTienSauThue { get; set; }
    }

}
