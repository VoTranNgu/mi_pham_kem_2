using Humanizer;
using mi_pham_kem.Models;
using mi_pham_kem.Models.SQLServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace mi_pham_kem.Controllers
{
    public class GioHangController : Controller
    {
        private readonly MiPhamContext _context;
        private readonly ILogger<HomeController> _logger;

        public GioHangController(MiPhamContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GioHang()
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            var danhSach = _context.GioHangs
                .Where(g => g.MaKh == maKh)
                .Include(g => g.MaSanPhamNavigation)
                .ToList();

            if (danhSach == null || !danhSach.Any())
            {
                return View("GioHangTrong");
            }

            var tongTien = danhSach.Sum(g => g.SoLuong * g.MaSanPhamNavigation.Gia);
            ViewBag.TongTien = tongTien;

            //
            string maUuDai = HttpContext.Session.GetString("MaUuDai");

            if (!string.IsNullOrEmpty(maUuDai))
            {
                var uuDai = _context.UuDais.FirstOrDefault(u => u.NoiDungMa == maUuDai);
                if (uuDai != null && (!uuDai.ThoiHan.HasValue || uuDai.ThoiHan >= DateOnly.FromDateTime(DateTime.Now)))
                {
                    TempData["LoiUuDai"] = "Mã đã hết hạn sử dụng!";
                    if (!uuDai.GiaToiThieu.HasValue || tongTien >= uuDai.GiaToiThieu)
                    {
                        TempData["LoiUuDai"] = $"Đơn hàng chưa đủ điều kiện để áp mã, Vui lòng đặt hàng trên {uuDai.GiaToiThieu} để áp dụng mã này!";
                        ViewBag.GiaGiam = uuDai.GiaGiam;
                        ViewBag.MaUuDai = maUuDai;
                    }
                }
            }
            else
            {
                
                ViewBag.GiaGiam = 0;
            }

            return View(danhSach);
        }


        [HttpPost]
        public IActionResult Them(int maSanPham, int soLuong = 1)
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            var gioHang = _context.GioHangs
                .FirstOrDefault(g => g.MaKh == maKh && g.MaSanPham == maSanPham);

            if (gioHang != null)
            {
                gioHang.SoLuong += soLuong;
            }
            else
            {
                gioHang = new GioHang
                {
                    MaKh = maKh.Value,
                    MaSanPham = maSanPham,
                    SoLuong = soLuong
                };
                _context.GioHangs.Add(gioHang);
            }

            _context.SaveChanges();

            return RedirectToAction("GioHang");
        }
        [HttpPost]
        public IActionResult Xoa(int maSanPham)
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            var gioHang = _context.GioHangs
                .FirstOrDefault(g => g.MaKh == maKh && g.MaSanPham == maSanPham);

            if (gioHang != null)
            {
                _context.GioHangs.Remove(gioHang);
                _context.SaveChanges();
            }

            return RedirectToAction("GioHang");
        }
        public IActionResult GioHangTrong()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CapNhatSoLuong(int maSanPham, string hanhDong)
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            var gioHang = _context.GioHangs
                .FirstOrDefault(g => g.MaKh == maKh && g.MaSanPham == maSanPham);

            if (gioHang != null)
            {
                if (hanhDong == "Tang")
                {
                    gioHang.SoLuong++;
                }
                else if (hanhDong == "Giam")
                {
                    gioHang.SoLuong--;
                    if (gioHang.SoLuong <= 0)
                    {
                        _context.GioHangs.Remove(gioHang);
                    }
                }

                _context.SaveChanges();
            }

            return RedirectToAction("GioHang");
        }
        [HttpPost]
        public IActionResult XoaTatCa()
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            var gioHang = _context.GioHangs.Where(g => g.MaKh == maKh).ToList();

            if (gioHang.Any())
            {
                _context.GioHangs.RemoveRange(gioHang);
                _context.SaveChanges();
            }

            return RedirectToAction("GioHang");
        }
        [HttpPost]
        public IActionResult ApDungUuDai(string maUuDai)
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            var danhSach = _context.GioHangs
                .Where(g => g.MaKh == maKh)
                .Include(g => g.MaSanPhamNavigation)
                .ToList();

            if (danhSach == null || !danhSach.Any())
            {
                return View("GioHangTrong");
            }

            var tongTien = danhSach.Sum(g => g.SoLuong * g.MaSanPhamNavigation.Gia);
            ViewBag.TongTien = tongTien;

            var uuDai = _context.UuDais.FirstOrDefault(u => u.NoiDungMa == maUuDai);

            if (uuDai == null || (uuDai.ThoiHan.HasValue && uuDai.ThoiHan < DateOnly.FromDateTime(DateTime.Now)))
            {
                TempData["LoiUuDai"] = "Mã không hợp lệ hoặc đã hết hạn.";
                ViewBag.GiaGiam = 0;
            }
            else if (uuDai.GiaToiThieu.HasValue && tongTien < uuDai.GiaToiThieu)
            {
                TempData["LoiUuDai"] = $"Mã chỉ áp dụng cho đơn từ {uuDai.GiaToiThieu:N0} đ.";
                ViewBag.GiaGiam = 0;
            }
            else
            {
                // kiểm tra tình trạng đã dùng
                bool daDung = _context.UuDaiKhachHangs.Any(x =>
                    x.MaKh == maKh && x.MaUd == uuDai.MaUd && x.DaSuDung == true);

                if (daDung)
                {
                    TempData["LoiUuDai"] = "Bạn đã sử dụng mã này rồi.";
                    ViewBag.GiaGiam = 0;
                }
                else
                {
                    ViewBag.GiaGiam = uuDai.GiaGiam;
                    ViewBag.MaUuDai = uuDai.NoiDungMa; // thêm dòng này
                    TempData["ThongBaoUuDai"] = "Áp dụng mã thành công!";
                    HttpContext.Session.SetString("MaUuDai", maUuDai); // lưu lại trạng thái dùng
                }
            }
            return View("GioHang", danhSach);
        }


    }

}
