using System;
using System.Collections.Generic;

namespace mi_pham_kem.Models.SQLServer;

public partial class GioHang
{
    public int MaGioHang { get; set; }

    public int SoLuong { get; set; }

    public int MaKh { get; set; }

    public int MaSanPham { get; set; }

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
