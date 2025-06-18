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
            return View();
        }
        [HttpPost]
        public IActionResult GioHang(DonHang don)
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH != null)
            {
                don.MaKh = maKH.Value;
                don.NgayDat = DateOnly.FromDateTime(DateTime.Now);
                don.TrangThai = "Đã đặt";

                _context.DonHangs.Add(don);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("DonHang", don.MaDh);
                return RedirectToAction("GioHang","GioHang");
            }
            else
            {
                return Content("Không thể thêm sản phẩm, vui lòng đăng nhập.");
            }

        }
        public IActionResult GioHangTrong()
        {
            return View();
        }
    }
}
