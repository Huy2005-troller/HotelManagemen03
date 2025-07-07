using System;
using System.Collections.Generic;

namespace HotelManagement.Models
{
    public partial class HoaDonDoHong
    {
        public int Id { get; set; }
        public string MaHoaDon { get; set; } = null!;
        public string TenDoHong { get; set; } = null!;
        public int SoLuong { get; set; }
        public float DonGia { get; set; }
        public float ThanhTien { get; set; }

        public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;
    }
} 