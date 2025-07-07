namespace HotelManagement.ViewModels
{
    public class SelectedDichVuViewModel
    {
        public string MaDichVu { get; set; }
        public int? SoLuong { get; set; }
        public bool IsSelected { get; set; }
        public float? DonGia { get; set; } // optional nếu muốn dùng lại trong View
    }
}
