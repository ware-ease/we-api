using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.ReceivingDetail;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IReceivingDetailService
    {
        Task<PagedResult<ReceivingDetail>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<ReceivingDetail> GetByIdAsync(string id);
        Task<ReceivingDetail> AddAsync(string noteId, string productTypeId, CreateReceivingDetailDTO createReceivingDetailDTO);
        Task<ReceivingDetail> UpdateAsync(string receivingDetailId, UpdateReceivingDetailDTO updateReceivingDetailDTO);
        Task DeleteAsync(string Id, string deletedBy);
    }
}
