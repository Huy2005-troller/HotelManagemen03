using System;

namespace HotelManagement.Models
{
    public partial class Xe
    {
        public int Id { get; set; }
        public string TenXe { get; set; }
        public string BienSo { get; set; }
        public decimal GiaTien1Km { get; set; }
        public string TenTaiXe { get; set; }
        public string CCCDTaiXe { get; set; }
        public string SDTTaiXe { get; set; }
        public int LoaiXeId { get; set; }
        public int TrangThaiXeId { get; set; }
        public virtual LoaiXe LoaiXe { get; set; }
        public virtual TrangThaiXe TrangThaiXe { get; set; }
    }
} 