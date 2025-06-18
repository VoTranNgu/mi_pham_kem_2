using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mi_pham_kem.Models.SQLServer;

public partial class User
{
    public int MaUser { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();

    [NotMapped]
    public string HoTen { get; set; } = null!;
    [NotMapped]
    public string Email { get; set; } = null!;
    [NotMapped]
    public string DiaChi { get; set; } = null!;
    [NotMapped]
    public string Sdt { get; set; } = null!;


}
