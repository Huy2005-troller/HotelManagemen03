using System.Collections.Generic;

namespace HotelManagement.Models
{
    public partial class LoaiXe
    {
        public int Id { get; set; }
        public string TenLoaiXe { get; set; }
        public virtual ICollection<Xe> Xes { get; set; }
    }
} 