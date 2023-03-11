using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Entities;

public partial class GreenGardenDbContext : DbContext
{
    public GreenGardenDbContext(DbContextOptions<GreenGardenDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCart> TblCarts { get; set; }

    public virtual DbSet<TblCartDetail> TblCartDetails { get; set; }

    public virtual DbSet<TblCategory> TblCategories { get; set; }

    public virtual DbSet<TblEmailOtpcode> TblEmailOtpcodes { get; set; }

    public virtual DbSet<TblFeedBack> TblFeedBacks { get; set; }

    public virtual DbSet<TblFile> TblFiles { get; set; }

    public virtual DbSet<TblImage> TblImages { get; set; }

    public virtual DbSet<TblPayment> TblPayments { get; set; }

    public virtual DbSet<TblProduct> TblProducts { get; set; }

    public virtual DbSet<TblProductItem> TblProductItems { get; set; }

    public virtual DbSet<TblProductItemDetail> TblProductItemDetails { get; set; }

    public virtual DbSet<TblRentOrder> TblRentOrders { get; set; }

    public virtual DbSet<TblRentOrderDetail> TblRentOrderDetails { get; set; }

    public virtual DbSet<TblReport> TblReports { get; set; }

    public virtual DbSet<TblRequest> TblRequests { get; set; }

    public virtual DbSet<TblRequestDetail> TblRequestDetails { get; set; }

    public virtual DbSet<TblReward> TblRewards { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblSaleOrder> TblSaleOrders { get; set; }

    public virtual DbSet<TblSaleOrderDetail> TblSaleOrderDetails { get; set; }

    public virtual DbSet<TblServiceOrder> TblServiceOrders { get; set; }

    public virtual DbSet<TblSize> TblSizes { get; set; }

    public virtual DbSet<TblTransaction> TblTransactions { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblCart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblCarts");

            entity.ToTable("tblCart");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.TblCarts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tblCarts_tblUsers");
        });

        modelBuilder.Entity<TblCartDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblCartDetails");

            entity.ToTable("tblCartDetail");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.SizeProductItemId).HasColumnName("SizeProductItemID");

            entity.HasOne(d => d.Cart).WithMany(p => p.TblCartDetails)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCartDetails_tblCarts");

