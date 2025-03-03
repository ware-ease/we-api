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
        public virtual DbSet<AccountAction> AccountActions { get; set; }
        public virtual DbSet<AccountWarehouse> AccountWarehouses { get; set; }
        public virtual DbSet<AppAction> Actions { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Cell> Cells { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Floor> Floors { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupAction> GroupActions { get; set; }
        public virtual DbSet<IssueNote> IssueNotes { get; set; }
        public virtual DbSet<IssueDetail> IssueDetails { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ReceivingDetail> ReceivingDetails { get; set; }
        public virtual DbSet<ReceivingNote> ReceivingNotes { get; set; }
        public virtual DbSet<Shelf> Shelves { get; set; }
        public virtual DbSet<CellBatch> StockCards { get; set; }
        public virtual DbSet<InOutDetail> StockCardDetails { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<ErrorTicket> ErrorTickets { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<InventoryCheck> InventoryChecks { get; set; }
        public virtual DbSet<InventoryCheckDetail> InventoryCheckDetails { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<ScheduleSetting> ScheduleSettings { get; set; }
        public virtual DbSet<Unit> Units { get; set; }

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
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var foreignKey in entityType.GetForeignKeys())
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
                }
            }
        }
    }
}
