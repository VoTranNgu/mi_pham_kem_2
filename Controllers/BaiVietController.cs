using mi_pham_kem.Models.SQLServer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace mi_pham_kem.Controllers
{
    public class BaiVietController : Controller
    {
        private readonly MiPhamContext _context;
        public BaiVietController(MiPhamContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var dsBaiViet = _context.BaiViets
                .OrderByDescending(b => b.NgayDang)
                .ToList();
            return View(dsBaiViet);
        }
        public IActionResult ChiTiet(int id)
        {
            var baiViet = _context.BaiViets.FirstOrDefault(b => b.Id == id);
            if (baiViet == null)
            {
                return NotFound();
            }
            return View(baiViet);
        }
        public IActionResult VeChungToi()
        {
            return View();
        }
    }
}

