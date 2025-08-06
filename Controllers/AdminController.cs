using mi_pham_kem.Models.SQLServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Mvc.Core;
using System.Linq;
using X.PagedList.Extensions;

namespace mi_pham_kem.Controllers
{
    public class AdminController : Controller

    {
        MiPhamContext db = new MiPhamContext();
        private readonly MiPhamContext _context;

        public AdminController(MiPhamContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult XoaUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.MaUser == id);
            if (user == null)
            {
                return NotFound("❌ Không tìm thấy người dùng.");
            }

            // Xóa KhachHang nếu có
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaUser == user.MaUser);
            if (khachHang != null)
            {
                // Xóa giỏ hàng liên kết trước
                var gioHangs = _context.GioHangs.Where(g => g.MaKh == khachHang.MaKh).ToList();
                _context.GioHangs.RemoveRange(gioHangs);

                _context.KhachHangs.Remove(khachHang);
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult SuaUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            return View(user);
        }


        [HttpPost]
public IActionResult SuaUser(User model)
{
    if (ModelState.IsValid)
    {
        var user = _context.Users.FirstOrDefault(u => u.MaUser == model.MaUser);
        if (user == null)
            return NotFound();

        user.TenDangNhap = model.TenDangNhap;
        user.MatKhau = model.MatKhau;
        user.Role = model.Role;

        var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaUser == model.MaUser);
        if (khachHang != null)
        {
            khachHang.HoTen = model.HoTen;
            khachHang.Email = model.Email;
            khachHang.Sdt = model.Sdt;
            khachHang.DiaChi = model.DiaChi;
        }

        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // ✅ Load lại dữ liệu khách hàng khi model lỗi
    var kh = _context.KhachHangs.FirstOrDefault(kh => kh.MaUser == model.MaUser);
    if (kh != null)
    {
        model.HoTen = kh.HoTen;
        model.Email = kh.Email;
        model.Sdt = kh.Sdt;
        model.DiaChi = kh.DiaChi;
    }

    return View(model);
}




        public IActionResult Index()
        {
            var users = _context.Users
                .Include(u => u.KhachHangs)
                .Where(u => u.Role == "user")
                .ToList();

            foreach (var user in users)
            {
                var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaUser == user.MaUser);
                if (khachHang != null)
                {
                    user.HoTen = khachHang.HoTen;
                    user.Email = khachHang.Email;
                    user.Sdt = khachHang.Sdt;
                    user.DiaChi = khachHang.DiaChi;
                }
            }

