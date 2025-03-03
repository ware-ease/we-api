//using BusinessLogicLayer.Models.Pagination;
//using BusinessLogicLayer.Models.TypeDetail;
//using Data.Entity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BusinessLogicLayer.IService
//{
//    public interface ITypeDetailService
//    {
//        Task<PagedResult<TypeDetail>> GetAllAsync(int? pageNumber, int? pageSize);
//        Task<TypeDetail> GetByIdAsync(string id);
//        Task<TypeDetail> AddAsync(CreateTypeDetailDTO createTypeDetailDTO);
//        Task<TypeDetail> UpdateAsync(string typeDetaiId, UpdateTypeDetailDTO updateTypeDetailDTO);
//        Task DeleteAsync(string Id, string deletedBy);
//    }
//}
