using BusinessLogicLayer.Generic;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IWarehouseService : IGenericService
    {
        Task<ServiceResponse> GetFullWarehouseInfo<TResult>(string id);

    }
}
