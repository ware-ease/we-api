using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class BatchService : GenericService<Batch>, IBatchService
    {
        public BatchService(IGenericRepository<Batch> genericService, 
            IMapper mapper, 
            IUnitOfWork unitOfWork) : base(genericService, mapper, unitOfWork) 
        {
            //public async Task<ServiceResponse> Add()
        }
    }
}
