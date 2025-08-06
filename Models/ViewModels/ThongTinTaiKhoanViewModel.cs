using mi_pham_kem.Models.SQLServer;
using X.PagedList;

namespace mi_pham_kem.Models.ViewModels
{
    public class ThongTinTaiKhoanViewModel
    {
        public KhachHang KhachHang { get; set; }
        public IPagedList<DonHang> DonHangs { get; set; }
    }
}
