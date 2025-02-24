using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity;
using Data.Entity.Base;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer
{
    public interface IApplicationDbContext
    {

    }

    public class WaseEaseDbContext : DbContext, IApplicationDbContext
    {
        public WaseEaseDbContext()
        {
        }

        public WaseEaseDbContext(DbContextOptions<WaseEaseDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountGroup> AccountGroups { get; set; }
        public virtual DbSet<AccountPermission> AccountPermissions { get; set; }
        public virtual DbSet<AccountWarehouse> AccountWarehouses { get; set; }
        public virtual DbSet<AppAction> Actions { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Cell> Cells { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Floor> Floors { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupPermission> GroupPermissions { get; set; }
        public virtual DbSet<IssueNote> IssueNotes { get; set; }
        public virtual DbSet<IssueDetail> NoteDetails { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionAction> PermissionActions { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<ProductTypeTypeDetail> ProductTypeTypeDetails { get; set; }
        public virtual DbSet<PurchaseReceipt> PurchaseReceipts { get; set; }
        public virtual DbSet<ReceivingDetail> ReceiptDetails { get; set; }
        public virtual DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public virtual DbSet<ReceivingNote> ReceivingNotes { get; set; }
        public virtual DbSet<SaleReceipt> SaleReceipts { get; set; }
        public virtual DbSet<Shelf> Shelves { get; set; }
        public virtual DbSet<StockCard> StockCards { get; set; }
        public virtual DbSet<StockCardDetail> StockCardDetails { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<TypeDetail> TypeDetails { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                Env.Load();

                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DB_URL"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set OnDelete behavior to NoAction for all foreign keys
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var foreignKey in entityType.GetForeignKeys())
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
                }
            }

            modelBuilder.Entity<Profile>()
                .HasOne(p => p.Account)
                .WithOne(a => a.Profile)
                .HasForeignKey<Profile>(p => p.AccountId)
                .OnDelete(DeleteBehavior.NoAction);

            // AccountGroup composite key
            modelBuilder.Entity<AccountGroup>()
                .HasKey(ag => new { ag.AccountId, ag.GroupId });

            modelBuilder.Entity<AccountGroup>()
                .HasOne(ag => ag.Account)
                .WithMany(a => a.AccountGroups)
                .HasForeignKey(ag => ag.AccountId);

            modelBuilder.Entity<AccountGroup>()
                .HasOne(ag => ag.Group)
                .WithMany(g => g.AccountGroups)
                .HasForeignKey(ag => ag.GroupId);

            // GroupPermission composite key
            modelBuilder.Entity<GroupPermission>()
                .HasKey(gp => new { gp.GroupId, gp.PermissionId });

            modelBuilder.Entity<GroupPermission>()
                .HasOne(gp => gp.Group)
                .WithMany(g => g.GroupPermissions)
                .HasForeignKey(gp => gp.GroupId);

            modelBuilder.Entity<GroupPermission>()
                .HasOne(gp => gp.Permission)
                .WithMany(p => p.GroupPermissions)
                .HasForeignKey(gp => gp.PermissionId);

            // PermissionAction composite key
            modelBuilder.Entity<PermissionAction>()
                .HasKey(pa => new { pa.PermissionId, pa.ActionId });

            modelBuilder.Entity<PermissionAction>()
                .HasOne(pa => pa.Permission)
                .WithMany(p => p.PermissionActions)
                .HasForeignKey(pa => pa.PermissionId);

            modelBuilder.Entity<PermissionAction>()
                .HasOne(pa => pa.Action)
                .WithMany(a => a.PermissionActions)
                .HasForeignKey(pa => pa.ActionId);

            // AccountWarehouse composite key
            modelBuilder.Entity<AccountWarehouse>()
                .HasKey(aw => new { aw.AccountId, aw.WarehouseId });

            modelBuilder.Entity<AccountWarehouse>()
                .HasOne(aw => aw.Account)
                .WithMany(a => a.AccountWarehouses)
                .HasForeignKey(aw => aw.AccountId);

            modelBuilder.Entity<AccountWarehouse>()
                .HasOne(aw => aw.Warehouse)
                .WithMany(w => w.AccountWarehouses)
                .HasForeignKey(aw => aw.WarehouseId);

            // StockCardDetail composite key
            modelBuilder.Entity<StockCardDetail>()
                .HasKey(scd => new { scd.StockCardId, scd.ProductTypeId });

            modelBuilder.Entity<StockCardDetail>()
                .HasOne(scd => scd.StockCard)
                .WithMany(sc => sc.StockCardDetails)
                .HasForeignKey(scd => scd.StockCardId);

            modelBuilder.Entity<StockCardDetail>()
                .HasOne(scd => scd.ProductType)
                .WithMany(pt => pt.StockCardDetails)
                .HasForeignKey(scd => scd.ProductTypeId);

            // Account configurations
            modelBuilder.Entity<Account>()
                .HasMany(a => a.AccountGroups)
                .WithOne(ag => ag.Account)
                .HasForeignKey(ag => ag.AccountId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Notifications)
                .WithOne(n => n.Account)
                .HasForeignKey(n => n.AccountId);

            modelBuilder.Entity<Account>()
                .Property(a => a.UserName)
                .IsRequired()
                .HasMaxLength(100);

            // Category configurations
            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Customer configurations
            modelBuilder.Entity<Customer>()
                .Property(c => c.Status)
                .HasDefaultValue(true);

            // Product configurations
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductTypes)
                .WithOne(pt => pt.Product)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);




            // ProductType configuration
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

            // Shelf and Floor configurations
            modelBuilder.Entity<Shelf>()
                .HasMany(s => s.Floors)
                .WithOne(f => f.Shelf)
                .HasForeignKey(f => f.ShelfId);

            // Warehouse and Shelf configurations
            modelBuilder.Entity<Warehouse>()
                .HasMany(w => w.Shelves)
                .WithOne(s => s.Warehouse)
                .HasForeignKey(s => s.WarehouseId);

            // ReceivingNote configurations
            modelBuilder.Entity<ReceivingNote>()
                .HasMany(rn => rn.ReceivingDetails)
                .WithOne(nd => nd.receivingNote)
                .HasForeignKey(nd => nd.NoteId);

            // Supplier configurations
            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.ReceivingNotes)
                .WithOne(rn => rn.Supplier)
                .HasForeignKey(rn => rn.SupplierId);

            // TypeDetail configurations
            modelBuilder.Entity<TypeDetail>()
                .Property(td => td.Name)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<TypeDetail>()
                .Property(td => td.Description)
                .IsRequired()
                .HasMaxLength(500);

            base.OnModelCreating(modelBuilder);
        }

    }
}
