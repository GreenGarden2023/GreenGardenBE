using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Entities;

public partial class GreenGardenDbContext : DbContext
{
    public GreenGardenDbContext()
    {
    }

    public GreenGardenDbContext(DbContextOptions<GreenGardenDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAddendum> TblAddendums { get; set; }

    public virtual DbSet<TblAddendumProductItem> TblAddendumProductItems { get; set; }

    public virtual DbSet<TblCategory> TblCategories { get; set; }

    public virtual DbSet<TblFeedBack> TblFeedBacks { get; set; }

    public virtual DbSet<TblImage> TblImages { get; set; }

    public virtual DbSet<TblOrder> TblOrders { get; set; }

    public virtual DbSet<TblPayment> TblPayments { get; set; }

    public virtual DbSet<TblProduct> TblProducts { get; set; }

    public virtual DbSet<TblProductItem> TblProductItems { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblTransaction> TblTransactions { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblVoucher> TblVouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAddendum>(entity =>
        {
            entity.ToTable("tblAddendums");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");

            entity.HasOne(d => d.Order).WithMany(p => p.TblAddenda)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAddendums_tblOrders");
        });

        modelBuilder.Entity<TblAddendumProductItem>(entity =>
        {
            entity.ToTable("tblAddendumProductItems");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.AddendumId).HasColumnName("AddendumID");
            entity.Property(e => e.ProductItemId).HasColumnName("ProductItemID");

            entity.HasOne(d => d.Addendum).WithMany(p => p.TblAddendumProductItems)
                .HasForeignKey(d => d.AddendumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAddendumProductItems_tblAddendums");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.TblAddendumProductItems)
                .HasForeignKey(d => d.ProductItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAddendumProductItems_tblProductItems");
        });

        modelBuilder.Entity<TblCategory>(entity =>
        {
            entity.ToTable("tblCategories");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TblFeedBack>(entity =>
        {
            entity.ToTable("tblFeedBacks");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.ProductItemId).HasColumnName("ProductItemID");
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

        modelBuilder.Entity<TblImage>(entity =>
        {
            entity.ToTable("tblImages");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductItemId).HasColumnName("ProductItemID");

            entity.HasOne(d => d.Category).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_tblImages_tblCategories");

            entity.HasOne(d => d.Feedback).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK_tblImages_tblFeedBacks");

            entity.HasOne(d => d.Product).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_tblImages_tblProducts");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ProductItemId)
                .HasConstraintName("FK_tblImages_tblProductItems");
        });

        modelBuilder.Entity<TblOrder>(entity =>
        {
            entity.ToTable("tblOrders");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VoucherId).HasColumnName("VoucherID");

            entity.HasOne(d => d.User).WithMany(p => p.TblOrders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblOrders_tblUsers");

            entity.HasOne(d => d.Voucher).WithMany(p => p.TblOrders)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK_tblOrders_tblVouchers");
        });

        modelBuilder.Entity<TblPayment>(entity =>
        {
            entity.ToTable("tblPayments");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

            entity.HasOne(d => d.Order).WithMany(p => p.TblPayments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_tblPayments_tblOrders");

            entity.HasOne(d => d.Transaction).WithMany(p => p.TblPayments)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK_tblPayments_tblTransactions");
        });

        modelBuilder.Entity<TblProduct>(entity =>
        {
            entity.ToTable("tblProducts");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

            entity.HasOne(d => d.Category).WithMany(p => p.TblProducts)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_tblProducts_tblCategories");
        });

        modelBuilder.Entity<TblProductItem>(entity =>
        {
            entity.ToTable("tblProductItems");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");

            entity.HasOne(d => d.Product).WithMany(p => p.TblProductItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_tblProductItems_tblProducts");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.ToTable("tblRoles");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TblTransaction>(entity =>
        {
            entity.ToTable("tblTransactions");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.AddendumId).HasColumnName("AddendumID");

            entity.HasOne(d => d.Addendum).WithMany(p => p.TblTransactions)
                .HasForeignKey(d => d.AddendumId)
                .HasConstraintName("FK_tblTransactions_tblAddendums");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.ToTable("tblUsers");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.RoleId)
                .HasDefaultValueSql("('c98b8768-5827-4e5d-bf3c-3ba67b913d70')")
                .HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblUsers_tblRoles");
        });

        modelBuilder.Entity<TblVoucher>(entity =>
        {
            entity.ToTable("tblVouchers");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
