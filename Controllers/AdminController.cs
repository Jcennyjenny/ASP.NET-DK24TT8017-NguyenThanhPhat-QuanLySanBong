using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using QuanLySanBong.Models; // <-- Đảm bảo dòng này đúng tên dự án của bạn

namespace QuanLySanBong.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly QuanLySanBongContext _context;

        public AdminController(QuanLySanBongContext context)
        {
            _context = context;
        }

        // 1. TRANG TRẠNG THÁI SÂN TRỐNG / KÍN
        public IActionResult TinhTrangSanHienTai(DateTime? ngayXem)
        {
            // 1. Kiểm tra ngày xem lịch
            DateTime ngayHienTai = ngayXem ?? DateTime.Today;
            ViewBag.NgayChon = ngayHienTai.ToString("yyyy-MM-dd");

            // Chuyển đổi DateTime sang DateOnly để so sánh với Database
            DateOnly ngayDateOnly = DateOnly.FromDateTime(ngayHienTai);

            // 2. Lấy danh sách sân bóng đang sẵn sàng hoạt động
            var danhSachSan = _context.Set<SanBong>().Where(s => s.TrangThai == "Sẵn sàng").ToList();
            ViewBag.CacSan = danhSachSan;

            // 3. Truy vấn lịch bận (Đã sửa lỗi so sánh DateOnly)
            var lichBans = _context.Set<DatLich>()
                .Include(d => d.MaNguoiDungNavigation)
                .Where(d => d.NgayDa == ngayDateOnly && d.TrangThai != "Đã hủy")
                .ToList();

            return View(lichBans);
        }

        // 2. TRANG DANH SÁCH SÂN BÓNG
        public IActionResult DanhSachSan()
        {
            var dsSan = _context.Set<SanBong>().ToList();
            return View(dsSan);
        }

        [HttpPost]
        public IActionResult ThemSan(SanBong model)
        {
            if (ModelState.IsValid)
            {
                _context.Set<SanBong>().Add(model);
                _context.SaveChanges();
            }
            return RedirectToAction("DanhSachSan");
        }

        public IActionResult XoaSan(int id)
        {
            var san = _context.Set<SanBong>().Find(id);
            if (san != null)
            {
                _context.Set<SanBong>().Remove(san);
                _context.SaveChanges();
            }
            return RedirectToAction("DanhSachSan");
        }

        // 3. TRANG DANH SÁCH KHÁCH HÀNG
        public IActionResult DanhSachKhachHang()
        {
            var dsKhach = _context.Set<NguoiDung>().Where(u => u.VaiTro == "KhachHang").ToList();
            return View(dsKhach);
        }
        // ================= 4. DUYỆT HOẶC HỦY LỊCH ĐẶT =================
        [HttpPost]
        public IActionResult DuyetLich(int maDatLich)
        {
            var lich = _context.Set<DatLich>().Find(maDatLich);
            if (lich != null)
            {
                lich.TrangThai = "Đã xác nhận";
                _context.SaveChanges();
            }
            return RedirectToAction("TinhTrangSanHienTai");
        }

        [HttpPost]
        public IActionResult HuyLich(int maDatLich)
        {
            var lich = _context.Set<DatLich>().Find(maDatLich);
            if (lich != null)
            {
                lich.TrangThai = "Đã hủy";
                _context.SaveChanges();
            }
            return RedirectToAction("TinhTrangSanHienTai");
        }
    }

}
