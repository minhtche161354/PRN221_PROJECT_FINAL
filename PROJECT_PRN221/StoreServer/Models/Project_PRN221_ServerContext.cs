using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace StoreServer.Models
{
    public partial class Project_PRN221_ServerContext : DbContext
    {
        public Project_PRN221_ServerContext()
        {
        }

        public Project_PRN221_ServerContext(DbContextOptions<Project_PRN221_ServerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bill> Bills { get; set; } = null!;
        public virtual DbSet<BillDetail> BillDetails { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Discount> Discounts { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=(local);database=Project_PRN221_Server;uid=sa;pwd=123;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bill>(entity =>
            {
                entity.Property(e => e.BillId).HasColumnName("BillID");

                entity.Property(e => e.BillDate).HasColumnType("date");

                entity.Property(e => e.DiscountId).HasColumnName("DiscountID");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");

                entity.Property(e => e.TotalWorth).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.DiscountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Bills__DiscountI__30F848ED");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK__Bills__EmployeeI__2F10007B");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Bills__PaymentMe__300424B4");
            });

            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.Property(e => e.BillDetailId).HasColumnName("BillDetailID");

                entity.Property(e => e.BillId).HasColumnName("BillID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Bill)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.BillId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__BillDetai__BillI__33D4B598");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__BillDetai__Produ__34C8D9D1");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.Property(e => e.DiscountId).HasColumnName("DiscountID");

                entity.Property(e => e.DiscountName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.Account)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");

                entity.Property(e => e.PaymentMethodName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.ExpirationDate).HasColumnType("date");

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ProductImg)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProductName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.WarehouseArrivalDate).HasColumnType("date");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Products__Catego__2C3393D0");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
