using System;
using System.Collections.Generic;

namespace mi_pham_kem.Models.SQLServer;

public partial class ChiTietDonHang
{
    public int MaChiTietDh { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public int MaSanPham { get; set; }

    public int MaDh { get; set; }

    public virtual DonHang MaDhNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
