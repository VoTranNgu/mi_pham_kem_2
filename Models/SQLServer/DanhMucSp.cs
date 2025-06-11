using System;
using System.Collections.Generic;

namespace mi_pham_kem.Models.SQLServer;

public partial class DanhMucSp
{
    public int MaDm { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
