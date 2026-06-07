using System;
using System.Collections.Generic;

namespace QuanLySanBong.Models;

public partial class SanBong
{
    public int MaSan { get; set; }

    public string TenSan { get; set; } = null!;

    public int LoaiSan { get; set; }

    public decimal GiaTheoGio { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<DatLich> DatLiches { get; set; } = new List<DatLich>();
}
