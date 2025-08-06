using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mi_pham_kem.Models.SQLServer;

public partial class User
{
    public int MaUser { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    public string TenDangNhap { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string MatKhau { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();

    [NotMapped]
    [Required(ErrorMessage = "Họ tên không được để trống")]
    public string HoTen { get; set; } = null!;
    [NotMapped]
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;
    [NotMapped]
    [Required(ErrorMessage = "Địa chỉ không được để trống")]
    public string DiaChi { get; set; } = null!;
    [NotMapped]
    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải gồm 10 chữ số")]
    public string Sdt { get; set; } = null!;

    [NotMapped]
    [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]
    public string ConfirmPass { get; set; } = null!;

}
