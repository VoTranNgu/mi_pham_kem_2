using mi_pham_kem.Models.SQLServer;
using mi_pham_kem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using X.PagedList.Extensions;

namespace mi_pham_kem.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly MiPhamContext _context;
        public TaiKhoanController(MiPhamContext context)
        {
            _context = context;
        }
        public IActionResult ThongTin(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            var kh = _context.KhachHangs.FirstOrDefault(k => k.MaKh == maKh);
            if (kh == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            var donHangs = _context.DonHangs
                .Where(dh => dh.MaKh == maKh)
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ct => ct.MaSanPhamNavigation)
                .OrderByDescending(dh => dh.NgayDat)
                .ToPagedList(pageNumber, pageSize);

            var viewModel = new ThongTinTaiKhoanViewModel
            {
                KhachHang = kh,
                DonHangs = donHangs
            };

            return View(viewModel);
        }

        public IActionResult SuaThongTin()
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            var kh = _context.KhachHangs.FirstOrDefault(k => k.MaKh == maKh);
            return View(kh);
        }

        [HttpPost]
        public IActionResult SuaThongTin(KhachHang model)
        {
            var kh = _context.KhachHangs.FirstOrDefault(k => k.MaKh == model.MaKh);
            if (kh != null)
            {
                kh.HoTen = model.HoTen;
                kh.Email = model.Email;
                kh.DiaChi = model.DiaChi;
                kh.Sdt = model.Sdt;
                _context.SaveChanges();
            }
            return RedirectToAction("ThongTin");
        }

        public IActionResult DoiMatKhau()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DoiMatKhau(string matKhauCu, string matKhauMoi, string xacNhanMatKhau)
        {
            string username = HttpContext.Session.GetString("NameUser");
            var user = _context.Users.FirstOrDefault(u => u.TenDangNhap == username);

            if (user == null || user.MatKhau != matKhauCu)
            {
                ViewBag.Error = "Mật khẩu cũ không đúng!";
                return View();
            }

            if (matKhauMoi != xacNhanMatKhau)
            {
                ViewBag.Error = "Xác nhận mật khẩu không khớp!";
                return View();
            }

            user.MatKhau = matKhauMoi;
            _context.SaveChanges();
            ViewBag.Message = "Đổi mật khẩu thành công!";
            return View();
        }

        public IActionResult ChiTietDonHang(int id)
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            var donHang = _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                .FirstOrDefault(dh => dh.MaDh == id && dh.MaKh == maKh);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        //huydon
        [HttpPost]
        public IActionResult HuyDonHang(int id)
        {
            var donHang = _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .FirstOrDefault(dh => dh.MaDh == id);

            if (donHang == null)
            {
                return NotFound();
            }

            //kiem tra trang thai
            if (donHang.TrangThai == "Đã giao hàng" || donHang.TrangThai == "Đã hoàn tất" || donHang.TrangThai == "Đang vận chuyển")
            {
                TempData["Error"] = "Không thể hủy đơn hàng này.";
                return RedirectToAction("ChiTietDonHang", new { id = id });
            }

            //xoa chi tiet giao hang
            _context.ChiTietDonHangs.RemoveRange(donHang.ChiTietDonHangs);

            //xoa don hang
            _context.DonHangs.Remove(donHang);
            _context.SaveChanges();

            TempData["Success"] = "Đơn hàng đã được hủy.";

            return RedirectToAction("ThongTin", "TaiKhoan");
        }

    }
}
