using mi_pham_kem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using mi_pham_kem.Models.SQLServer;

namespace mi_pham_kem.Controllers
{
    public class HomeController : Controller
    {
        private readonly MiPhamContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(MiPhamContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }


        public IActionResult Index()
        {
            var sanPhams = _context.SanPhams
            .Include(sp => sp.MaThNavigation)
            .Include(sp => sp.MaDmNavigation)
            .ToList();

            var sanPhamMoi = _context.SanPhams
                .OrderByDescending(sp => sp.MaSanPham) //lay tu moi nhat
                .Take(5)//lay 5 mon
                .ToList();

            var sanPhamBanChay = _context.ChiTietDonHangs
                .GroupBy(ct => ct.MaSanPham)
                .Select(g => new
                {
                    MaSanPham = g.Key,
                    TongSoLuong = g.Sum(ct => ct.SoLuong)
                })
                .OrderByDescending(x => x.TongSoLuong)
                .Take(5)
                .Join(_context.SanPhams
                .Include(sp => sp.MaThNavigation)
                .Include(sp => sp.MaDmNavigation),
                    g => g.MaSanPham,
                    sp => sp.MaSanPham,
                    (g, sp) => sp)
                .ToList();

            var danhGias = _context.DanhGia.ToList();

            ViewBag.SanPhamBanChay = sanPhamBanChay;
            ViewBag.SanPhamMoi = sanPhamMoi;
            ViewBag.DanhGias = danhGias;
            ViewBag.ThuongHieus = _context.ThuongHieus.ToList();
            ViewBag.DanhMucs = _context.DanhMucSps.ToList();

            return View(sanPhams);
        }
        
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Product(string _name)
        {
            var query = _context.SanPhams
                .Include(sp => sp.MaThNavigation)
                .Include(sp => sp.MaDmNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(_name))
            {
                query = query.Where(sp => sp.TenSanPham.Contains(_name));
            }

            var sanPhams = query.ToList();

            ViewBag.DanhGias = _context.DanhGia.ToList();
            ViewBag.ThuongHieus = _context.ThuongHieus.ToList();
            ViewBag.DanhMucs = _context.DanhMucSps.ToList();

            return View(sanPhams);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SanPhamTheoThuongHieu(int id)
        {
            var sanPhams = _context.SanPhams
                .Include(sp => sp.MaThNavigation)
                .Include(sp => sp.MaDmNavigation)
                .Where(sp => sp.MaTh == id)
                .ToList();

            return PartialView("_DanhSachSanPhamPartial", sanPhams);
        }

        public IActionResult SanPhamTheoDanhMuc(int id)
        {
            var sanPhams = _context.SanPhams
                .Include(sp => sp.MaThNavigation)
                .Include(sp => sp.MaDmNavigation)
                .Where(sp => sp.MaDm == id)
                .ToList();

            return PartialView("_DanhSachSanPhamPartial", sanPhams);
        }
        public IActionResult DMvaTH()
        {
            var thuongHieuList = _context.ThuongHieus
                .Select(th => new
                {
                    th.MaTh,
                    th.TenThuongHieu,
                    HinhAnh = _context.SanPhams
                        .Where(sp => sp.MaTh == th.MaTh && !string.IsNullOrEmpty(sp.HinhAnh))
                        .Select(sp => sp.HinhAnh)
                        .FirstOrDefault() ?? "/images/default.jpg"
                }).ToList();

            var danhMucList = _context.DanhMucSps
                .Select(dm => new
                {
                    dm.MaDm,
                    dm.TenDanhMuc,
                    HinhAnh = _context.SanPhams
                        .Where(sp => sp.MaDm == dm.MaDm && !string.IsNullOrEmpty(sp.HinhAnh))
                        .Select(sp => sp.HinhAnh)
                        .FirstOrDefault() ?? "/images/default.jpg"
                }).ToList();

            ViewBag.ThuongHieus = thuongHieuList;
            ViewBag.DanhMucs = danhMucList;
            return View();
        }

        public IActionResult UuDai()
        {

            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }


            var maUdDaSuDung = _context.UuDaiKhachHangs
                .Where(u => u.MaKh == maKh && u.DaSuDung == true)
                .Select(u => u.MaUd)
                .ToList();


            var uuDais = _context.UuDais
                .Where(ud => !maUdDaSuDung.Contains(ud.MaUd)
                             && ud.ThoiHan >= DateOnly.FromDateTime(DateTime.Today))
                .ToList();

            return View(uuDais);
        }

    }
}
