using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity;
using Data.Entity.Base;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        //public virtual DbSet<Cell> Cells { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        //public virtual DbSet<Floor> Floors { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupPermission> GroupPermissions { get; set; }
        public virtual DbSet<IssueNote> IssueNotes { get; set; }
        public virtual DbSet<IssueDetail> IssueDetails { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<ReceivingDetail> ReceivingDetails { get; set; }
        public virtual DbSet<ReceivingNote> ReceivingNotes { get; set; }
        //public virtual DbSet<Shelf> Shelves { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        //public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<ErrorTicket> ErrorTickets { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<InventoryCount> InventoryChecks { get; set; }
        public virtual DbSet<InventoryCountDetail> InventoryCheckDetails { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<ScheduleSetting> ScheduleSettings { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<GoodNote> GoodNotes { get; set; }
        public virtual DbSet<GoodNoteDetail> GoodNoteDetails { get; set; }
        public virtual DbSet<GoodRequest> GoodRequests { get; set; }
        public virtual DbSet<GoodRequestDetail> GoodRequestDetails { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<LocationLog> LocationLogs { get; set; }
        public virtual DbSet<InventoryAdjustment> InventoryAdjustments { get; set; }
        public virtual DbSet<InventoryAdjustmentDetail> InventoryAdjustmentDetails { get; set; }
        public virtual DbSet<InventoryCount> InventoryCounts { get; set; }
        public virtual DbSet<InventoryCountDetail> InventoryCountDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                Env.Load();
                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DB_URL"));
            }

            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.NavigationBaseIncludeIgnored));
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
