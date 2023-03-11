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
        _ = modelBuilder.Entity<TblCart>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblCarts");

            _ = entity.ToTable("tblCart");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Status).HasMaxLength(50);
            _ = entity.Property(e => e.UserId).HasColumnName("UserID");

            _ = entity.HasOne(d => d.User).WithMany(p => p.TblCarts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tblCarts_tblUsers");
        });

        _ = modelBuilder.Entity<TblCartDetail>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblCartDetails");

            _ = entity.ToTable("tblCartDetail");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.CartId).HasColumnName("CartID");
            _ = entity.Property(e => e.SizeProductItemId).HasColumnName("SizeProductItemID");

            _ = entity.HasOne(d => d.Cart).WithMany(p => p.TblCartDetails)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCartDetails_tblCarts");

            _ = entity.HasOne(d => d.SizeProductItem).WithMany(p => p.TblCartDetails)
                .HasForeignKey(d => d.SizeProductItemId)
                .HasConstraintName("FK_tblCartDetails_tblSIzeProductItems");
        });

        _ = modelBuilder.Entity<TblCategory>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblCategories");

            _ = entity.ToTable("tblCategory");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Description).HasMaxLength(500);
            _ = entity.Property(e => e.Name).HasMaxLength(200);
            _ = entity.Property(e => e.Status).HasMaxLength(50);
        });

        _ = modelBuilder.Entity<TblEmailOtpcode>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_EmailOTPCode");

            _ = entity.ToTable("tblEmailOTPCode");

            _ = entity.HasIndex(e => e.Optcode, "Index_EmailOTPCode_1").IsUnique();

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Email).HasMaxLength(200);
            _ = entity.Property(e => e.Optcode)
                .HasMaxLength(50)
                .HasColumnName("OPTCode");

            _ = entity.HasOne(d => d.EmailNavigation).WithMany(p => p.TblEmailOtpcodes)
                .HasPrincipalKey(p => p.Mail)
                .HasForeignKey(d => d.Email)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailOTPCode_tblUsers");
        });

        _ = modelBuilder.Entity<TblFeedBack>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblFeedBacks");

            _ = entity.ToTable("tblFeedBack");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Comment).HasMaxLength(500);
            _ = entity.Property(e => e.ProductItemId).HasColumnName("ProductItemID");
            _ = entity.Property(e => e.Status).HasMaxLength(50);
            _ = entity.Property(e => e.UserId).HasColumnName("UserID");

            _ = entity.HasOne(d => d.ProductItem).WithMany(p => p.TblFeedBacks)
                .HasForeignKey(d => d.ProductItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblFeedBacks_tblProductItems");

            _ = entity.HasOne(d => d.User).WithMany(p => p.TblFeedBacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblFeedBacks_tblUsers");
        });

        _ = modelBuilder.Entity<TblFile>(entity =>
        {
            _ = entity.ToTable("tblFile");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.FileUrl).HasMaxLength(2048);
            _ = entity.Property(e => e.ReportId).HasColumnName("ReportID");

            _ = entity.HasOne(d => d.Report).WithMany(p => p.TblFiles)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblFile_tblReport");
        });

        _ = modelBuilder.Entity<TblImage>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblImages");

            _ = entity.ToTable("tblImage");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            _ = entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            _ = entity.Property(e => e.ImageUrl)
                .HasMaxLength(2048)
                .HasColumnName("ImageURL");
            _ = entity.Property(e => e.ProductId).HasColumnName("ProductID");
            _ = entity.Property(e => e.ProductItemId).HasColumnName("ProductItemID");
            _ = entity.Property(e => e.ReportId).HasColumnName("ReportID");
            _ = entity.Property(e => e.RequestId).HasColumnName("RequestID");

            _ = entity.HasOne(d => d.Category).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_tblImages_tblCategories");

            _ = entity.HasOne(d => d.Feedback).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK_tblImages_tblFeedBacks");

            _ = entity.HasOne(d => d.Product).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_tblImages_tblProducts");

            _ = entity.HasOne(d => d.ProductItemDetail).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ProductItemDetailId)
                .HasConstraintName("FK_tblImages_tblSizeProductItems");

            _ = entity.HasOne(d => d.ProductItem).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ProductItemId)
                .HasConstraintName("FK_tblImages_tblProductItems");

            _ = entity.HasOne(d => d.Report).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("FK_tblImages_tblReport");

            _ = entity.HasOne(d => d.ReportNavigation).WithMany(p => p.TblImageReportNavigations)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("FK_tblImage_tblRequest");

            _ = entity.HasOne(d => d.Request).WithMany(p => p.TblImageRequests)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_tblImages_tblRequest");
        });

        _ = modelBuilder.Entity<TblPayment>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblTransactions");

            _ = entity.ToTable("tblPayment");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.PaymentMethod).HasMaxLength(200);
            _ = entity.Property(e => e.Status).HasMaxLength(50);
        });

        _ = modelBuilder.Entity<TblProduct>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblProducts");

            _ = entity.ToTable("tblProduct");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            _ = entity.Property(e => e.Description).HasMaxLength(500);
            _ = entity.Property(e => e.Name).HasMaxLength(200);
            _ = entity.Property(e => e.Status).HasMaxLength(50);

            _ = entity.HasOne(d => d.Category).WithMany(p => p.TblProducts)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblProducts_tblCategories");
        });

        _ = modelBuilder.Entity<TblProductItem>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblProductItems");

            _ = entity.ToTable("tblProductItem");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Description).HasMaxLength(500);
            _ = entity.Property(e => e.Name).HasMaxLength(200);
            _ = entity.Property(e => e.Type).HasMaxLength(50);

            _ = entity.HasOne(d => d.Product).WithMany(p => p.TblProductItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblProductItems_tblProducts");
        });

        _ = modelBuilder.Entity<TblProductItemDetail>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblSIzeProductItems");

            _ = entity.ToTable("tblProductItemDetail");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.ProductItemId).HasColumnName("ProductItemID");
            _ = entity.Property(e => e.SizeId).HasColumnName("SizeID");
            _ = entity.Property(e => e.Status).HasMaxLength(50);

            _ = entity.HasOne(d => d.ProductItem).WithMany(p => p.TblProductItemDetails)
                .HasForeignKey(d => d.ProductItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSizeProductItems_tblProductItems");

            _ = entity.HasOne(d => d.Size).WithMany(p => p.TblProductItemDetails)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSizeProductItems_tblSizes");
        });

        _ = modelBuilder.Entity<TblRentOrder>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblAddendums");

            _ = entity.ToTable("tblRentOrder");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.ReferenceOrderId).HasColumnName("ReferenceOrderID");
            _ = entity.Property(e => e.Status).HasMaxLength(50);
        });

        _ = modelBuilder.Entity<TblRentOrderDetail>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblAddendumProductItems");

            _ = entity.ToTable("tblRentOrderDetail");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.ProductItemId).HasColumnName("ProductItemID");
            _ = entity.Property(e => e.RentOrderId).HasColumnName("RentOrderID");
        });

        _ = modelBuilder.Entity<TblReport>(entity =>
        {
            _ = entity.ToTable("tblReport");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.CreateDate).HasColumnType("datetime");
            _ = entity.Property(e => e.Name).HasMaxLength(250);
            _ = entity.Property(e => e.Sumary).HasMaxLength(500);

            _ = entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.TblReports)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblReport_tblUser");
        });

        _ = modelBuilder.Entity<TblRequest>(entity =>
        {
            _ = entity.ToTable("tblRequest");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Address).HasMaxLength(200);
            _ = entity.Property(e => e.CreateDate).HasColumnType("datetime");
            _ = entity.Property(e => e.Descripton).HasMaxLength(200);
            _ = entity.Property(e => e.Phone).HasMaxLength(50);
            _ = entity.Property(e => e.TreeName).HasMaxLength(50);
            _ = entity.Property(e => e.UserId).HasColumnName("UserID");

            _ = entity.HasOne(d => d.User).WithMany(p => p.TblRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblRequests_tblUsers");
        });

        _ = modelBuilder.Entity<TblReward>(entity =>
        {
            _ = entity.ToTable("tblReward");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Status).HasMaxLength(50);
            _ = entity.Property(e => e.UserId).HasColumnName("UserID");

            _ = entity.HasOne(d => d.User).WithMany(p => p.TblRewards)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tblRewards_tblUsers");
        });

        _ = modelBuilder.Entity<TblRole>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblRoles");

            _ = entity.ToTable("tblRole");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Description).HasMaxLength(200);
            _ = entity.Property(e => e.RoleName).HasMaxLength(200);
        });

        _ = modelBuilder.Entity<TblSaleOrder>(entity =>
        {
            _ = entity.ToTable("tblSaleOrder");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.CreateDate).HasColumnType("datetime");
            _ = entity.Property(e => e.Status).HasMaxLength(50);
            _ = entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        _ = modelBuilder.Entity<TblSaleOrderDetail>(entity =>
        {
            _ = entity.ToTable("tblSaleOrderDetail");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.ProductItemDetailId).HasColumnName("ProductItemDetailID");
            _ = entity.Property(e => e.SaleOderId).HasColumnName("SaleOderID");
        });

        _ = modelBuilder.Entity<TblServiceOrder>(entity =>
        {
            _ = entity.ToTable("tblServiceOrder");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.CreateDate).HasColumnType("datetime");
            _ = entity.Property(e => e.RequestId).HasColumnName("RequestID");
            _ = entity.Property(e => e.ServiceEndDate).HasColumnType("datetime");
            _ = entity.Property(e => e.ServiceStartDate).HasColumnType("datetime");
            _ = entity.Property(e => e.Status).HasMaxLength(50);
            _ = entity.Property(e => e.UserId).HasColumnName("UserID");

            _ = entity.HasOne(d => d.Request).WithMany(p => p.TblServiceOrders)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblServiceOrder_tblRequest");
        });

        _ = modelBuilder.Entity<TblSize>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblSizes");

            _ = entity.ToTable("tblSize");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Name).HasMaxLength(200);
        });

        _ = modelBuilder.Entity<TblTransaction>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblPayments");

            _ = entity.ToTable("tblTransaction");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.DatetimePaid).HasColumnType("datetime");
            _ = entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            _ = entity.Property(e => e.RentOrderId).HasColumnName("RentOrderID");
            _ = entity.Property(e => e.SaleOrderId).HasColumnName("SaleOrderID");
            _ = entity.Property(e => e.ServiceOrderId).HasColumnName("ServiceOrderID");
            _ = entity.Property(e => e.Status).HasMaxLength(50);
            _ = entity.Property(e => e.Type).HasMaxLength(50);

            _ = entity.HasOne(d => d.Payment).WithMany(p => p.TblTransactions)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTransactions_tblPayments");

            _ = entity.HasOne(d => d.RentOrder).WithMany(p => p.TblTransactions)
                .HasForeignKey(d => d.RentOrderId)
                .HasConstraintName("FK_tblTransaction_tblRentOrder");

            _ = entity.HasOne(d => d.SaleOrder).WithMany(p => p.TblTransactions)
                .HasForeignKey(d => d.SaleOrderId)
                .HasConstraintName("FK_tblTransaction_tblSaleOrder");

            _ = entity.HasOne(d => d.ServiceOrder).WithMany(p => p.TblTransactions)
                .HasForeignKey(d => d.ServiceOrderId)
                .HasConstraintName("FK_tblTransaction_tblServiceOrder");
        });

        _ = modelBuilder.Entity<TblUser>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_tblUsers");

            _ = entity.ToTable("tblUser");

            _ = entity.HasIndex(e => e.UserName, "Index_tblUsers_1").IsUnique();

            _ = entity.HasIndex(e => e.Mail, "Index_tblUsers_2").IsUnique();

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Address).HasMaxLength(500);
            _ = entity.Property(e => e.Favorite).HasMaxLength(500);
            _ = entity.Property(e => e.FullName).HasMaxLength(200);
            _ = entity.Property(e => e.Mail).HasMaxLength(200);
            _ = entity.Property(e => e.Phone).HasMaxLength(50);
            _ = entity.Property(e => e.RoleId)
                .HasDefaultValueSql("('c98b8768-5827-4e5d-bf3c-3ba67b913d70')")
                .HasColumnName("RoleID");
            _ = entity.Property(e => e.UserName).HasMaxLength(200);

            _ = entity.HasOne(d => d.Role).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblUsers_tblRoles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
