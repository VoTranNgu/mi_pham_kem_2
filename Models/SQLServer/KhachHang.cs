using System;
using System.Collections.Generic;

namespace mi_pham_kem.Models.SQLServer;

public partial class KhachHang
{
    public int MaKh { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public int MaUser { get; set; }

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual User MaUserNavigation { get; set; } = null!;

    public virtual ICollection<UuDaiKhachHang> UuDaiKhachHangs { get; set; } = new List<UuDaiKhachHang>();
}
