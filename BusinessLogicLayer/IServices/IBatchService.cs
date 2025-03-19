using BusinessLogicLayer.Generic;
using Data.Model.DTO;
using Data.Model.Request.Batch;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IBatchService : IGenericService
    {
        Task<int> CountBatch();
        Task<BatchDTO> AddBatch(BatchCreateDTO request);
        Task<BatchDTO> UpdateBatch(BatchUpdateDTO request);
        Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null);
    }
}
