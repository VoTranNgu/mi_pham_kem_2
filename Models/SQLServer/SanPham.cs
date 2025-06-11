using System;
using System.Collections.Generic;

namespace mi_pham_kem.Models.SQLServer;

public partial class SanPham
{
    public int MaSanPham { get; set; }

    public string TenSanPham { get; set; } = null!;

    public decimal Gia { get; set; }

    public string MoTa { get; set; } = null!;

    public int SoLuong { get; set; }

    public string? HinhAnh { get; set; }

    public string? Hinh2 { get; set; }

    public string? Hinh3 { get; set; }

    public int MaTh { get; set; }

    public int MaDm { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual DanhMucSp MaDmNavigation { get; set; } = null!;

    public virtual ThuongHieu MaThNavigation { get; set; } = null!;
}
