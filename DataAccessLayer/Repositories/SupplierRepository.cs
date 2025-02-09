using Data.Entity;
using DataAccessLayer.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly WaseEaseDbContext _context;

        public SupplierRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        public IQueryable<Supplier> GetAllQueryable()
        {
            return _context.Suppliers.AsQueryable();
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task<Supplier> GetByIdAsync(string id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task AddAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Supplier supplier)
        {
            supplier.IsDeleted = true;
            supplier.DeletedTime = DateTime.Now;
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

    }
}