            return View(users);
        }


        public IActionResult TaoTaiKhoanAdmin()
        {
            var existing = _context.Users.FirstOrDefault(u => u.TenDangNhap == "toan");
            if (existing == null)
            {
                var newAdmin = new User
                {
                    TenDangNhap = "toan",
                    MatKhau = "1234",
                    Role = "AD"
                };

                _context.Users.Add(newAdmin);
                _context.SaveChanges();

                return RedirectToAction("Index"); // Chuyển sang trang Index sau khi tạo xong
            }

            return Content("⚠️ Admin đã tồn tại");
        }
        public IActionResult SanPhamAD(string? search)
        {
            var query = _context.SanPhams
                .Include(sp => sp.MaThNavigation)
                .Include(sp => sp.MaDmNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(sp => sp.TenSanPham.Contains(search));
            }

            var sanPhamList = query.ToList();
            return View(sanPhamList);
        }

        public IActionResult ThemSanPham()
        {
            ViewBag.DanhMucs = new SelectList(_context.DanhMucSps, "MaDm", "TenDanhMuc");
            ViewBag.ThuongHieus = new SelectList(_context.ThuongHieus, "MaTh", "TenThuongHieu");
            return View();
        }
        [HttpPost]
        public IActionResult ThemSanPham(SanPham sp)
        {
            if (ModelState.IsValid)
            {
                _context.SanPhams.Add(sp);
                _context.SaveChanges();
                Console.WriteLine("✅ Đã thêm sản phẩm: " + sp.TenSanPham);
                return RedirectToAction("SanPhamAD");
            }

            Console.WriteLine("❌ Không hợp lệ:");
            foreach (var e in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(" - " + e.ErrorMessage);
            }

            ViewBag.DanhMucs = new SelectList(_context.DanhMucSps, "MaDm", "TenDanhMuc", sp.MaDm);
            ViewBag.ThuongHieus = new SelectList(_context.ThuongHieus, "MaTh", "TenThuongHieu", sp.MaTh);
            return View(sp);
        }
        [HttpPost]
        public IActionResult XoaDanhMuc(int id)
        {
            var item = _context.DanhMucSps.Find(id);
            if (item != null)
            {
                _context.DanhMucSps.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("SanPhamAD");
        }



        [HttpPost]
        public IActionResult XoaThuongHieu(int id)
        {
            var item = _context.ThuongHieus.Find(id);
            if (item != null)
            {
                _context.ThuongHieus.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("SanPhamAD");
        }

   

        [HttpPost]
        public IActionResult XoaSanPham(int id)
        {
            var item = _context.SanPhams.Find(id);
            if (item != null)
            {
                _context.SanPhams.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("SanPhamAD");
        }
       
        //SuaSANPHAM
        [HttpGet]
        public IActionResult SuaSPAD(int id)
        {
            var sp = _context.SanPhams.FirstOrDefault(s => s.MaSanPham == id);
            if (sp == null)
                return NotFound();

            ViewBag.DanhMucs = new SelectList(_context.DanhMucSps, "MaDm", "TenDanhMuc", sp.MaDm);
            ViewBag.ThuongHieus = new SelectList(_context.ThuongHieus, "MaTh", "TenThuongHieu", sp.MaTh);

            return View(sp);
        }
        [HttpPost]
        public IActionResult SuaSPAD(SanPham sp)
        {
            if (ModelState.IsValid)
            {
                _context.SanPhams.Update(sp);
                _context.SaveChanges();
                return RedirectToAction("SanPhamAD");
            }

            // Trường hợp lỗi, load lại danh mục và thương hiệu
            ViewBag.DanhMucs = new SelectList(_context.DanhMucSps, "MaDm", "TenDanhMuc", sp.MaDm);
            ViewBag.ThuongHieus = new SelectList(_context.ThuongHieus, "MaTh", "TenThuongHieu", sp.MaTh);
            return View(sp);
        }

        
        
        public IActionResult DanhMucAd(int? page)
        {
            int pageSize = 5; // số danh mục trên mỗi trang
            int pageNumber = page ?? 1; 
            var danhMucQuery = db.DanhMucSps.OrderBy(s => s.MaDm).AsQueryable();

            IPagedList<DanhMucSp> lstdanhmuc = danhMucQuery.ToPagedList(pageNumber, pageSize);

            return View(lstdanhmuc);
        }

        [HttpGet]
        public IActionResult ThemDanhMucAD()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemDanhMucAD(DanhMucSp dm)
        {
            if (ModelState.IsValid)
            {
                db.DanhMucSps.Add(dm);
                db.SaveChanges();
                return RedirectToAction("DanhMucAD"); // chuyển về danh sách
            }

            return View(dm); // nếu không hợp lệ
        }
     

        // GET: Sửa danh mục
        [HttpGet]
        public IActionResult SuaDanhMucAD(int? id)
        {
            if (id == null)
                return NotFound();

            var dm = db.DanhMucSps.Find(id);
            if (dm == null)
                return NotFound();

            return View(dm);
        }

        // POST: Sửa danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaDanhMucAD(DanhMucSp dm)
        {
            if (ModelState.IsValid)
            {
                db.Update(dm);
                db.SaveChanges();
                return RedirectToAction("DanhMucAD");
            }
            return View(dm);
        }

        [HttpPost]
        public IActionResult XoaDanhMucAD(int id)
        {
            var danhMuc = _context.DanhMucSps
                .Include(dm => dm.SanPhams)
                .FirstOrDefault(dm => dm.MaDm == id);

            if (danhMuc == null)
                return NotFound();

            if (danhMuc.SanPhams.Any())
            {
                TempData["ErrorMessage"] = "❌ Không thể xóa! Danh mục này vẫn còn sản phẩm.";
                return RedirectToAction("DanhMucAD");
            }

            _context.DanhMucSps.Remove(danhMuc);
            _context.SaveChanges();

            return RedirectToAction("DanhMucAD");
        }
        //đánh giá
        public IActionResult XemDanhGiaAd(int id)
        {
            var sp = _context.SanPhams
                .Include(s => s.DanhGia)
                .ThenInclude(d => d.MaKhNavigation)
                .FirstOrDefault(s => s.MaSanPham == id);

            if (sp == null) return NotFound();

            return View(sp);
        }
        //chitiet
        public IActionResult ChiTietDMAD(int? id)
        {
            if (id == null)
                return NotFound();

            var dm = db.DanhMucSps
                .Include(d => d.SanPhams) // lấy luôn sản phẩm
                .FirstOrDefault(x => x.MaDm == id);

            if (dm == null)
                return NotFound();

            return View(dm);
        }
        //UuDai
        public IActionResult UuDaiAD()
        {
            var list = _context.UuDais.ToList();
            return View(list);
        }

        [HttpGet]
        public IActionResult ThemUuDai() => View();

        [HttpPost]
        public IActionResult ThemUuDai(UuDai uuDai)
        {
            if (ModelState.IsValid)
            {
                _context.UuDais.Add(uuDai);
                _context.SaveChanges();
                return RedirectToAction("UuDaiAD");
            }
            return View(uuDai);
        }

        [HttpGet]
        public IActionResult SuaUuDai(int id)
        {
            var uuDai = _context.UuDais.Find(id);
            if (uuDai == null) return NotFound();
            return View(uuDai);
        }

        [HttpPost]
        public IActionResult SuaUuDai(UuDai uuDai)
        {
            if (ModelState.IsValid)
            {
                _context.UuDais.Update(uuDai);
                _context.SaveChanges();
                return RedirectToAction("UuDaiAD");
            }
            return View(uuDai);
        }

        public IActionResult XoaUuDai(int id)
        {
            var uuDai = _context.UuDais.Find(id);
            if (uuDai != null)
            {
                _context.UuDais.Remove(uuDai);
                _context.SaveChanges();
            }
            return RedirectToAction("UuDaiAD");
        }
        //ThuongHieu
        public IActionResult ThuongHieuAD(int? page)
        {

            int pageSize = 5; // số thương hiệu mỗi trang
            int pageNumber = page ?? 1;

            var thuongHieuQuery = db.ThuongHieus.OrderBy(th => th.MaTh).AsQueryable();

            IPagedList<ThuongHieu> lstThuongHieu = thuongHieuQuery.ToPagedList(pageNumber, pageSize);

            return View(lstThuongHieu);
        }

        [HttpGet]
        public IActionResult ThemThuongHieuAD()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemThuongHieuAD(ThuongHieu th)
        {
            if (ModelState.IsValid)
            {
                _context.ThuongHieus.Add(th);
                _context.SaveChanges();
                TempData["Success"] = "✅ Thêm thương hiệu thành công!";
                return RedirectToAction("ThuongHieuAD");
            }

            return View(th);
        }

        [HttpGet]
        public IActionResult SuaThuongHieuAD(int id)
        {
            var th = _context.ThuongHieus.Find(id);
            if (th == null)
                return NotFound();

            return View(th);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaThuongHieuAD(ThuongHieu th)
        {
            if (ModelState.IsValid)
            {
                _context.ThuongHieus.Update(th);
                _context.SaveChanges();
                TempData["Success"] = " Cập nhật thương hiệu thành công!";
                return RedirectToAction("ThuongHieuAD");
            }

            return View(th);
        }

        [HttpPost]
        public IActionResult XoaThuongHieuAD(int id)
        {
            var thuongHieu = _context.ThuongHieus
                .Include(th => th.SanPhams)
                .FirstOrDefault(th => th.MaTh == id);

            if (thuongHieu == null)
                return NotFound();

            if (thuongHieu.SanPhams.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa! Thương hiệu này vẫn còn sản phẩm.";
                return RedirectToAction("ThuongHieuAD");
            }

            _context.ThuongHieus.Remove(thuongHieu);
            _context.SaveChanges();

            TempData["SuccessMessage"] = " Đã xóa thương hiệu thành công.";
            return RedirectToAction("ThuongHieuAD");
        }
        public IActionResult ChiTietThuongHieuAD(int? id)
        {
            if (id == null)
                return NotFound();

            var thuongHieu = _context.ThuongHieus
                .Include(th => th.SanPhams) // lấy luôn danh sách sản phẩm thuộc thương hiệu
                .FirstOrDefault(th => th.MaTh == id);

            if (thuongHieu == null)
                return NotFound();

            return View(thuongHieu);
        }
        //dangxuat
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("DangNhap", "Login");
        }
        //quanlidonhang
        public IActionResult QuanLyAD(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var donHangs = _context.DonHangs
                .Include(dh => dh.MaKhNavigation)
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                .OrderByDescending(d => d.NgayDat)
                .ToPagedList(pageNumber, pageSize);

            return View(donHangs);
        }
        public IActionResult ChinhSuaQuanLyDH(int id)
        {
            var donHang = _context.DonHangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                .FirstOrDefault(d => d.MaDh == id);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        [HttpPost]
        public IActionResult ChinhSuaQuanLyDH(DonHang donHang)
        {
            var dh = _context.DonHangs.FirstOrDefault(d => d.MaDh == donHang.MaDh);
            if (dh == null)
            {
                return NotFound();
            }
            
            if(donHang.TrangThai == "Đã hoàn tất" && dh.MaHd == null)
            {
                var hoaDon = new HoaDon
                {
                    NgayThanhToan = DateOnly.FromDateTime(DateTime.Now),
                    TongTien = dh.TongTien,
                    MaKh = dh.MaKh
                };
                _context.HoaDons.Add(hoaDon);
                _context.SaveChanges();
                //gan MaHD vao DonHang
                dh.MaHd = hoaDon.MaHd;
            }

            dh.TrangThai = donHang.TrangThai;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công.";
            return RedirectToAction("QuanLyAD"); 
        }

        //Bai Viet
        public IActionResult BaiVietAD(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var dsBaiViet = _context.BaiViets
                .OrderByDescending(b => b.NgayDang)
                .ToPagedList(pageNumber, pageSize);

            return View(dsBaiViet);
        }

        public IActionResult ThemBaiViet()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemBaiViet(BaiViets model)
        {
            if (ModelState.IsValid)
            {
                model.NgayDang = DateTime.Now;
                _context.BaiViets.Add(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult SuaBaiViet(int id)
        {
            var baiViet = _context.BaiViets.FirstOrDefault(b => b.Id == id);
            if (baiViet == null)
            {
                return NotFound();
            }
            return View(baiViet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaBaiViet(int id, BaiViets model)
        {
            var baiViet = _context.BaiViets.FirstOrDefault(b => b.Id == id);
            if (baiViet == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                baiViet.TieuDe = model.TieuDe;
                baiViet.AnhDaiDien = model.AnhDaiDien;
                baiViet.NoiDung = model.NoiDung;
                //ngay dang khong doi
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult XoaBaiViet(int id)
        {
            var item = _context.BaiViets.Find(id);
            if (item != null)
            {
                _context.BaiViets.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("BaiVietAD");
        }
        public IActionResult ChiTietBaiVietAD(int id)
        {
            var baiViet = _context.BaiViets.FirstOrDefault(b => b.Id == id);
            if (baiViet == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài viết!";
                return RedirectToAction("BaiVietAD");
            }

            return View(baiViet);
        }
        //thong ke
        public IActionResult ThongKeDoanhThu(DateTime? fromDate, DateTime? toDate)
        {
            // Chuyển đổi DateTime? thành DateOnly
            var from = fromDate.HasValue ? DateOnly.FromDateTime(fromDate.Value) : DateOnly.MinValue;
            var to = toDate.HasValue ? DateOnly.FromDateTime(toDate.Value) : DateOnly.MaxValue;

            // Truy vấn hóa đơn trong khoảng thời gian
            var hoaDonQuery = _context.HoaDons
                .Include(h => h.MaKhNavigation)
                .Where(h => h.NgayThanhToan >= from && h.NgayThanhToan <= to);


            // Tính tổng doanh thu
            decimal doanhThu = hoaDonQuery.Sum(h => h.TongTien);

            // Group theo tháng để tạo biểu đồ
            var labels = new List<string>();
            var doanhThuData = new List<decimal>();

            var groupByMonth = hoaDonQuery
                .ToList()
                .GroupBy(h => new { h.NgayThanhToan.Month, h.NgayThanhToan.Year })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month);

            foreach (var g in groupByMonth)
            {
                labels.Add($"{g.Key.Month}/{g.Key.Year}");
                doanhThuData.Add(g.Sum(x => x.TongTien));
            }

            // Danh sách hóa đơn
            var donHangList = hoaDonQuery
                .Select(h => new
                {
                    MaDon = h.MaHd,
                    TenKhach = h.MaKhNavigation.HoTen,
                    NgayDat = h.NgayThanhToan.ToDateTime(new TimeOnly(0, 0)),
                    TongTien = h.TongTien,
                    TrangThai = "Đã thanh toán"
                }).ToList();

            // Gửi dữ liệu qua ViewBag
            ViewBag.DoanhThu = doanhThu;
            ViewBag.Labels = labels;
            ViewBag.DoanhThuData = doanhThuData;
            ViewBag.TienVonData = new List<decimal>(); // rỗng vì không dùng
            ViewBag.TraHang = 0;
            ViewBag.LoiNhuan = doanhThu; // nếu không tính vốn thì doanh thu = lợi nhuận
            ViewBag.DonHangList = donHangList;

            return View();
        }


    }
}
