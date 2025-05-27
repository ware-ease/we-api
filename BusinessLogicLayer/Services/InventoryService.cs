using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using Data.Model.Request.Inventory;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using System.Linq.Expressions;

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

        public async Task<ServiceResponse> Search(int? pageIndex = null, int? pageSize = null,
                                                  string? keyword = null, string? warehouseId = null, string? productId = null)
        {

            Expression<Func<Inventory, bool>> filter = p =>
                (string.IsNullOrEmpty(keyword) || p.CurrentQuantity.ToString().Contains(keyword)
                || p.Batch.Name.Contains(keyword)
                || p.Batch.Code.Contains(keyword)
                || p.Warehouse.Name.Contains(keyword)) &&
                (string.IsNullOrEmpty(warehouseId) || p.WarehouseId == warehouseId) &&
                (string.IsNullOrEmpty(productId) || p.Batch.ProductId == productId);

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "Warehouse,Batch,Batch.Product,Batch.Product.Unit,Batch.Product.Brand");

            var mappedResults = _mapper.Map<IEnumerable<InventoryDTOv2>>(results).ToList();
            foreach (var inventory in mappedResults)
            {
                var createdByAccount = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == inventory.CreatedBy, "Profile,AccountGroups,AccountGroups.Group");
                if (createdByAccount != null)
                {
                    inventory.CreatedByAvatarUrl = createdByAccount.Profile!.AvatarUrl;
                    inventory.CreatedByFullName = $"{createdByAccount.Profile.FirstName} {createdByAccount.Profile.LastName}";
                    inventory.CreatedByGroup = createdByAccount.AccountGroups.FirstOrDefault()?.Group?.Name;
                }
            }

            int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? totalRecords));

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Tìm thành công!",
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
