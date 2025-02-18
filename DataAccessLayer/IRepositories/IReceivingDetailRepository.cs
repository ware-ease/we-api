using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IReceivingDetailRepository
    {
        IQueryable<ReceivingDetail> GetAllQueryable();
        Task<List<ReceivingDetail>> GetAllAsync();
        Task<ReceivingDetail> GetByIdAsync(string id);
        Task AddAsync(ReceivingDetail receivingDetail);
        Task UpdateAsync(ReceivingDetail receivingDetail);
        Task DeleteAsync(ReceivingDetail receivingDetail);
    }
}
