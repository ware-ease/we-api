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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class InventoryService : GenericService<Inventory>, IInventoryService
    {
        private readonly IGenericRepository<Batch> _batchRepository;
        private readonly IGenericRepository<Warehouse> _warehouseRepository;
        public InventoryService(IGenericRepository<Inventory> genericRepository,
            IGenericRepository<Batch> batchRepository,
            IGenericRepository<Warehouse> warehouseRepository,
            IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _batchRepository = batchRepository;
            _warehouseRepository = warehouseRepository;
        }

        public override async Task<ServiceResponse> Get<TResult>(string id)
        {
            var inventory = await _genericRepository.GetByCondition(
                p => p.Id == id,
                includeProperties: "Batch,Warehouse"
            );

            if (inventory == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Inventory not found!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(inventory);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = result
            };
        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null)
        {

            Expression<Func<Inventory, bool>> filter = p =>
                (string.IsNullOrEmpty(keyword) || p.CurrentQuantity.ToString().Contains(keyword)
                || p.Batch.Name.Contains(keyword)
                || p.Batch.Code.Contains(keyword)
                || p.Warehouse.Name.Contains(keyword));

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "Batch,Warehouse");

            var mappedResults = _mapper.Map<IEnumerable<TResult>>(results);

            int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? totalRecords));

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Search successful!",
                Data = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageIndex = pageIndex ?? 1,
                    PageSize = pageSize ?? totalRecords,
                    Records = mappedResults
                }
            };
        }
    }
}
