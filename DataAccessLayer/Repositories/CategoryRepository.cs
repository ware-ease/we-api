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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly WaseEaseDbContext _context;

        public CategoryRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        public IQueryable<Category> GetAllQueryable()
        {
            return _context.Categories.AsQueryable();
        }

        public async Task<Category> GetByIdAsync(string id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            category.IsDeleted = true;
            category.DeletedTime = DateTime.Now;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
