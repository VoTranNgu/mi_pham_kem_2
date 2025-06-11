using System;
using System.Collections.Generic;

namespace mi_pham_kem.Models.SQLServer;

public partial class DanhGium
{
    public int MaDg { get; set; }

    public DateOnly NgayTao { get; set; }

    public string BinhLuan { get; set; } = null!;

    public int DiemDanhGia { get; set; }

    public int MaSanPham { get; set; }

    public int MaKh { get; set; }

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
