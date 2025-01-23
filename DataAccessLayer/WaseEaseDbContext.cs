using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity;
using Data.Entity.Base;
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
        public virtual DbSet<NoteDetail> NoteDetails { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionAction> PermissionActions { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<ProductTypeTypeDetail> ProductTypeTypeDetails { get; set; }
        public virtual DbSet<PurchaseReceipt> PurchaseReceipts { get; set; }
        public virtual DbSet<ReceiptDetail> ReceiptDetails { get; set; }
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
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();

                optionsBuilder.UseSqlServer(config.GetConnectionString("Default"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Iterate through all entity types
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Find all foreign keys in the entity type
                var foreignKeys = entityType.GetForeignKeys();

                // Set OnDelete behavior to NoAction for each foreign key
                foreach (var foreignKey in foreignKeys)
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
                }
            }

            // Account relationships
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

            // Permission and PermissionAction relationships
            modelBuilder.Entity<PermissionAction>()
                .HasOne(pa => pa.Permission)
                .WithMany(p => p.PermissionActions)
                .HasForeignKey(pa => pa.PermissionId);

            modelBuilder.Entity<PermissionAction>()
                .HasOne(pa => pa.Action)
                .WithMany(a => a.PermissionActions)
                .HasForeignKey(pa => pa.ActionId);

            // Category configurations
            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Customer configurations
            modelBuilder.Entity<Customer>()
                .Property(c => c.Status)
                .HasDefaultValue(true);

            // Product relationships
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

            // ProductType relationships
            modelBuilder.Entity<ProductType>()
                .HasMany(pt => pt.NoteDetails)
                .WithOne(nd => nd.ProductType)
                .HasForeignKey(nd => nd.ProductTypeId);

            modelBuilder.Entity<ProductType>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTypes)
                .HasForeignKey(pt => pt.ProductId);

            // Shelf and Floor relationships
            modelBuilder.Entity<Shelf>()
                .HasMany(s => s.Floors)
                .WithOne(f => f.Shelf)
                .HasForeignKey(f => f.ShelfId);

            // Warehouse and Shelf relationships
            modelBuilder.Entity<Warehouse>()
                .HasMany(w => w.Shelves)
                .WithOne(s => s.Warehouse)
                .HasForeignKey(s => s.WarehouseId);

            // ReceivingNote relationships
            modelBuilder.Entity<ReceivingNote>()
                .HasMany(rn => rn.NoteDetails)
                .WithOne(nd => nd.ReceivingNote)
                .HasForeignKey(nd => nd.ReceivingNoteId);

            // Supplier relationships
            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.ReceivingNotes)
                .WithOne(rn => rn.Supplier)
                .HasForeignKey(rn => rn.SupplierId);

            // StockCard and StockCardDetail relationships
            modelBuilder.Entity<StockCard>()
                .HasMany(sc => sc.StockCardDetails)
                .WithOne(scd => scd.StockCard)
                .HasForeignKey(scd => scd.StockCardId);

            // Group relationships
            modelBuilder.Entity<Group>()
                .HasMany(g => g.GroupPermissions)
                .WithOne(gp => gp.Group)
                .HasForeignKey(gp => gp.GroupId);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.AccountGroups)
                .WithOne(ag => ag.Group)
                .HasForeignKey(ag => ag.GroupId);

            // Other configurations
            modelBuilder.Entity<Permission>()
                .HasMany(p => p.GroupPermissions)
                .WithOne(gp => gp.Permission)
                .HasForeignKey(gp => gp.PermissionId);

            modelBuilder.Entity<ReceivingNote>()
                .Property(rn => rn.Date)
                .IsRequired();

            modelBuilder.Entity<IssueNote>()
                .Property(isn => isn.Date)
                .IsRequired();

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
