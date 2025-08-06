using Humanizer;
using mi_pham_kem.Models;
using mi_pham_kem.Models.SQLServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace mi_pham_kem.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly MiPhamContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly PayOSService _payOS;
        public ThanhToanController(MiPhamContext context, ILogger<HomeController> logger, PayOSService payOS)
        {
            _context = context;
            _logger = logger;
            _payOS = payOS;
        }

        public IActionResult XacNhanDon()
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

            if (!danhSach.Any())
            {
                return View("GioHangTrong");
            }

            var khachHang = _context.KhachHangs.FirstOrDefault(k => k.MaKh == maKh);
            if (khachHang != null)
            {
                ViewBag.DiaChi = khachHang.DiaChi;
                ViewBag.SoDienThoai = khachHang.Sdt;
            }

            var tongTien = danhSach.Sum(g => g.SoLuong * g.MaSanPhamNavigation.Gia);
            ViewBag.TongTien = tongTien;

            string? maUuDai = HttpContext.Session.GetString("MaUuDai");
            ViewBag.MaUuDai = maUuDai;

            decimal giaGiam = 0;
            if (!string.IsNullOrEmpty(maUuDai))
            {
                var uuDai = _context.UuDais.FirstOrDefault(u => u.NoiDungMa == maUuDai);
                if (uuDai != null && (!uuDai.ThoiHan.HasValue || uuDai.ThoiHan >= DateOnly.FromDateTime(DateTime.Now)))
                {
                    if (!uuDai.GiaToiThieu.HasValue || tongTien >= uuDai.GiaToiThieu)
                    {
                        giaGiam = uuDai.GiaGiam;
                    }
                }
            }

            ViewBag.GiaGiam = giaGiam;
            return View(danhSach);
        }

        [HttpPost]
        public IActionResult ApDungUuDaiTaiXacNhan(string maUuDai)
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
                return RedirectToAction("DangNhap", "Login");

            var danhSach = _context.GioHangs
                .Where(g => g.MaKh == maKh)
                .Include(g => g.MaSanPhamNavigation)
                .ToList();

            var tongTien = danhSach.Sum(g => g.SoLuong * g.MaSanPhamNavigation.Gia);
            var uuDai = _context.UuDais.FirstOrDefault(u => u.NoiDungMa == maUuDai);

            if (uuDai == null || (uuDai.ThoiHan.HasValue && uuDai.ThoiHan < DateOnly.FromDateTime(DateTime.Now)))
            {
                TempData["LoiUuDai"] = "Mã không hợp lệ hoặc đã hết hạn.";
            }
            else if (uuDai.GiaToiThieu.HasValue && tongTien < uuDai.GiaToiThieu)
            {
                TempData["LoiUuDai"] = $"Mã chỉ áp dụng cho đơn từ {uuDai.GiaToiThieu:N0} đ.";
            }
            else
            {
                //kiem tra tinh trang su dung
                bool daDung = _context.UuDaiKhachHangs
                    .Any(x => x.MaKh == maKh && x.MaUd == uuDai.MaUd && x.DaSuDung == true);

                if (daDung)
                {
                    TempData["LoiUuDai"] = "Bạn đã sử dụng mã này rồi.";
                }
                else
                {
                    HttpContext.Session.SetString("MaUuDai", maUuDai);
                    TempData["ThongBaoUuDai"] = "Áp dụng mã thành công!";
                }
            }

            return RedirectToAction("XacNhanDon");
        }
        
        [HttpPost]
        public IActionResult DatHang()
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null) return RedirectToAction("DangNhap", "Login");

            var gioHang = _context.GioHangs
                .Where(g => g.MaKh == maKh)
                .Include(g => g.MaSanPhamNavigation)
                .ToList();

            if (!gioHang.Any()) return RedirectToAction("GioHang", "GioHang");

            decimal tongTienSanPham = gioHang.Sum(g => g.SoLuong * g.MaSanPhamNavigation.Gia);
            decimal giamGia = 0;
            int? maUd = null;

            // Kiểm tra mã ưu đãi (nếu có)
            string? maUuDai = HttpContext.Session.GetString("MaUuDai");
            if (!string.IsNullOrEmpty(maUuDai))
            {
                var uuDai = _context.UuDais.FirstOrDefault(u => u.NoiDungMa == maUuDai);
                if (uuDai != null && (!uuDai.ThoiHan.HasValue || uuDai.ThoiHan >= DateOnly.FromDateTime(DateTime.Now)))
                {
                    if (!uuDai.GiaToiThieu.HasValue || tongTienSanPham >= uuDai.GiaToiThieu)
                    {
                        giamGia = uuDai.GiaGiam;
                        maUd = uuDai.MaUd;
                    }
                }
            }

            // Tổng tiền đơn hàng cuối cùng = tổng sản phẩm - giảm giá + phí ship
            decimal tongTienDonHang = tongTienSanPham - giamGia + 30000;

            // Tạo đơn hàng
            var donHang = new DonHang
            {
                MaKh = maKh.Value,
                NgayDat = DateOnly.FromDateTime(DateTime.Now),
                TongTien = tongTienDonHang,
                TrangThai = "Đã xác nhận",
                MaHd = null,
                MaUd = maUd
            };

            _context.DonHangs.Add(donHang);
            _context.SaveChanges(); // Save sớm để có MaDh

            // Lưu chi tiết đơn hàng
            foreach (var item in gioHang)
            {
                var chiTiet = new ChiTietDonHang
                {
                    MaDh = donHang.MaDh,
                    MaSanPham = item.MaSanPham,
                    SoLuong = item.SoLuong,
                    DonGia = item.MaSanPhamNavigation.Gia
                };
                _context.ChiTietDonHangs.Add(chiTiet);
            }

            // Nếu dùng mã ưu đãi thì đánh dấu đã dùng
            if (maUd.HasValue)
            {
                var daDung = new UuDaiKhachHang
                {
                    MaKh = maKh.Value,
                    MaUd = maUd.Value,
                    MaDh = donHang.MaDh,
                    DaSuDung = true
                };
                _context.UuDaiKhachHangs.Add(daDung);
            }

            // Xoá giỏ hàng
            _context.GioHangs.RemoveRange(gioHang);

            // Lưu tất cả thay đổi
            _context.SaveChanges();
            HttpContext.Session.SetInt32("MaDonHangVuaDat", donHang.MaDh);

            HttpContext.Session.Remove("MaUuDai");
            HttpContext.Session.Remove("GiaGiam");

            return RedirectToAction("ThanhToanThanhCong", new { id = donHang.MaDh });

        }



        public IActionResult ThanhToanThanhCong()
        {
            return View();
        }

        public IActionResult ChiTietDonHangCuoi()
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null) return RedirectToAction("DangNhap", "Login");

            int? maDh = HttpContext.Session.GetInt32("MaDonHangVuaDat");
            if (maDh == null) return RedirectToAction("Index", "Home");

            var donHang = _context.DonHangs
                .Where(d => d.MaKh == maKh && d.MaDh == maDh)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.MaSanPhamNavigation)
                .FirstOrDefault();

            if (donHang == null)
                return RedirectToAction("Index", "Home");

            if (donHang.MaUd != null)
            {
                var uuDai = _context.UuDais.FirstOrDefault(u => u.MaUd == donHang.MaUd);
                if (uuDai != null)
                {
                    ViewBag.TenMaUuDai = uuDai.NoiDungMa;
                }
            }

            return View("ChiTietDonHangCuoi", donHang);

        }

        public IActionResult ThanhToanVnPay()
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
                return RedirectToAction("DangNhap", "Login");

            var danhSach = _context.GioHangs
                .Where(g => g.MaKh == maKh)
                .Include(g => g.MaSanPhamNavigation)
                .ToList();

            if (!danhSach.Any())
                return RedirectToAction("GioHang", "GioHang");

            decimal tongTienSanPham = danhSach.Sum(g => g.SoLuong * g.MaSanPhamNavigation.Gia);
            decimal giamGia = 0;

            string? maUuDai = HttpContext.Session.GetString("MaUuDai");
            if (!string.IsNullOrEmpty(maUuDai))
            {
                var uuDai = _context.UuDais.FirstOrDefault(u => u.NoiDungMa == maUuDai);
                if (uuDai != null && (!uuDai.ThoiHan.HasValue || uuDai.ThoiHan >= DateOnly.FromDateTime(DateTime.Now)))
                {
                    if (!uuDai.GiaToiThieu.HasValue || tongTienSanPham >= uuDai.GiaToiThieu)
                    {
                        giamGia = uuDai.GiaGiam;
                    }
                }
            }

            decimal tongTienSauGiam = tongTienSanPham - giamGia + 30000;
            ViewBag.TongTien = tongTienSauGiam;

            return View();
        }


        //Thanh toan PayOS
        public async Task<IActionResult> ThanhToanPayOS()
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null) return RedirectToAction("DangNhap", "Login");

            var danhSach = _context.GioHangs
                .Where(g => g.MaKh == maKh)
                .Include(g => g.MaSanPhamNavigation)
                .ToList();

            if (!danhSach.Any()) return RedirectToAction("GioHang", "GioHang");

            decimal tongTienSanPham = danhSach.Sum(g => g.SoLuong * g.MaSanPhamNavigation.Gia);
            decimal giamGia = 0;

            string? maUuDai = HttpContext.Session.GetString("MaUuDai");
            if (!string.IsNullOrEmpty(maUuDai))
            {
                var uuDai = _context.UuDais.FirstOrDefault(u => u.NoiDungMa == maUuDai);
                if (uuDai != null && (!uuDai.ThoiHan.HasValue || uuDai.ThoiHan >= DateOnly.FromDateTime(DateTime.Now)))
                {
                    if (!uuDai.GiaToiThieu.HasValue || tongTienSanPham >= uuDai.GiaToiThieu)
                    {
                        giamGia = uuDai.GiaGiam;
                    }
                }
            }

            decimal tongTienSauGiam = tongTienSanPham - giamGia;

            // Danh sách sản phẩm
            var items = danhSach.Select(sp => new ItemData(
                sp.MaSanPhamNavigation.TenSanPham,
                sp.SoLuong,
                (int)sp.MaSanPhamNavigation.Gia
            )).ToList();

            // Mã đơn hàng duy nhất
            long orderCode = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
            string moTa = "Don hang #" + orderCode;

            string returnUrl = Url.Action("PayOSCallback", "ThanhToan", null, Request.Scheme);
            string cancelUrl = Url.Action("XacNhanDon", "ThanhToan", null, Request.Scheme);

            var result = await _payOS.TaoLinkThanhToan(orderCode, (int)tongTienSauGiam, "Thanh toan Chella", items, returnUrl, cancelUrl);

            if (result != null)
            {
                // Lưu tạm thông tin đơn hàng vào session để sau khi thanh toán sẽ lưu vào DB
                HttpContext.Session.SetString("ThanhToan_OrderCode", orderCode.ToString());
                HttpContext.Session.SetInt32("ThanhToan_TongTien", (int)tongTienSauGiam);
                return Redirect(result.checkoutUrl);
            }

            TempData["LoiThanhToan"] = "Không thể tạo liên kết thanh toán.";
            return RedirectToAction("XacNhanDon");
        }
        public IActionResult PayOSCallback()
        {
            return View("PayOSAutoSubmit");
        }
        public IActionResult PayOSReturn()
        {
            return RedirectToAction("DonHang");
        }

        public IActionResult PayOSCancel()
        {
            TempData["LoiThanhToan"] = "Bạn đã hủy thanh toán.";
            return RedirectToAction("XacNhanDon");
        }

        //xu ly khi thanh toan payOS
        //dat hang thanh cong
        [HttpPost]
        public IActionResult DatHangPayOS()
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null) return RedirectToAction("DangNhap", "Login");

            var gioHang = _context.GioHangs
                .Where(g => g.MaKh == maKh)
                .Include(g => g.MaSanPhamNavigation)
                .ToList();

            if (!gioHang.Any()) return RedirectToAction("GioHang", "GioHang");

            decimal tongTienSanPham = gioHang.Sum(g => g.SoLuong * g.MaSanPhamNavigation.Gia);
            decimal giamGia = 0;
            int? maUd = null;

            // Kiểm tra mã ưu đãi (nếu có)
            string? maUuDai = HttpContext.Session.GetString("MaUuDai");
            if (!string.IsNullOrEmpty(maUuDai))
            {
                var uuDai = _context.UuDais.FirstOrDefault(u => u.NoiDungMa == maUuDai);
                if (uuDai != null && (!uuDai.ThoiHan.HasValue || uuDai.ThoiHan >= DateOnly.FromDateTime(DateTime.Now)))
                {
                    if (!uuDai.GiaToiThieu.HasValue || tongTienSanPham >= uuDai.GiaToiThieu)
                    {
                        giamGia = uuDai.GiaGiam;
                        maUd = uuDai.MaUd;
                    }
                }
            }

            // Tổng tiền đơn hàng cuối cùng = tổng sản phẩm - giảm giá + phí ship
            decimal tongTienDonHang = tongTienSanPham - giamGia + 30000;
            var hoaDon = new HoaDon
            {
                NgayThanhToan = DateOnly.FromDateTime(DateTime.Now),
                TongTien = tongTienDonHang,
                MaKh = maKh.Value
            };
            _context.HoaDons.Add(hoaDon);
            _context.SaveChanges();
            // Tạo đơn hàng
            var donHang = new DonHang
            {
                MaKh = maKh.Value,
                NgayDat = DateOnly.FromDateTime(DateTime.Now),
                TongTien = tongTienDonHang,
                TrangThai = "Đã xác nhận",
                MaHd = hoaDon.MaHd,
                MaUd = maUd
            };

            _context.DonHangs.Add(donHang);
            _context.SaveChanges();

            // Lưu chi tiết đơn hàng
            foreach (var item in gioHang)
            {
                var chiTiet = new ChiTietDonHang
                {
                    MaDh = donHang.MaDh,
                    MaSanPham = item.MaSanPham,
                    SoLuong = item.SoLuong,
                    DonGia = item.MaSanPhamNavigation.Gia
                };
                _context.ChiTietDonHangs.Add(chiTiet);
            }

            // Nếu dùng mã ưu đãi thì đánh dấu đã dùng
            if (maUd.HasValue)
            {
                var daDung = new UuDaiKhachHang
                {
                    MaKh = maKh.Value,
                    MaUd = maUd.Value,
                    MaDh = donHang.MaDh,
                    DaSuDung = true
                };
                _context.UuDaiKhachHangs.Add(daDung);
            }

            // Xoá giỏ hàng
            _context.GioHangs.RemoveRange(gioHang);

            // Lưu tất cả thay đổi
            _context.SaveChanges();
            HttpContext.Session.SetInt32("MaDonHangVuaDat", donHang.MaDh);

            HttpContext.Session.Remove("MaUuDai");
            HttpContext.Session.Remove("GiaGiam");

            return RedirectToAction("ThanhToanThanhCongPayOS", new { id = donHang.MaDh });

        }

        public IActionResult ThanhToanThanhCongPayOS()
        {
            return View();
        }

        //chitiethoadon
        public IActionResult InHoaDon()
        {
            int? maKh = HttpContext.Session.GetInt32("MaKh");
            int? maDh = HttpContext.Session.GetInt32("MaDonHangVuaDat");
            if (maKh == null || maDh == null)
                return RedirectToAction("Index", "Home");

            var donHang = _context.DonHangs
                .Where(d => d.MaDh == maDh && d.MaKh == maKh)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.MaSanPhamNavigation)
                .Include(d => d.MaHdNavigation)
                .FirstOrDefault();

            if (donHang == null)
                return RedirectToAction("Index", "Home");

            return View("InHoaDon", donHang);
        }

    }

}
