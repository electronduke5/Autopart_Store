using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace University.Database
{
    public partial class UniversityContext : DbContext
    {
        public UniversityContext()
        {
        }

        public UniversityContext(DbContextOptions<UniversityContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accounting> Accountings { get; set; }
        public virtual DbSet<Autopart> Autoparts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CombinationOrder> CombinationOrders { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DefectAutopart> DefectAutoparts { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<Role> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=University;Username=postgres;Password=admin");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Russian_Russia.1251");

            modelBuilder.Entity<Accounting>(entity =>
            {
                entity.HasKey(e => e.IdAccounting)
                    .HasName("pk_accounting");

                entity.ToTable("accounting");

                entity.Property(e => e.IdAccounting).HasColumnName("id_accounting");

                entity.Property(e => e.Expenditure)
                    .HasPrecision(38, 2)
                    .HasColumnName("expenditure")
                    .HasDefaultValueSql("0.0");

                entity.Property(e => e.Profit)
                    .HasPrecision(38, 2)
                    .HasColumnName("profit")
                    .HasDefaultValueSql("0.0");

                entity.Property(e => e.RecordDate)
                    .HasColumnName("record_date")
                    .HasDefaultValueSql("(now())::date");
            });

            modelBuilder.Entity<Autopart>(entity =>
            {
                entity.HasKey(e => e.IdAutopart)
                    .HasName("pk_autopart");

                entity.ToTable("autopart");

                entity.Property(e => e.IdAutopart).HasColumnName("id_autopart");

                entity.Property(e => e.AutopartName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("autopart_name");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.Price)
                    .HasPrecision(38, 2)
                    .HasColumnName("price");

                entity.Property(e => e.ProviderId).HasColumnName("provider_id");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Autoparts)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("autopart_category_id_fkey");

                entity.HasOne(d => d.Provider)
                    .WithMany(p => p.Autoparts)
                    .HasForeignKey(d => d.ProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("autopart_provider_id_fkey");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.IdCategory)
                    .HasName("pk_category");

                entity.ToTable("category");

                entity.HasIndex(e => e.CategoryName, "uq_category_name")
                    .IsUnique();

                entity.Property(e => e.IdCategory).HasColumnName("id_category");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("category_name");
            });

            modelBuilder.Entity<CombinationOrder>(entity =>
            {
                entity.HasKey(e => e.IdCombination)
                    .HasName("pk_combination");

                entity.ToTable("combination_order");

                entity.Property(e => e.IdCombination).HasColumnName("id_combination");

                entity.Property(e => e.AutopartId).HasColumnName("autopart_id");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.OrdersId).HasColumnName("orders_id");

                entity.HasOne(d => d.Autopart)
                    .WithMany(p => p.CombinationOrders)
                    .HasForeignKey(d => d.AutopartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("combination_order_autopart_id_fkey");

                entity.HasOne(d => d.Orders)
                    .WithMany(p => p.CombinationOrders)
                    .HasForeignKey(d => d.OrdersId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("combination_order_orders_id_fkey");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.IdCustomer)
                    .HasName("pk_customer");

                entity.ToTable("customer");

                entity.Property(e => e.IdCustomer).HasColumnName("id_customer");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("password");

                entity.Property(e => e.Patronymic)
                    .HasMaxLength(30)
                    .HasColumnName("patronymic")
                    .HasDefaultValueSql("'-'::character varying");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(19)
                    .HasColumnName("phone_number");

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("surname");
            });

            modelBuilder.Entity<DefectAutopart>(entity =>
            {
                entity.HasKey(e => e.IdDefect)
                    .HasName("pk_defect");

                entity.ToTable("defect_autopart");

                entity.Property(e => e.IdDefect).HasColumnName("id_defect");

                entity.Property(e => e.AutopartId).HasColumnName("autopart_id");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.HasOne(d => d.Autopart)
                    .WithMany(p => p.DefectAutoparts)
                    .HasForeignKey(d => d.AutopartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("defect_autopart_autopart_id_fkey");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.IdEmployee)
                    .HasName("pk_user");

                entity.ToTable("employee");

                entity.Property(e => e.IdEmployee).HasColumnName("id_employee");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("password");

                entity.Property(e => e.Patronymic)
                    .HasMaxLength(30)
                    .HasColumnName("patronymic")
                    .HasDefaultValueSql("'-'::character varying");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(19)
                    .HasColumnName("phone_number");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("surname");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("employee_role_id_fkey");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.IdOrders)
                    .HasName("pk_orders");

                entity.ToTable("orders");

                entity.Property(e => e.IdOrders).HasColumnName("id_orders");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.OrdersDate)
                    .HasColumnType("date")
                    .HasColumnName("orders_date")
                    .HasDefaultValueSql("CURRENT_DATE");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("orders_customer_id_fkey");
            });

            modelBuilder.Entity<Provider>(entity =>
            {
                entity.HasKey(e => e.IdProvider)
                    .HasName("pk_provider");

                entity.ToTable("provider");

                entity.HasIndex(e => e.ProviderName, "uq_provider")
                    .IsUnique();

                entity.Property(e => e.IdProvider).HasColumnName("id_provider");

                entity.Property(e => e.ProviderName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("provider_name");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.IdRole)
                    .HasName("pk_role");

                entity.ToTable("role");

                entity.HasIndex(e => e.RoleName, "uq_role_name")
                    .IsUnique();

                entity.Property(e => e.IdRole).HasColumnName("id_role");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("role_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
