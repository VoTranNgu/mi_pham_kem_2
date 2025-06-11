using System;
using System.Collections.Generic;

namespace mi_pham_kem.Models.SQLServer;

public partial class DonHang
{
    public int MaDh { get; set; }

    public string TrangThai { get; set; } = null!;

    public decimal TongTien { get; set; }

    public DateOnly NgayDat { get; set; }

    public int MaKh { get; set; }

    public int MaHd { get; set; }

    public int? MaUd { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual UuDai? MaUdNavigation { get; set; }

    public virtual ICollection<UuDaiKhachHang> UuDaiKhachHangs { get; set; } = new List<UuDaiKhachHang>();
}
