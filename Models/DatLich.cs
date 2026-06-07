using System;
using System.Collections.Generic;

namespace QuanLySanBong.Models;

public partial class DatLich
{
    public int MaDatLich { get; set; }

    public int? MaNguoiDung { get; set; }

    public int? MaSan { get; set; }

    public DateOnly NgayDa { get; set; }

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }

    public decimal? TongTien { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayDat { get; set; }

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }

    public virtual SanBong? MaSanNavigation { get; set; }
}
