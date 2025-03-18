using BusinessLogicLayer.Generic;
using Data.Model.DTO;
using Data.Model.Request.ProductType;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IProductTypesService : IGenericService
    {
        Task<int> Count();
        Task<ProductTypeDTO> AddProductType(ProductTypeCreateDTO request);
        Task<ProductTypeDTO> UpdateProductType(ProductTypeUpdateDTO request);
        Task<ServiceResponse> GetAllProducts();
    }
}
