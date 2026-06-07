using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using QuanLySanBong.Models;

namespace QuanLySanBong.Controllers
{
    // Chỉ những người dùng đã đăng nhập (cả khách hàng) mới được truy cập
    [Authorize]
    public class KhachHangController : Controller
    {
        private readonly QuanLySanBongContext _context;

        public KhachHangController(QuanLySanBongContext context)
        {
            _context = context;
        }

        // ================= 1. GIAO DIỆN TÌM SÂN TRỐNG =================
        [HttpGet]
        public IActionResult TrangTimSan()
        {
            // Mặc định hiển thị ngày hôm nay khi vừa vào trang
            ViewBag.NgayDa = DateTime.Today.ToString("yyyy-MM-dd");
            return View();
        }

        [HttpPost]
        public IActionResult TimSanTrong(DateTime ngayDa, string gioBatDau, string gioKetThuc)
        {
            // Chuyển đổi dữ liệu chuỗi từ View sang định dạng TimeOnly và DateOnly để khớp Database
            DateOnly ngayCheck = DateOnly.FromDateTime(ngayDa);
            TimeOnly batDauCheck = TimeOnly.Parse(gioBatDau);
            TimeOnly ketThucCheck = TimeOnly.Parse(gioKetThuc);

            // Thuật toán: Tìm ID của những sân bóng ĐÃ CÓ người đặt trùng/đè lên khung giờ khách chọn
            var sanBiTrungIds = _context.Set<DatLich>()
                .Where(d => d.NgayDa == ngayCheck && d.TrangThai != "Đã hủy" &&
                      ((batDauCheck >= d.GioBatDau && batDauCheck < d.GioKetThuc) ||
                       (ketThucCheck > d.GioBatDau && ketThucCheck <= d.GioKetThuc) ||
                       (batDauCheck <= d.GioBatDau && ketThucCheck >= d.GioKetThuc)))
                .Select(d => d.MaSan)
                .ToList();

            // Lấy ra danh sách các sân TRỐNG (Những sân không nằm trong list bị trùng ở trên)
            var danhSachSanTrong = _context.Set<SanBong>()
                .Where(s => !sanBiTrungIds.Contains(s.MaSan) && s.TrangThai == "Sẵn sàng")
                .ToList();

            // Gửi ngược lại thông tin tìm kiếm ra View để hiển thị nút Đặt Sân
            ViewBag.NgayDaStr = ngayDa.ToString("yyyy-MM-dd");
            ViewBag.GioBatDauStr = gioBatDau;
            ViewBag.GioKetThucStr = gioKetThuc;

            return View("TrangTimSan", danhSachSanTrong);
        }

        // ================= 2. XỬ LÝ ĐẶT SÂN BÓNG =================
        [HttpPost]
        public IActionResult XulyDatSan(int maSan, string ngayDa, string gioBatDau, string gioKetThuc)
        {
            // Lấy ID của Khách hàng đang đăng nhập từ Cookie hệ thống
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("DangNhap", "TaiKhoan");
            int maUser = int.Parse(userIdStr);

            // Tìm giá sân để tính tổng tiền tự động
            var sanBong = _context.Set<SanBong>().Find(maSan);
            if (sanBong == null) return RedirectToAction("TrangTimSan");

            TimeOnly bd = TimeOnly.Parse(gioBatDau);
            TimeOnly kt = TimeOnly.Parse(gioKetThuc);
            double soGioDa = (double)(kt.ToTimeSpan() - bd.ToTimeSpan()).TotalHours;
            decimal tongTien = sanBong.GiaTheoGio * (decimal)soGioDa;

            // Khởi tạo đơn đặt sân mới
            DatLich donDat = new DatLich
            {
                MaNguoiDung = maUser,
                MaSan = maSan,
                NgayDa = DateOnly.Parse(ngayDa),
                GioBatDau = bd,
                GioKetThuc = kt,
                TongTien = tongTien,
                TrangThai = "Đang chờ", // Chờ Admin xác nhận
                NgayDat = DateTime.Now
            };

            _context.Set<DatLich>().Add(donDat);
            _context.SaveChanges();

            // Đặt thành công thì chuyển hướng về trang lịch sử để xem đơn
            return RedirectToAction("LichSuDatSan");
        }

        // ================= 3. TRANG LỊCH SỬ ĐẶT SÂN CỦA KHÁCH =================
        public IActionResult LichSuDatSan()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int maUser = int.Parse(userIdStr);

            // Chỉ lọc ra các đơn đặt lịch của RIÊNG khách hàng này
            var lichSu = _context.Set<DatLich>()
                .Include(d => d.MaSanNavigation) // Để lấy tên sân hiển thị
                .Where(d => d.MaNguoiDung == maUser)
                .OrderByDescending(d => d.NgayDat)
                .ToList();

            return View(lichSu);
        }
    }
}