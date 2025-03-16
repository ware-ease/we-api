using Data.Entity;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {

        public ProductRepository(WaseEaseDbContext context) : base(context)
        {
        }
        public async Task<Product?> GetFullProductById(string id)
        {
            return await _dbSet
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Include(p => p.Unit)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        /*private readonly WaseEaseDbContext _context;

        public ProductRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        // Get all products as IQueryable
        public IQueryable<Product> GetAllQueryable()
        {
            return _context.Products.AsQueryable();
        }

        // Get product by Id
        public async Task<Product> GetByIdAsync(string id)
        {
            //return await _context.Products
            //    .Include(p => p.Category)
            //    .Include(p => p.ProductTypes)
            //    .FirstOrDefaultAsync(p => p.Id == id);

            return null;
        }

        // Add a new product
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        // Update an existing product
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // Soft delete a product
        public async Task DeleteAsync(Product product)
        {
            product.IsDeleted = true;
            product.DeletedTime = DateTime.Now;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // Get products by category Id
        public IQueryable<Product> GetByCategoryIdQueryable(string categoryId)
        {
            //return _context.Products
            //    .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
            //    .Include(p => p.Category)
            //    .Include(p => p.ProductTypes)
            //    .AsQueryable();

            return null;
        }*/
    }
}
