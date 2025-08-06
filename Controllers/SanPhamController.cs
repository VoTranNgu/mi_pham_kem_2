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
        public IActionResult ChiTiet(int id, int page = 1)
        {
            int pageSize = 4;

            var sanPham = _context.SanPhams
                .Include(sp => sp.MaThNavigation)
                .Include(sp => sp.MaDmNavigation)
                .FirstOrDefault(sp => sp.MaSanPham == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            var sanPhamLienQuan = _context.SanPhams
                .Where(sp => sp.MaDm == sanPham.MaDm && sp.MaSanPham != id)
                .Take(4)
                .ToList();

            var allDanhGias = _context.DanhGia
                .Where(dg => dg.MaSanPham == id)
                .Include(dg => dg.MaKhNavigation)
                .OrderByDescending(dg => dg.NgayTao)
                .ToList();

            double diemTrungBinh = allDanhGias.Count > 0 ? allDanhGias.Average(dg => dg.DiemDanhGia) : 0;

            var danhGiasPaged = allDanhGias
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.DiemTrungBinh = diemTrungBinh;
            ViewBag.SoLuongDanhGia = allDanhGias.Count;
            ViewBag.DanhGias = danhGiasPaged;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)allDanhGias.Count / pageSize);

            int? maKh = HttpContext.Session.GetInt32("MaKh");
            bool choPhep = false;
            if (maKh != null)
            {
                choPhep = _context.DonHangs
                    .Include(dh => dh.ChiTietDonHangs)
                    .Any(dh => dh.MaKh == maKh
                               && dh.TrangThai == "Đã hoàn tất"
                               && dh.ChiTietDonHangs.Any(ct => ct.MaSanPham == id));
            }

            ViewBag.ChoPhepDanhGia = choPhep;

            var viewModel = new ChiTietSanPhamViewModel
            {
                SanPham = sanPham,
                SanPhamLienQuan = sanPhamLienQuan
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ThemDanhGia(int MaSanPham, int DiemDanhGia, string BinhLuan)
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }
            var danhGia = new DanhGium
            {
                MaSanPham = MaSanPham,
                DiemDanhGia = DiemDanhGia,
                BinhLuan = BinhLuan,
                NgayTao = DateOnly.FromDateTime(DateTime.Now),
                MaKh = maKh.Value,
            };

            _context.DanhGia.Add(danhGia);
            _context.SaveChanges();

            return RedirectToAction("ChiTiet", new { id = MaSanPham });
        }

        //[HttpPost]
        //public IActionResult ThemDanhGia(DanhGium danhGia)
        //{
        //    int? maKh = HttpContext.Session.GetInt32("MaKh");
        //    if (maKh == null)
        //    {
        //        return RedirectToAction("DangNhap", "Login");
        //    }

        //    danhGia.MaKh = maKh.Value;
        //    danhGia.NgayTao = DateOnly.FromDateTime(DateTime.Now);


        //    _context.DanhGia.Add(danhGia);
        //    _context.SaveChanges();

        //    return RedirectToAction("ChiTiet", new { id = danhGia.MaSanPham });
        //}

    }
}
