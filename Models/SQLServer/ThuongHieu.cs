using System;
using System.Collections.Generic;

namespace mi_pham_kem.Models.SQLServer;

public partial class ThuongHieu
{
    public int MaTh { get; set; }

    public string TenThuongHieu { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
