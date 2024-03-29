﻿using System;
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

    public virtual DbSet<TblComboServiceCalendar> TblComboServiceCalendars { get; set; }

    public virtual DbSet<TblDistrict> TblDistricts { get; set; }

    public virtual DbSet<TblEmailOtpcode> TblEmailOtpcodes { get; set; }

    public virtual DbSet<TblFeedBack> TblFeedBacks { get; set; }

    public virtual DbSet<TblImage> TblImages { get; set; }

    public virtual DbSet<TblPayment> TblPayments { get; set; }

    public virtual DbSet<TblProduct> TblProducts { get; set; }

    public virtual DbSet<TblProductItem> TblProductItems { get; set; }

    public virtual DbSet<TblProductItemDetail> TblProductItemDetails { get; set; }

    public virtual DbSet<TblRentOrder> TblRentOrders { get; set; }

    public virtual DbSet<TblRentOrderDetail> TblRentOrderDetails { get; set; }

    public virtual DbSet<TblRentOrderGroup> TblRentOrderGroups { get; set; }

    public virtual DbSet<TblReward> TblRewards { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblSaleOrder> TblSaleOrders { get; set; }

    public virtual DbSet<TblSaleOrderDetail> TblSaleOrderDetails { get; set; }

    public virtual DbSet<TblService> TblServices { get; set; }

    public virtual DbSet<TblServiceCalendar> TblServiceCalendars { get; set; }

    public virtual DbSet<TblServiceDetail> TblServiceDetails { get; set; }

    public virtual DbSet<TblServiceOrder> TblServiceOrders { get; set; }

    public virtual DbSet<TblShippingFee> TblShippingFees { get; set; }

    public virtual DbSet<TblSize> TblSizes { get; set; }

    public virtual DbSet<TblTakecareCombo> TblTakecareCombos { get; set; }

    public virtual DbSet<TblTakecareComboOrder> TblTakecareComboOrders { get; set; }

    public virtual DbSet<TblTakecareComboService> TblTakecareComboServices { get; set; }

    public virtual DbSet<TblTakecareComboServiceDetail> TblTakecareComboServiceDetails { get; set; }

    public virtual DbSet<TblTransaction> TblTransactions { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblUserTree> TblUserTrees { get; set; }

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

        modelBuilder.Entity<TblComboServiceCalendar>(entity =>
        {
            entity.ToTable("tblComboServiceCalendar");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.NextServiceDate).HasColumnType("datetime");
            entity.Property(e => e.ServiceDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Sumary).HasMaxLength(500);
            entity.Property(e => e.TakecareComboOrderId).HasColumnName("TakecareComboOrderID");

            entity.HasOne(d => d.TakecareComboOrder).WithMany(p => p.TblComboServiceCalendars)
                .HasForeignKey(d => d.TakecareComboOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblComboServiceCalendar_tblTakecareComboOrder");
        });

        modelBuilder.Entity<TblDistrict>(entity =>
        {
            entity.ToTable("tblDistrict");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DistrictName).HasMaxLength(200);
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
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductItemDetailId).HasColumnName("ProductItemDetailID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ProductItemDetail).WithMany(p => p.TblFeedBacks)
                .HasForeignKey(d => d.ProductItemDetailId)
                .HasConstraintName("FK_tblFeedBack_tblProductItemDetail");

            entity.HasOne(d => d.User).WithMany(p => p.TblFeedBacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblFeedBacks_tblUsers");
        });

        modelBuilder.Entity<TblImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblImages");

            entity.ToTable("tblImage");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ComboServiceCalendarId).HasColumnName("ComboServiceCalendarID");
            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(2048)
                .HasColumnName("ImageURL");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductItemId).HasColumnName("ProductItemID");
            entity.Property(e => e.RentOrderDetailId).HasColumnName("RentOrderDetailID");
            entity.Property(e => e.SaleOrderDetailId).HasColumnName("SaleOrderDetailID");
            entity.Property(e => e.ServiceCalendarId).HasColumnName("ServiceCalendarID");
            entity.Property(e => e.ServiceDetailId).HasColumnName("ServiceDetailID");
            entity.Property(e => e.UserTreeId).HasColumnName("UserTreeID");

            entity.HasOne(d => d.Category).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_tblImages_tblCategories");

            entity.HasOne(d => d.ComboServiceCalendar).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ComboServiceCalendarId)
                .HasConstraintName("FK_tblImage_tblComboServiceCalendar");

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

            entity.HasOne(d => d.RentOrderDetail).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.RentOrderDetailId)
                .HasConstraintName("FK_tblImage_tblRentOrderDetail");

            entity.HasOne(d => d.SaleOrderDetail).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.SaleOrderDetailId)
                .HasConstraintName("FK_tblImage_tblSaleOrderDetail");

            entity.HasOne(d => d.ServiceCalendar).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ServiceCalendarId)
                .HasConstraintName("FK_tblImage_tblServiceCalendar");

            entity.HasOne(d => d.ServiceDetail).WithMany(p => p.TblImages)
                .HasForeignKey(d => d.ServiceDetailId)
                .HasConstraintName("FK_tblImage_tblServiceDetail");
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
            entity.Property(e => e.CareGuide).HasMaxLength(1000);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Rule).HasMaxLength(500);
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
            entity.Property(e => e.CareGuideUrl)
                .HasMaxLength(2048)
                .HasColumnName("CareGuideURL");
            entity.Property(e => e.ContractUrl)
                .HasMaxLength(2048)
                .HasColumnName("ContractURL");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.OrderCode).HasMaxLength(20);
            entity.Property(e => e.RecipientAddress).HasMaxLength(500);
            entity.Property(e => e.RecipientName).HasMaxLength(200);
            entity.Property(e => e.RecipientPhone).HasMaxLength(50);
            entity.Property(e => e.RentOrderGroupId).HasColumnName("RentOrderGroupID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblRentOrderCreatedByNavigations).HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.RecipientDistrictNavigation).WithMany(p => p.TblRentOrders)
                .HasForeignKey(d => d.RecipientDistrict)
                .HasConstraintName("FK_tblRentOrder_tblDistrict");

            entity.HasOne(d => d.RentOrderGroup).WithMany(p => p.TblRentOrders)
                .HasForeignKey(d => d.RentOrderGroupId)
                .HasConstraintName("FK_tblRentOrder_tblRentOrderGroup");

            entity.HasOne(d => d.User).WithMany(p => p.TblRentOrderUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tblRentOrder_tblUser");
        });

        modelBuilder.Entity<TblRentOrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblAddendumProductItems");

            entity.ToTable("tblRentOrderDetail");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.FeedbackStatus).HasDefaultValueSql("((0))");
            entity.Property(e => e.ProductItemDetailId).HasColumnName("ProductItemDetailID");
            entity.Property(e => e.ProductItemName).HasMaxLength(200);
            entity.Property(e => e.RentOrderId).HasColumnName("RentOrderID");
            entity.Property(e => e.SizeName).HasMaxLength(200);

            entity.HasOne(d => d.ProductItemDetail).WithMany(p => p.TblRentOrderDetails)
                .HasForeignKey(d => d.ProductItemDetailId)
                .HasConstraintName("FK_tblRentOrderDetail_tblProductItemDetail");

            entity.HasOne(d => d.RentOrder).WithMany(p => p.TblRentOrderDetails)
                .HasForeignKey(d => d.RentOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblRentOrderDetail_tblRentOrder");
        });

        modelBuilder.Entity<TblRentOrderGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_RentOrderGroup");

            entity.ToTable("tblRentOrderGroup");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblReward>(entity =>
        {
            entity.ToTable("tblReward");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
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
            entity.Property(e => e.CareGuideUrl)
                .HasMaxLength(2048)
                .HasColumnName("CareGuideURL");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.OrderCode).HasMaxLength(20);
            entity.Property(e => e.RecipientAddress).HasMaxLength(500);
            entity.Property(e => e.RecipientName).HasMaxLength(200);
            entity.Property(e => e.RecipientPhone).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.RecipientDistrictNavigation).WithMany(p => p.TblSaleOrders)
                .HasForeignKey(d => d.RecipientDistrict)
                .HasConstraintName("FK_tblSaleOrder_tblDistrict");

            entity.HasOne(d => d.User).WithMany(p => p.TblSaleOrders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSaleOrder_tblUser");
        });

        modelBuilder.Entity<TblSaleOrderDetail>(entity =>
        {
            entity.ToTable("tblSaleOrderDetail");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.FeedbackStatus).HasDefaultValueSql("((0))");
            entity.Property(e => e.ProductItemDetailId).HasColumnName("ProductItemDetailID");
            entity.Property(e => e.ProductItemName).HasMaxLength(200);
            entity.Property(e => e.SaleOderId).HasColumnName("SaleOderID");
            entity.Property(e => e.SizeName).HasMaxLength(200);

            entity.HasOne(d => d.SaleOder).WithMany(p => p.TblSaleOrderDetails)
                .HasForeignKey(d => d.SaleOderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSaleOrderDetail_tblSaleOrder");
        });

        modelBuilder.Entity<TblService>(entity =>
        {
            entity.ToTable("tblService");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DistrictId).HasColumnName("DistrictID");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Rules).HasMaxLength(1000);
            entity.Property(e => e.ServiceCode).HasMaxLength(20);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TechnicianId).HasColumnName("TechnicianID");
            entity.Property(e => e.TechnicianName).HasMaxLength(200);

            entity.HasOne(d => d.District).WithMany(p => p.TblServices)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK_tblService_tblDistrict");

            entity.HasOne(d => d.User).WithMany(p => p.TblServices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblService_tblUser");
        });

        modelBuilder.Entity<TblServiceCalendar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblCalendar");

            entity.ToTable("tblServiceCalendar");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.NextServiceDate).HasColumnType("datetime");
            entity.Property(e => e.ServiceDate).HasColumnType("datetime");
            entity.Property(e => e.ServiceOrderId).HasColumnName("ServiceOrderID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Sumary).HasMaxLength(500);

            entity.HasOne(d => d.ServiceOrder).WithMany(p => p.TblServiceCalendars)
                .HasForeignKey(d => d.ServiceOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCalendar_tblServiceOrder");
        });

        modelBuilder.Entity<TblServiceDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblServiceUserTree");

            entity.ToTable("tblServiceDetail");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CareGuide).HasMaxLength(500);
            entity.Property(e => e.Desciption).HasMaxLength(500);
            entity.Property(e => e.ManagerDescription).HasMaxLength(500);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.ServicePrice).HasDefaultValueSql("((0))");
            entity.Property(e => e.TreeName).HasMaxLength(200);
            entity.Property(e => e.UserTreeId).HasColumnName("UserTreeID");

            entity.HasOne(d => d.Service).WithMany(p => p.TblServiceDetails)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_tblServiceUserTree_tblService");

            entity.HasOne(d => d.UserTree).WithMany(p => p.TblServiceDetails)
                .HasForeignKey(d => d.UserTreeId)
                .HasConstraintName("FK_tblServiceUserTree_tblUserTree");
        });

        modelBuilder.Entity<TblServiceOrder>(entity =>
        {
            entity.ToTable("tblServiceOrder");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CareGuideUrl)
                .HasMaxLength(2048)
                .HasColumnName("CareGuideURL");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.OrderCode).HasMaxLength(20);
            entity.Property(e => e.ServiceEndDate).HasColumnType("datetime");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.ServiceStartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TechnicianId).HasColumnName("TechnicianID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Service).WithMany(p => p.TblServiceOrders)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblServiceOrder_tblService");
        });

        modelBuilder.Entity<TblShippingFee>(entity =>
        {
            entity.ToTable("tblShippingFee");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.DistrictId).HasColumnName("DistrictID");

            entity.HasOne(d => d.District).WithMany(p => p.TblShippingFees)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblShippingFee_tblDistrict");
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

        modelBuilder.Entity<TblTakecareCombo>(entity =>
        {
            entity.ToTable("tblTakecareCombo");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Careguide).HasMaxLength(1000);
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.Guarantee).HasMaxLength(2000);
            entity.Property(e => e.Name).HasMaxLength(500);
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<TblTakecareComboOrder>(entity =>
        {
            entity.ToTable("tblTakecareComboOrder");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CareGuideUrl).HasMaxLength(1000);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.OrderCode).HasMaxLength(50);
            entity.Property(e => e.ServiceEndDate).HasColumnType("datetime");
            entity.Property(e => e.ServiceStartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TakecareComboServiceId).HasColumnName("TakecareComboServiceID");
            entity.Property(e => e.TechnicianId).HasColumnName("TechnicianID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.CancelByNavigation).WithMany(p => p.TblTakecareComboOrderCancelByNavigations)
                .HasForeignKey(d => d.CancelBy)
                .HasConstraintName("FK_tblTakecareComboOrder_tblUserCancel");

            entity.HasOne(d => d.TakecareComboService).WithMany(p => p.TblTakecareComboOrders)
                .HasForeignKey(d => d.TakecareComboServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTakecareComboOrder_tblTakecareComboService");

            entity.HasOne(d => d.Technician).WithMany(p => p.TblTakecareComboOrderTechnicians)
                .HasForeignKey(d => d.TechnicianId)
                .HasConstraintName("FK_tblTakecareComboOrder_tblUserTech");

            entity.HasOne(d => d.User).WithMany(p => p.TblTakecareComboOrderUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tblTakecareComboOrder_tblUser");
        });

        modelBuilder.Entity<TblTakecareComboService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TakeceComboService");

            entity.ToTable("tblTakecareComboService");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.CancelReason).HasMaxLength(500);
            entity.Property(e => e.CareGuide).HasMaxLength(500);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IsAtShop).HasDefaultValueSql("((1))");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(200);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TakecareComboId).HasColumnName("TakecareComboID");
            entity.Property(e => e.TechnicianId).HasColumnName("TechnicianID");
            entity.Property(e => e.TechnicianName).HasMaxLength(200);

            entity.HasOne(d => d.CancelByNavigation).WithMany(p => p.TblTakecareComboServiceCancelByNavigations)
                .HasForeignKey(d => d.CancelBy)
                .HasConstraintName("FK_tblTakecareComboService_tblUserCancel");

            entity.HasOne(d => d.TakecareCombo).WithMany(p => p.TblTakecareComboServices)
                .HasForeignKey(d => d.TakecareComboId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TakeceComboService_tblTakecareCombo");

            entity.HasOne(d => d.Technician).WithMany(p => p.TblTakecareComboServiceTechnicians)
                .HasForeignKey(d => d.TechnicianId)
                .HasConstraintName("FK_TakeceComboService_tblUserTech");

            entity.HasOne(d => d.User).WithMany(p => p.TblTakecareComboServiceUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TakeceComboService_tblUser");
        });

        modelBuilder.Entity<TblTakecareComboServiceDetail>(entity =>
        {
            entity.ToTable("tblTakecareComboServiceDetail");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.TakecareComboCareguide).HasMaxLength(1000);
            entity.Property(e => e.TakecareComboDescription).HasMaxLength(4000);
            entity.Property(e => e.TakecareComboGuarantee).HasMaxLength(2000);
            entity.Property(e => e.TakecareComboId).HasColumnName("TakecareComboID");
            entity.Property(e => e.TakecareComboName).HasMaxLength(500);
            entity.Property(e => e.TakecareComboServiceId).HasColumnName("TakecareComboServiceID");

            entity.HasOne(d => d.TakecareCombo).WithMany(p => p.TblTakecareComboServiceDetails)
                .HasForeignKey(d => d.TakecareComboId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTakecareComboServiceDetail_tblTakecareCombo");

            entity.HasOne(d => d.TakecareComboService).WithMany(p => p.TblTakecareComboServiceDetails)
                .HasForeignKey(d => d.TakecareComboServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTakecareComboServiceDetail_tblTakecareComboService");
        });

        modelBuilder.Entity<TblTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblPayments");

            entity.ToTable("tblTransaction");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.DatetimePaid).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.RentOrderId).HasColumnName("RentOrderID");
            entity.Property(e => e.SaleOrderId).HasColumnName("SaleOrderID");
            entity.Property(e => e.ServiceOrderId).HasColumnName("ServiceOrderID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TakecareComboOrderId).HasColumnName("TakecareComboOrderID");
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

            entity.HasOne(d => d.TakecareComboOrder).WithMany(p => p.TblTransactions)
                .HasForeignKey(d => d.TakecareComboOrderId)
                .HasConstraintName("FK_tblTransaction_tblTakecareComboOrder");
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
            entity.Property(e => e.DistrictId).HasColumnName("DistrictID");
            entity.Property(e => e.Favorite).HasMaxLength(500);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Mail).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.RoleId)
                .HasDefaultValueSql("('c98b8768-5827-4e5d-bf3c-3ba67b913d70')")
                .HasColumnName("RoleID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("('disabled')");
            entity.Property(e => e.UserName).HasMaxLength(200);

            entity.HasOne(d => d.District).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK_tblUser_tblDistrict");

            entity.HasOne(d => d.Role).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblUsers_tblRoles");
        });

        modelBuilder.Entity<TblUserTree>(entity =>
        {
            entity.ToTable("tblUserTree");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TreeName).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
