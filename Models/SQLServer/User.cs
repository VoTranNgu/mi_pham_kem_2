using System;
using System.Collections.Generic;

namespace mi_pham_kem.Models.SQLServer;

public partial class User
{
    public int MaUser { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
}
