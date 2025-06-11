using System;
using System.Collections.Generic;

namespace mi_pham_kem.Models.SQLServer;

public partial class UuDai
{
    public int MaUd { get; set; }

    public string NoiDungMa { get; set; } = null!;

    public decimal GiaGiam { get; set; }

    public DateOnly? ThoiHan { get; set; }

    public decimal? GiaToiThieu { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<UuDaiKhachHang> UuDaiKhachHangs { get; set; } = new List<UuDaiKhachHang>();
}
