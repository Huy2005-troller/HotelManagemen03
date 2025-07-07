namespace HotelManagement.ViewModels
{
    public class DoHongViewModel
    {
        public string TenDoHong { get; set; }
        public int SoLuong { get; set; }
        public float DonGia { get; set; }
        public float ThanhTien => SoLuong * DonGia;
    }
}
