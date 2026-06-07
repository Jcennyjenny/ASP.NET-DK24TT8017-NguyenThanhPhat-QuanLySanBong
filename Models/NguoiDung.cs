using System;
using System.Collections.Generic;

namespace QuanLySanBong.Models;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string? Email { get; set; }

    public string? VaiTro { get; set; }

    public virtual ICollection<DatLich> DatLiches { get; set; } = new List<DatLich>();
}
