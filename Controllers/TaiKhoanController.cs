using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using QuanLySanBong.Models;

public class TaiKhoanController : Controller
{
    private readonly QuanLySanBongContext _context;

    public TaiKhoanController(QuanLySanBongContext context)
    {
        _context = context;
    }

    // ================= ĐĂNG KÝ =================
    [HttpGet]
    public IActionResult DangKy() => View();

    [HttpPost]
    public IActionResult DangKy(NguoiDung model)
    {
        // Kiểm tra xem tên đăng nhập đã tồn tại chưa
        var checkExit = _context.NguoiDung.Any(u => u.TenDangNhap == model.TenDangNhap);
        if (checkExit)
        {
            ViewBag.Loi = "Tên đăng nhập này đã có người sử dụng!";
            return View(model);
        }

        // Mặc định tài khoản đăng ký mới luôn là Khách hàng
        model.VaiTro = "KhachHang";

        _context.NguoiDung.Add(model);
        _context.SaveChanges();

        return RedirectToAction("DangNhap");
    }

    // ================= ĐĂNG NHẬP =================
    [HttpGet]
    public IActionResult DangNhap() => View();

    [HttpPost]
    public async Task<IActionResult> DangNhap(string tenDangNhap, string matKhau)
    {
        // Tìm tài khoản trong database khớp user và pass (Mẹo: Ở bài trước ta chèn sẵn pass là '123')
        var user = _context.NguoiDung.FirstOrDefault(u => u.TenDangNhap == tenDangNhap && u.MatKhau == matKhau);

        if (user != null)
        {
            // Thiết lập "Chứng minh thư điện tử" (Claims) lưu thông tin user vào Cookie trình duyệt
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.MaNguoiDung.ToString()),
                // Sửa lại dòng Claim cũ thành như sau:
                new Claim(ClaimTypes.Name, user.HoTen ?? ""),
                new Claim(ClaimTypes.Role, user.VaiTro ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Điều hướng dựa trên vai trò sau khi đăng nhập thành công
            if (user.VaiTro == "Admin")
            {
                return RedirectToAction("TinhTrangSanHienTai", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Loi = "Sai tên đăng nhập hoặc mật khẩu!";
        return View();
    }

    // ================= ĐĂNG XUẤT =================
    public async Task<IActionResult> DangXuat()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("DangNhap");
    }

    public IActionResult TuChoiTruyCap() => Content("Bạn không có quyền truy cập vào chức năng này!");
}