            entity.HasOne(d => d.SizeProductItem).WithMany(p => p.TblCartDetails)
                .HasForeignKey(d => d.SizeProductItemId)
                .HasConstraintName("FK_tblCartDetails_tblSIzeProductItems");
        });

        modelBuilder.Entity<TblCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblCategories");

            entity.ToTable("tblCategory");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<TblEmailOtpcode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_EmailOTPCode");

            entity.ToTable("tblEmailOTPCode");

            entity.HasIndex(e => e.Optcode, "Index_EmailOTPCode_1").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Optcode)
                .HasMaxLength(50)
                .HasColumnName("OPTCode");

            entity.HasOne(d => d.EmailNavigation).WithMany(p => p.TblEmailOtpcodes)
                .HasPrincipalKey(p => p.Mail)
                .HasForeignKey(d => d.Email)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailOTPCode_tblUsers");
        });

        modelBuilder.Entity<TblFeedBack>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblFeedBacks");

            entity.ToTable("tblFeedBack");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.ProductItemId).HasColumnName("ProductItemID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.TblFeedBacks)
                .HasForeignKey(d => d.ProductItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblFeedBacks_tblProductItems");

            entity.HasOne(d => d.User).WithMany(p => p.TblFeedBacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblFeedBacks_tblUsers");
        });

        modelBuilder.Entity<TblFile>(entity =>
        {
            entity.ToTable("tblFile");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.FileUrl).HasMaxLength(2048);
            entity.Property(e => e.ReportId).HasColumnName("ReportID");

            entity.HasOne(d => d.Report).WithMany(p => p.TblFiles)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblFile_tblReport");
        });

        modelBuilder.Entity<TblImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblImages");

            entity.ToTable("tblImage");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(2048)
                .HasColumnName("ImageURL");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductItemId).HasColumnName("ProductItemID");
            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.RequestId).HasColumnName("RequestID");

            entity.HasOne(d => d.Category).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_tblImages_tblCategories");

            entity.HasOne(d => d.Feedback).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK_tblImages_tblFeedBacks");

            entity.HasOne(d => d.Product).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_tblImages_tblProducts");

            entity.HasOne(d => d.ProductItemDetail).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ProductItemDetailId)
                .HasConstraintName("FK_tblImages_tblSizeProductItems");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ProductItemId)
                .HasConstraintName("FK_tblImages_tblProductItems");

            entity.HasOne(d => d.Report).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("FK_tblImages_tblReport");

            entity.HasOne(d => d.ReportNavigation).WithMany(p => p.TblImageReportNavigations)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("FK_tblImage_tblRequest");

            entity.HasOne(d => d.Request).WithMany(p => p.TblImageRequests)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_tblImages_tblRequest");
        });

        modelBuilder.Entity<TblPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblTransactions");

            entity.ToTable("tblPayment");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.PaymentMethod).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<TblProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblProducts");

            entity.ToTable("tblProduct");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.TblProducts)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblProducts_tblCategories");
        });

        modelBuilder.Entity<TblProductItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblProductItems");

            entity.ToTable("tblProductItem");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Product).WithMany(p => p.TblProductItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblProductItems_tblProducts");
        });

        modelBuilder.Entity<TblProductItemDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblSIzeProductItems");

            entity.ToTable("tblProductItemDetail");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.ProductItemId).HasColumnName("ProductItemID");
            entity.Property(e => e.SizeId).HasColumnName("SizeID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.ProductItem).WithMany(p => p.TblProductItemDetails)
                .HasForeignKey(d => d.ProductItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSizeProductItems_tblProductItems");

            entity.HasOne(d => d.Size).WithMany(p => p.TblProductItemDetails)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSizeProductItems_tblSizes");
        });

        modelBuilder.Entity<TblRentOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblAddendums");

            entity.ToTable("tblRentOrder");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.ReferenceOrderId).HasColumnName("ReferenceOrderID");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<TblRentOrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblAddendumProductItems");

            entity.ToTable("tblRentOrderDetail");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.ProductItemDetailId).HasColumnName("ProductItemDetailID");
            entity.Property(e => e.RentOrderId).HasColumnName("RentOrderID");

            entity.HasOne(d => d.ProductItemDetail).WithMany(p => p.TblRentOrderDetails)
                .HasForeignKey(d => d.ProductItemDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblRentOrderDetail_tblProductItemDetail");

            entity.HasOne(d => d.RentOrder).WithMany(p => p.TblRentOrderDetails)
                .HasForeignKey(d => d.RentOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblRentOrderDetail_tblRentOrder");
        });

        modelBuilder.Entity<TblReport>(entity =>
        {
            entity.ToTable("tblReport");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.Sumary).HasMaxLength(500);

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.TblReports)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblReport_tblUser");
        });

        modelBuilder.Entity<TblRequest>(entity =>
        {
            entity.ToTable("tblRequest");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.TblRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblRequests_tblUsers");
        });

        modelBuilder.Entity<TblRequestDetail>(entity =>
        {
            entity.ToTable("tblRequestDetail");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.ServiceOrderId).HasColumnName("ServiceOrderID");
            entity.Property(e => e.TreeName).HasMaxLength(50);

            entity.HasOne(d => d.Request).WithMany(p => p.TblRequestDetails)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblRequestDetail_tblRequest");

            entity.HasOne(d => d.ServiceOrder).WithMany(p => p.TblRequestDetails)
                .HasForeignKey(d => d.ServiceOrderId)
                .HasConstraintName("FK_tblRequestDetail_tblServiceOrder");
        });

        modelBuilder.Entity<TblReward>(entity =>
        {
            entity.ToTable("tblReward");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.TblRewards)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tblRewards_tblUsers");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblRoles");

            entity.ToTable("tblRole");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.RoleName).HasMaxLength(200);
        });

        modelBuilder.Entity<TblSaleOrder>(entity =>
        {
            entity.ToTable("tblSaleOrder");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblSaleOrderDetail>(entity =>
        {
            entity.ToTable("tblSaleOrderDetail");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.ProductItemDetailId).HasColumnName("ProductItemDetailID");
            entity.Property(e => e.SaleOderId).HasColumnName("SaleOderID");

            entity.HasOne(d => d.ProductItemDetail).WithMany(p => p.TblSaleOrderDetails)
                .HasForeignKey(d => d.ProductItemDetailId)
                .HasConstraintName("FK_tblSaleOrderDetail_tblProductItemDetail");

            entity.HasOne(d => d.SaleOder).WithMany(p => p.TblSaleOrderDetails)
                .HasForeignKey(d => d.SaleOderId)
                .HasConstraintName("FK_tblSaleOrderDetail_tblSaleOrder");
        });

        modelBuilder.Entity<TblServiceOrder>(entity =>
        {
            entity.ToTable("tblServiceOrder");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.ServiceEndDate).HasColumnType("datetime");
            entity.Property(e => e.ServiceStartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TechnicianId).HasColumnName("TechnicianID");

            entity.HasOne(d => d.Request).WithMany(p => p.TblServiceOrders)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblServiceOrder_tblRequest");
        });

        modelBuilder.Entity<TblSize>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblSizes");

            entity.ToTable("tblSize");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<TblTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblPayments");

            entity.ToTable("tblTransaction");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.DatetimePaid).HasColumnType("datetime");
            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.RentOrderId).HasColumnName("RentOrderID");
            entity.Property(e => e.SaleOrderId).HasColumnName("SaleOrderID");
            entity.Property(e => e.ServiceOrderId).HasColumnName("ServiceOrderID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Payment).WithMany(p => p.TblTransactions)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTransactions_tblPayments");

            entity.HasOne(d => d.RentOrder).WithMany(p => p.TblTransactions)
                .HasForeignKey(d => d.RentOrderId)
                .HasConstraintName("FK_tblTransaction_tblRentOrder");

            entity.HasOne(d => d.SaleOrder).WithMany(p => p.TblTransactions)
                .HasForeignKey(d => d.SaleOrderId)
                .HasConstraintName("FK_tblTransaction_tblSaleOrder");

            entity.HasOne(d => d.ServiceOrder).WithMany(p => p.TblTransactions)
                .HasForeignKey(d => d.ServiceOrderId)
                .HasConstraintName("FK_tblTransaction_tblServiceOrder");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblUsers");

            entity.ToTable("tblUser");

            entity.HasIndex(e => e.UserName, "Index_tblUsers_1").IsUnique();

            entity.HasIndex(e => e.Mail, "Index_tblUsers_2").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Favorite).HasMaxLength(500);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Mail).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.RoleId)
                .HasDefaultValueSql("('c98b8768-5827-4e5d-bf3c-3ba67b913d70')")
                .HasColumnName("RoleID");
            entity.Property(e => e.UserName).HasMaxLength(200);

            entity.HasOne(d => d.Role).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblUsers_tblRoles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
