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
    public class ProductTypeTypeDetailRepository : IProductTypeTypeDetailRepository
    {
        private readonly WaseEaseDbContext _context;

        public ProductTypeTypeDetailRepository(WaseEaseDbContext context)
        {
            _context = context;
        }

        public IQueryable<ProductTypeTypeDetail> GetAllQueryable()
        {
            //return _context.ProductTypeTypeDetails
            //    .Include(pttd => pttd.TypeDetail)
            //    .Include(pttd => pttd.ProductType)
            //    .AsQueryable();

            return null;
        }

        public async Task<List<ProductTypeTypeDetail>> GetAllAsync()
        {
            //return await _context.ProductTypeTypeDetails
            //    .Include(pttd => pttd.TypeDetail)
            //    .Include(pttd => pttd.ProductType)
            //    .ToListAsync();

            return null;
        }

        public async Task<ProductTypeTypeDetail> GetByIdAsync(string id)
        {
            //return await _context.ProductTypeTypeDetails
            //    .Include(pttd => pttd.TypeDetail)
            //    .Include(pttd => pttd.ProductType)
            //    .FirstOrDefaultAsync(pttd => pttd.Id == id);

            return null;
        }

        public async Task AddAsync(ProductTypeTypeDetail productTypeTypeDetail)
        {
            //await _context.ProductTypeTypeDetails.AddAsync(productTypeTypeDetail);
            //await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductTypeTypeDetail productTypeTypeDetail)
        {
            //_context.ProductTypeTypeDetails.Update(productTypeTypeDetail);
            //await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductTypeTypeDetail productTypeTypeDetail)
        {
            //productTypeTypeDetail.IsDeleted = true;
            //productTypeTypeDetail.DeletedTime = DateTime.Now;
            //_context.ProductTypeTypeDetails.Update(productTypeTypeDetail);
            //await _context.SaveChangesAsync();
        }

    }
}
