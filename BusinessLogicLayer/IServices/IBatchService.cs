using BusinessLogicLayer.Generic;
using Data.Model.DTO;
using Data.Model.Request.Batch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IBatchService : IGenericService
    {
        Task<BatchDTO> AddBatch(BatchCreateDTO request);
        Task<BatchDTO> UpdateBatch(BatchUpdateDTO request);
    }
}
