using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Data.Entity;
using Data.Entity.Base;

namespace DataAccessLayer
{
    public interface IApplicationDbContext
    {
        // Bạn có thể khai báo các phương thức cần thiết cho context tại đây
    }

    public class WaseEaseDbContext : DbContext, IApplicationDbContext
    {
        public WaseEaseDbContext()
        {
        }

        public WaseEaseDbContext(DbContextOptions<WaseEaseDbContext> options)
            : base(options)
        {
        }

        #region DbSet Properties

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountGroup> AccountGroups { get; set; }
        public virtual DbSet<AccountPermission> AccountPermissions { get; set; }
        public virtual DbSet<AccountWarehouse> AccountWarehouses { get; set; }
        public virtual DbSet<AppAction> AppActions { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Cell> Cells { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Floor> Floors { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupPermission> GroupPermissions { get; set; }
        public virtual DbSet<IssueNote> IssueNotes { get; set; }
        public virtual DbSet<IssueDetail> IssueDetails { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionAction> PermissionActions { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<ProductTypeTypeDetail> ProductTypeTypeDetails { get; set; }
        public virtual DbSet<PurchaseReceipt> PurchaseReceipts { get; set; }
        public virtual DbSet<ReceivingDetail> ReceivingDetails { get; set; }
        public virtual DbSet<ReceivingNote> ReceivingNotes { get; set; }
        public virtual DbSet<SaleReceipt> SaleReceipts { get; set; }
        public virtual DbSet<Shelf> Shelves { get; set; }
        public virtual DbSet<StockCard> StockCards { get; set; }
        public virtual DbSet<StockCardDetail> StockCardDetails { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<TypeDetail> TypeDetails { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Nếu chưa cấu hình từ bên ngoài (DI) thì sử dụng cấu hình trong appsettings.json
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                optionsBuilder.UseSqlServer(config.GetConnectionString("Default"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Thiết lập hành vi xóa cho tất cả các FK (theo phiên bản cũ là NoAction)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var foreignKey in entityType.GetForeignKeys())
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
                }
            }

            // 2. Các entity join trong phiên bản cũ sử dụng composite key.
            //    Tuy nhiên, trong các entity mới, do kế thừa từ BaseEntity (có thuộc tính Id) nên đã có khóa chính riêng.
            //    Nếu bạn vẫn muốn đảm bảo không có dữ liệu trùng lặp, hãy thiết lập index duy nhất cho tổ hợp các FK.

            modelBuilder.Entity<AccountGroup>()
                .HasIndex(ag => new { ag.AccountId, ag.GroupId })
                .IsUnique();

            modelBuilder.Entity<GroupPermission>()
                .HasIndex(gp => new { gp.GroupId, gp.PermissionId })
                .IsUnique();

            modelBuilder.Entity<PermissionAction>()
                .HasIndex(pa => new { pa.PermissionId, pa.ActionId })
                .IsUnique();

            modelBuilder.Entity<AccountWarehouse>()
                .HasIndex(aw => new { aw.AccountId, aw.WarehouseId })
                .IsUnique();

            modelBuilder.Entity<StockCardDetail>()
                .HasIndex(scd => new { scd.StockCardId, scd.ProductTypeId })
                .IsUnique();

            // 3. Cấu hình thuộc tính cho một số entity (các cấu hình này được áp dụng trong phiên bản cũ)
            modelBuilder.Entity<Account>()
                .Property(a => a.UserName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Customer>()
                .Property(c => c.Status)
                .HasDefaultValue(true);

            modelBuilder.Entity<TypeDetail>()
                .Property(td => td.Name)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<TypeDetail>()
                .Property(td => td.Description)
                .IsRequired()
                .HasMaxLength(500);

            // 4. Cấu hình quan hệ (chú ý: một số cấu hình đã được định nghĩa qua DataAnnotations nên có thể không cần thiết lặp lại)
            // --- Account & các quan hệ ---
            modelBuilder.Entity<Account>()
                .HasMany(a => a.AccountGroups)
                .WithOne(ag => ag.Account)
                .HasForeignKey(ag => ag.AccountId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Notifications)
                .WithOne(n => n.Account)
                .HasForeignKey(n => n.AccountId);

            // --- Quan hệ của AccountGroup ---
            modelBuilder.Entity<AccountGroup>()
                .HasOne(ag => ag.Account)
                .WithMany(a => a.AccountGroups)
                .HasForeignKey(ag => ag.AccountId);

            modelBuilder.Entity<AccountGroup>()
                .HasOne(ag => ag.Group)
                .WithMany(g => g.AccountGroups)
                .HasForeignKey(ag => ag.GroupId);

            // --- Quan hệ của GroupPermission ---
            modelBuilder.Entity<GroupPermission>()
                .HasOne(gp => gp.Group)
                .WithMany(g => g.GroupPermissions)
                .HasForeignKey(gp => gp.GroupId);

            modelBuilder.Entity<GroupPermission>()
                .HasOne(gp => gp.Permission)
                .WithMany(p => p.GroupPermissions)
                .HasForeignKey(gp => gp.PermissionId);

            // --- Quan hệ của PermissionAction ---
            modelBuilder.Entity<PermissionAction>()
                .HasOne(pa => pa.Permission)
                .WithMany(p => p.PermissionActions)
                .HasForeignKey(pa => pa.PermissionId);

            modelBuilder.Entity<PermissionAction>()
                .HasOne(pa => pa.Action)
                .WithMany(a => a.PermissionActions)
                .HasForeignKey(pa => pa.ActionId);

            // --- Quan hệ của AccountWarehouse ---
            modelBuilder.Entity<AccountWarehouse>()
                .HasOne(aw => aw.Account)
                .WithMany(a => a.AccountWarehouses)
                .HasForeignKey(aw => aw.AccountId);

            modelBuilder.Entity<AccountWarehouse>()
                .HasOne(aw => aw.Warehouse)
                .WithMany(w => w.AccountWarehouses)
                .HasForeignKey(aw => aw.WarehouseId);

            // --- Quan hệ của StockCardDetail ---
            modelBuilder.Entity<StockCardDetail>()
                .HasOne(scd => scd.StockCard)
                .WithMany(sc => sc.StockCardDetails)
                .HasForeignKey(scd => scd.StockCardId);

            modelBuilder.Entity<StockCardDetail>()
                .HasOne(scd => scd.ProductType)
                .WithMany(pt => pt.StockCardDetails)
                .HasForeignKey(scd => scd.ProductTypeId);

            // --- Cấu hình cho Product & ProductType ---
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductTypes)
                .WithOne(pt => pt.Product)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Cell)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CellId);

            modelBuilder.Entity<ProductType>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTypes)
                .HasForeignKey(pt => pt.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductType>()
                .HasMany(pt => pt.PurchaseDetails)
                .WithOne(pd => pd.ProductType)
                .HasForeignKey(pd => pd.ProductTypeId);

            modelBuilder.Entity<ProductType>()
                .HasMany(pt => pt.ReceivingDetails)
                .WithOne(rd => rd.ProductType)
                .HasForeignKey(rd => rd.ProductTypeId);

            modelBuilder.Entity<ProductType>()
                .HasMany(pt => pt.SaleDetails)
                .WithOne(sd => sd.ProductType)
                .HasForeignKey(sd => sd.ProductTypeId);

            modelBuilder.Entity<ProductType>()
                .HasMany(pt => pt.StockCardDetails)
                .WithOne(scd => scd.ProductType)
                .HasForeignKey(scd => scd.ProductTypeId);

            modelBuilder.Entity<ProductType>()
                .HasMany(pt => pt.IssueDetails)
                .WithOne(id => id.ProductType)
                .HasForeignKey(id => id.ProductTypeId);

            // --- Cấu hình cho Shelf & Floor ---
            modelBuilder.Entity<Shelf>()
                .HasMany(s => s.Floors)
                .WithOne(f => f.Shelf)
                .HasForeignKey(f => f.ShelfId);

            // --- Cấu hình cho Warehouse & Shelf ---
            modelBuilder.Entity<Warehouse>()
                .HasMany(w => w.Shelves)
                .WithOne(s => s.Warehouse)
                .HasForeignKey(s => s.WarehouseId);

            // --- Cấu hình ReceivingNote ---
            modelBuilder.Entity<ReceivingNote>()
                .HasMany(rn => rn.ReceivingDetails)
                .WithOne(rd => rd.receivingNote)
                .HasForeignKey(rd => rd.NoteId);

            // --- Cấu hình cho Supplier ---
            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.ReceivingNotes)
                .WithOne(rn => rn.Supplier)
                .HasForeignKey(rn => rn.SupplierId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
