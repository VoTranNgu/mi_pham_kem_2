using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mi_pham_kem.Models.SQLServer;

public partial class SanPham
{
    public int MaSanPham { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    public string TenSanPham { get; set; } = null!;

    [Required(ErrorMessage = "Giá sản phẩm không hợp lệ")]
    [Range(1000, double.MaxValue, ErrorMessage = "Giá phải từ 1,000 trở lên")]
    public decimal Gia { get; set; }

    [Required(ErrorMessage = "Mô tả không được để trống")]
    public string MoTa { get; set; } = null!;

    [Required(ErrorMessage = "Số lượng không hợp lệ")]
    [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số không âm")]
    public int SoLuong { get; set; }

    public string? HinhAnh { get; set; }
    public string? Hinh2 { get; set; }
    public string? Hinh3 { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn thương hiệu")]
    public int MaTh { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn danh mục")]
    public int MaDm { get; set; }
    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual DanhMucSp? MaDmNavigation { get; set; } = null!;

    public virtual ThuongHieu? MaThNavigation { get; set; } = null!;
}
