using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLySanBong.Models;

public partial class QuanLySanBongContext : DbContext
{
    public QuanLySanBongContext()
    {
    }

    public QuanLySanBongContext(DbContextOptions<QuanLySanBongContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DatLich> DatLiche { get; set; }

    public virtual DbSet<NguoiDung> NguoiDung { get; set; }

    public virtual DbSet<SanBong> SanBong { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=QuanLySanBong;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DatLich>(entity =>
        {
            entity.HasKey(e => e.MaDatLich).HasName("PK__DatLich__35B3DED8E7F03735");

            entity.ToTable("DatLich");

            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Đang chờ");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DatLiches)
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK__DatLich__MaNguoi__6383C8BA");

            entity.HasOne(d => d.MaSanNavigation).WithMany(p => p.DatLiches)
                .HasForeignKey(d => d.MaSan)
                .HasConstraintName("FK__DatLich__MaSan__6477ECF3");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D7624CAC7E4D");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.TenDangNhap, "UQ__NguoiDun__55F68FC0CA690AFA").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .HasDefaultValue("KhachHang");
        });

        modelBuilder.Entity<SanBong>(entity =>
        {
            entity.HasKey(e => e.MaSan).HasName("PK__SanBong__3188C826D9CDCAF5");

            entity.ToTable("SanBong");

            entity.Property(e => e.GiaTheoGio).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TenSan).HasMaxLength(100);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Sẵn sàng");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
