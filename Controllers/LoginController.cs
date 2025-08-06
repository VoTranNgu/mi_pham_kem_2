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
        private IConfiguration _config;

        public LoginController(MiPhamContext context, ILogger<HomeController> logger, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
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
                return View("DangNhap");
            }
            else
            {
                HttpContext.Session.SetString("NameUser", _user.TenDangNhap);

                HttpContext.Session.SetString("PasswordUser", _user.MatKhau);

                var khachHang = _context.KhachHangs
                    .Where(k => k.MaUser == check.MaUser)
                    .FirstOrDefault();

                if (khachHang != null)
                {
                    HttpContext.Session.SetInt32("MaKh", khachHang.MaKh);
                }

                if (check.Role == "user")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (check.Role == "AD")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.ErrorInfo = "Quyền truy cập không hợp lệ!";
                    return View("DangNhap");
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
            _user.Role = "user";
            if (string.IsNullOrEmpty(ConfirmPass))
            {
                ModelState.AddModelError("ConfirmPass", "Nhập lại mật khẩu còn trống!");
                return View();
            }
            if (_user.MatKhau != ConfirmPass)
            {
                ModelState.AddModelError("ConfirmPass", "Nhập lại mật khẩu không khớp!");
                ViewBag.ErrorInfo = "Nhập lại mật khẩu không khớp!";
                return View();
            }
            if (string.IsNullOrEmpty(_user.Sdt) || _user.Sdt.Length != 10 || !_user.Sdt.All(char.IsDigit))
            {
                ModelState.AddModelError("Sdt", "Số điện thoại phải gồm 10 chữ số!");
                ViewBag.ErrorInfo = "Số điện thoại phải gồm 10 chữ số!";
                return View();
            }
            var existingUser = _context.Users.FirstOrDefault(u => u.TenDangNhap == _user.TenDangNhap);
            if (existingUser != null)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã được sử dụng!");
                ViewBag.ErrorInfo = "Tên đăng nhập đã được sử dụng!";
                return View();
            }
            //_user.Role = "user";
            //_context.Users.Add(_user);
            //_context.SaveChanges();

            string otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("OTP_Expire", DateTime.Now.AddMinutes(5).ToString());

            //noi dung otp gui qua mail
            var emailService = new Services.EmailService(_config);
            emailService.SendEmail(
                _user.Email,
                "Mã xác thực OTP",
                $"<p>Xin chào <strong>{_user.HoTen}</strong>,</p>" +
                $"<p>Mã OTP của bạn là: <strong>{otp}</strong></p>" +
                $"<p>OTP sẽ hết hạn sau 5 phút.</p>"
            );
            //google email SMTP

            //luu tam thong tin user vao session
            HttpContext.Session.SetString("TempUser_TenDangNhap", _user.TenDangNhap);
            HttpContext.Session.SetString("TempUser_MatKhau", _user.MatKhau);
            HttpContext.Session.SetString("TempUser_HoTen", _user.HoTen);
            HttpContext.Session.SetString("TempUser_Email", _user.Email);
            HttpContext.Session.SetString("TempUser_Sdt", _user.Sdt);
            HttpContext.Session.SetString("TempUser_DiaChi", _user.DiaChi);

            return RedirectToAction("XacThucOTP");
            //KhachHang khachHang = new KhachHang
            //{
            //    HoTen = _user.HoTen,
            //    Sdt = _user.Sdt,
            //    Email = _user.Email,
            //    DiaChi = _user.DiaChi,
            //    MaUser = _user.MaUser
            //};
            //_context.KhachHangs.Add(khachHang);
            //_context.SaveChanges();

            //return RedirectToAction("DangNhap", "Login");

        }


        public IActionResult LogOutUser()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap", "Login");
        }

       
        public IActionResult XacThucOTP()
        {
            return View();
        }

        [HttpPost]
        public IActionResult XacThucOTP(string otpNhap)
        {
            var otpSession = HttpContext.Session.GetString("OTP");
            var otpExpire = HttpContext.Session.GetString("OTP_Expire");
            if (string.IsNullOrEmpty(otpSession) || DateTime.Now > DateTime.Parse(otpExpire))
            {
                ViewBag.ErrorInfo = "Mã OTP đã hết hạn. Vui lòng đăng ký lại.";
                return View("XacThucOTP");
            }
            if (otpNhap != otpSession)
            {
                ViewBag.ErrorInfo = "Mã OTP không chính xác!";
                return View("XacThucOTP");
            }

            //lay thong tin tu session
            var user = new User
            {
                TenDangNhap = HttpContext.Session.GetString("TempUser_TenDangNhap"),
                MatKhau = HttpContext.Session.GetString("TempUser_MatKhau"),
                HoTen = HttpContext.Session.GetString("TempUser_HoTen"),
                Email = HttpContext.Session.GetString("TempUser_Email"),
                Sdt = HttpContext.Session.GetString("TempUser_Sdt"),
                DiaChi = HttpContext.Session.GetString("TempUser_DiaChi"),
                Role = "user"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            //Tao khach hang moi
            KhachHang kh = new KhachHang
            {
                HoTen = user.HoTen,
                Sdt = user.Sdt,
                Email = user.Email,
                DiaChi = user.DiaChi,
                MaUser = user.MaUser
            };
            _context.KhachHangs.Add(kh);
            _context.SaveChanges();

            //xoa session
            HttpContext.Session.Clear();

            return RedirectToAction("DangNhap");
        }
        public IActionResult GuiLaiOTP()
        {
            var email = HttpContext.Session.GetString("TempUser_Email");
            var hoTen = HttpContext.Session.GetString("TempUser_HoTen");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("DangKy");
            }

            string otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("OTP_Expire", DateTime.Now.AddMinutes(5).ToString());

            var emailService = new Services.EmailService(_config);
            emailService.SendEmail(
                email,
                "Mã xác thực OTP (Gửi lại)",
                $"<p>Xin chào <strong>{hoTen}</strong>,</p>" +
                $"<p>Mã OTP mới của bạn là: <strong>{otp}</strong></p>" +
                $"<p>OTP sẽ hết hạn sau 5 phút.</p>"
            );

            TempData["SuccessMessage"] = "Mã OTP mới đã được gửi đến email của bạn.";
            return RedirectToAction("XacThucOTP");
        }

    }
}
