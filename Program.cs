using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// 1. Cấu hình kết nối SQL Server LocalDB
builder.Services.AddDbContext<QuanLySanBongContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=QuanLySanBong;Trusted_Connection=True;TrustServerCertificate=True;"));

// 2. Cấu hình Cookie Đăng nhập (Giúp phân quyền Admin và Khách hàng)
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/TaiKhoan/DangNhap"; // Trang tự chuyển hướng nếu chưa đăng nhập
        options.AccessDeniedPath = "/TaiKhoan/TuChoiTruyCap"; // Trang báo lỗi nếu vào sai quyền
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
