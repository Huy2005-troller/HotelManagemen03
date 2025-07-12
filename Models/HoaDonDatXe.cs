using System;

namespace HotelManagement.Models
{
    public partial class HoaDonDatXe
    {
        public int Id { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public string TenXe { get; set; }
        public string BienSo { get; set; }
        public string TenTaiXe { get; set; }
        public string CCCDTaiXe { get; set; }
        public string SDTTaiXe { get; set; }
        public string TenKhach { get; set; }
        public string CCCDKhach { get; set; }
        public string SDTKhach { get; set; }
        public string DiemA { get; set; }
        public string DiemB { get; set; }
        public double KhoangCach { get; set; }
        public decimal DonGia { get; set; }
        public decimal TongTien { get; set; }
        public int XeId { get; set; }
    }
} 