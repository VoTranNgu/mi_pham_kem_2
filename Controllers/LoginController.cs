using mi_pham_kem.Models;
using mi_pham_kem.Models.SQLServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace mi_pham_kem.Controllers
{
    public class LoginController : Controller
    {
        private readonly MiPhamContext _context;
        private readonly ILogger<HomeController> _logger;

        public LoginController(MiPhamContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult DangNhap()
        {

            return View();
        }
        [HttpPost]
        public IActionResult DangNhap(User _user)
        {
            var check = _context.Users
                .Where(s => s.TenDangNhap == _user.TenDangNhap && s.MatKhau == _user.MatKhau)
                .FirstOrDefault();

            if (check == null)
            {
                ModelState.AddModelError("LoginError", "Thông tin đăng nhập không đúng!");
                ViewBag.ErrorInfo = "Thông tin đăng nhập không đúng!";
                return View("Index");
            }
            else
            {
                HttpContext.Session.SetString("NameUser", _user.TenDangNhap);

                HttpContext.Session.SetString("PasswordUser", _user.MatKhau);

                // Lấy MaKH từ bảng KhachHang thông qua TaiKhoan
                var khachHang = _context.KhachHangs
                    .Where(k => k.MaUser == check.MaUser)
                    .FirstOrDefault();

                if (khachHang != null)
                {
                    HttpContext.Session.SetInt32("MaKh", khachHang.MaKh);
                }

                // Kiểm tra quyền và điều hướng tới trang phù hợp
                if (check.Role == "user")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (check.Role == "NV")
                {
                    return RedirectToAction("QuanLyChinh", "QuanLy");
                }
                else if (check.Role == "AD")
                {
                    return RedirectToAction("QLAdmin", "QuanLy");
                }
                else
                {
                    ViewBag.ErrorInfo = "Quyền truy cập không hợp lệ!";
                    return View("Index");
                }
            }
        }
        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DangKy(User _user, string ConfirmPass)
        {
            if (_user.MatKhau != ConfirmPass)
            {
                ModelState.AddModelError("ConfirmPass", "Mật khẩu không khớp");
                return View();
            }

            _user.Role = "user";
            _context.Users.Add(_user);
            _context.SaveChanges();

            KhachHang khachHang = new KhachHang
            {
                HoTen = _user.HoTen,
                Sdt = _user.Sdt,
                Email = _user.Email,
                DiaChi = _user.DiaChi,
                MaUser = _user.MaUser 
            };
            _context.KhachHangs.Add(khachHang);
            _context.SaveChanges();

            return RedirectToAction("DangNhap", "Login");

        }


        public IActionResult LogOutUser()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap", "Login");
        }

    }
}
