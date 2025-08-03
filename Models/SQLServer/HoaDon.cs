using System;
using System.Collections.Generic;

namespace mi_pham_kem.Models.SQLServer;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public DateOnly NgayThanhToan { get; set; }

    public decimal TongTien { get; set; }

    public int MaKh { get; set; }
    
    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual KhachHang MaKhNavigation { get; set; } = null!;
}
