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


            var sanPhams = _context.SanPhams
                .Include(sp => sp.MaThNavigation)
                .Include(sp => sp.MaDmNavigation)
                .ToList();

            ViewBag.ThuongHieus = _context.ThuongHieus.ToList();
            ViewBag.DanhMucs = _context.DanhMucSps.ToList();

            if (_name == null)
                return View(sanPhams);
            else
                return View(_context.SanPhams.Where(s => s.TenSanPham.Contains(_name)).ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
