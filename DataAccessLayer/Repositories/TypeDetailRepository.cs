//using Data.Entity;
//using DataAccessLayer.IRepositories;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DataAccessLayer.Repositories
//{
//    public class TypeDetailRepository : ITypeDetailRepository
//    {
//        private readonly WaseEaseDbContext _context;

//        public TypeDetailRepository(WaseEaseDbContext context)
//        {
//            _context = context;
//        }

//        public IQueryable<TypeDetail> GetAllQueryable()
//        {
//            return _context.TypeDetails.AsQueryable();
//        }

//        public async Task<List<TypeDetail>> GetAllAsync()
//        {
//            return await _context.TypeDetails.ToListAsync();
//        }

//        public async Task<TypeDetail> GetByIdAsync(string id)
//        {
//            return await _context.TypeDetails.FirstOrDefaultAsync(td => td.Id == id);
//        }

//        public async Task AddAsync(TypeDetail typeDetail)
//        {
//            await _context.TypeDetails.AddAsync(typeDetail);
//            await _context.SaveChangesAsync();
//        }

//        public async Task UpdateAsync(TypeDetail typeDetail)
//        {
//            _context.TypeDetails.Update(typeDetail);
//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteAsync(TypeDetail typeDetail)
//        {
//            typeDetail.IsDeleted = true;
//            typeDetail.DeletedTime = DateTime.Now;
//            _context.TypeDetails.Update(typeDetail);
//            await _context.SaveChangesAsync();
//        }

//    }
//}
