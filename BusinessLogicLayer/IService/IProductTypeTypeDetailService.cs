//using BusinessLogicLayer.Models.Pagination;
//using BusinessLogicLayer.Models.ProductTypeTypeDetail;
//using Data.Entity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BusinessLogicLayer.IService
//{
//    public interface IProductTypeTypeDetailService
//    {
//        Task<PagedResult<ProductTypeTypeDetail>> GetAllAsync(int? pageNumber, int? pageSize);
//        Task<ProductTypeTypeDetail> GetByIdAsync(string id);
//        Task<ProductTypeTypeDetail> AddAsync(string typeDetailId, string productTypeId,
//            CreateProductTypeTypeDetailDTO createProductTypeTypeDetailDTO);
//        Task<ProductTypeTypeDetail> UpdateAsync(string productTypeTypeDetailId,
//            UpdateProductTypeTypeDetailDTO updateProductTypeTypeDetailDTO);
//        Task DeleteAsync(string Id, string deletedBy);
//    }
//}
