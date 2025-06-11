using System;
using System.Collections.Generic;

namespace mi_pham_kem.Models.SQLServer;

public partial class UuDaiKhachHang
{
    public int MaKh { get; set; }

    public int MaUd { get; set; }

    public int MaDh { get; set; }

    public bool? DaSuDung { get; set; }

    public virtual DonHang MaDhNavigation { get; set; } = null!;

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual UuDai MaUdNavigation { get; set; } = null!;
}
