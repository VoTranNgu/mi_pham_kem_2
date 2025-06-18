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
    }

}
