using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.InventoryLocation;
using Data.Model.Request.Warehouse;
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

        public async Task<ServiceResponse> GetLocationsByInventoryId(string inventoryId)
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByCondition(
                i => i.Id == inventoryId,
                includeProperties: "InventoryLocations.Location,InventoryLocations," +
                "Batch,Batch.Product,Batch.Product.Unit,Batch.Product.Brand"
            );

            if (inventory == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Inventory not found.",
                    Data = inventoryId
                };
            }

            //var locations = inventory.InventoryLocations
            //    .Select(il => il.Location)
            //.Distinct()
            //.ToList();

            //var locationDtos = _mapper.Map<List<InventoryLocationDTO>>(inventory.InventoryLocations);

            var locationsByInventoryDtos = _mapper.Map<InventoryDTOv2>(inventory); 
            locationsByInventoryDtos.InventoryLocations = _mapper.Map<List<InventoryLocationDTO>>(inventory.InventoryLocations);

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Locations retrieved successfully.",
                Data = locationsByInventoryDtos
            };
        }

        public async Task<ServiceResponse> GetLocationsByBatchId(string batchId)
        {
            var inventories = await _unitOfWork.InventoryRepository.Search(
                i => i.BatchId == batchId,
                includeProperties: "InventoryLocations.Location"
            );

            if (inventories == null || !inventories.Any())
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "No inventories found for the given batch.",
                    Data = batchId
                };
            }

            var locations = inventories
                .SelectMany(i => i.InventoryLocations)
                .Select(il => il.Location)
                .Distinct()
                .ToList();

            var locationDtos = _mapper.Map<List<LocationDTO>>(locations);

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Locations retrieved successfully.",
                Data = locationDtos
            };
        }

    }
}
