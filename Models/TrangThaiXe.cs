using System.Collections.Generic;

namespace HotelManagement.Models
{
    public partial class TrangThaiXe
    {
        public int Id { get; set; }
        public string TenTrangThai { get; set; }
        public virtual ICollection<Xe> Xes { get; set; }
    }
} 