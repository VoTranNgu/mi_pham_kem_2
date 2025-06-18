using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace mi_pham_kem.Models.SQLServer;

public partial class MiPhamContext : DbContext
{
    public MiPhamContext()
    {
    }

    public MiPhamContext(DbContextOptions<MiPhamContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<DanhGium> DanhGia { get; set; }

    public virtual DbSet<DanhMucSp> DanhMucSps { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<ThuongHieu> ThuongHieus { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UuDai> UuDais { get; set; }

    public virtual DbSet<UuDaiKhachHang> UuDaiKhachHangs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-ILCD9RQ;Initial Catalog=MiPham;User ID=abc;Password=123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.MaChiTietDh).HasName("PK__ChiTietD__651E6E6A95F390CC");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.MaChiTietDh).HasColumnName("MaChiTietDH");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaDh).HasColumnName("MaDH");

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDon__MaDH__534D60F1");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaSan__52593CB8");
        });

        modelBuilder.Entity<DanhGium>(entity =>
        {
            entity.HasKey(e => e.MaDg).HasName("PK__DanhGia__27258660F3A8E48B");

            entity.Property(e => e.MaDg).HasColumnName("MaDG");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__MaKH__4BAC3F29");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__MaSanPh__4AB81AF0");
        });

        modelBuilder.Entity<DanhMucSp>(entity =>
        {
            entity.HasKey(e => e.MaDm).HasName("PK__DanhMucS__2725866ECFC819B7");

            entity.ToTable("DanhMucSP");

            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDh).HasName("PK__DonHang__27258661E5A8C7E6");

            entity.ToTable("DonHang");

            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaUd).HasColumnName("MaUD");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__MaHD__4F7CD00D");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__MaKH__4E88ABD4");

            entity.HasOne(d => d.MaUdNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaUd)
                .HasConstraintName("FK_DonHang_UuDai");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__GioHang__F5001DA3DEB7CE44");

            entity.ToTable("GioHang");

            entity.Property(e => e.MaKh).HasColumnName("MaKH");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaKH__440B1D61");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaSanPh__44FF419A");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HoaDon__2725A6E0E508A28E");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaKH__47DBAE45");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KhachHan__2725CF1E24A608DD");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .HasColumnName("SDT");

            entity.HasOne(d => d.MaUserNavigation).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.MaUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__MaUse__412EB0B6");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SanPham__FAC7442DCCECE240");

            entity.ToTable("SanPham");

            entity.Property(e => e.Gia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Hinh2).HasMaxLength(255);
            entity.Property(e => e.Hinh3).HasMaxLength(255);
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.MaTh).HasColumnName("MaTH");
            entity.Property(e => e.TenSanPham).HasMaxLength(100);

            entity.HasOne(d => d.MaDmNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDm)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaDM__3C69FB99");

            entity.HasOne(d => d.MaThNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaTh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaTH__3B75D760");
        });

        modelBuilder.Entity<ThuongHieu>(entity =>
        {
            entity.HasKey(e => e.MaTh).HasName("PK__ThuongHi__272500755FCF77B7");

            entity.ToTable("ThuongHieu");

            entity.Property(e => e.MaTh).HasColumnName("MaTH");
            entity.Property(e => e.TenThuongHieu).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.MaUser).HasName("PK__User__55DAC4B780CAAC2E");

            entity.ToTable("User");

            entity.Property(e => e.MatKhau).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
        });

        modelBuilder.Entity<UuDai>(entity =>
        {
            entity.HasKey(e => e.MaUd).HasName("PK__UuDai__272518CFC01B49EA");

            entity.ToTable("UuDai");

            entity.Property(e => e.MaUd).HasColumnName("MaUD");
            entity.Property(e => e.GiaGiam).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaToiThieu).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NoiDungMa).HasMaxLength(100);
        });

        modelBuilder.Entity<UuDaiKhachHang>(entity =>
        {
            entity.HasKey(e => new { e.MaKh, e.MaUd, e.MaDh }).HasName("PK__UuDai_Kh__A570BB14025F5BAC");

            entity.ToTable("UuDai_KhachHang");

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaUd).HasColumnName("MaUD");
            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.DaSuDung).HasDefaultValue(false);

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.UuDaiKhachHangs)
                .HasForeignKey(d => d.MaDh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UuDai_Khac__MaDH__5AEE82B9");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.UuDaiKhachHangs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UuDai_Khac__MaKH__59063A47");

            entity.HasOne(d => d.MaUdNavigation).WithMany(p => p.UuDaiKhachHangs)
                .HasForeignKey(d => d.MaUd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UuDai_Khac__MaUD__59FA5E80");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
