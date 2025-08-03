using mi_pham_kem.Models;
using mi_pham_kem.Models.SQLServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace mi_pham_kem.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly MiPhamContext _context;
        public SanPhamController(MiPhamContext context) 
        { 
            _context = context;
        }
        public IActionResult ChiTiet(int id)
        {
            var sanPham = _context.SanPhams
            .Include(sp => sp.MaThNavigation)
            .Include(sp => sp.MaDmNavigation)
            .FirstOrDefault(sp => sp.MaSanPham == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            var danhGias = _context.DanhGia
                .Where(dg => dg.MaSanPham == id)
                .Include(dg => dg.MaKhNavigation)
                .OrderByDescending(dg => dg.DiemDanhGia)
                .ToList();

            double diemTrungBinh = danhGias.Count > 0 ? danhGias.Average(dg => dg.DiemDanhGia) : 0;

            ViewBag.DiemTrungBinh = diemTrungBinh;
            ViewBag.SoLuongDanhGia = danhGias.Count;
            ViewBag.DanhGias = danhGias;

            return View(sanPham);
        }
        [HttpPost]
        public IActionResult ThemDanhGia(DanhGium danhGia)
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            danhGia.MaKh = maKh.Value;
            danhGia.NgayTao = DateOnly.FromDateTime(DateTime.Now);


            _context.DanhGia.Add(danhGia);
            _context.SaveChanges();

            return RedirectToAction("ChiTiet", new { id = danhGia.MaSanPham });
        }
        
    }
}
