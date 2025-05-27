using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.Batch;
using Data.Model.Request.Product;
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
    public class BatchService : GenericService<Batch>, IBatchService
    {
        private readonly IGenericRepository<Batch> _batchRepository;
        private readonly IGenericRepository<Partner> _partnerRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<AccountGroup> _accountGroupRepository;
        private readonly IGenericRepository<Group> _groupRepository;
        private readonly IMapper _mapper;
        public readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseService _firebaseService;
        public BatchService(IGenericRepository<Batch> genericService,
            IGenericRepository<Partner> partnerRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<Account> accountRepository,
            IGenericRepository<AccountGroup> accountGroupRepository,
            IGenericRepository<Group> groupRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IFirebaseService firebaseService) : base(genericService, mapper, unitOfWork)
        {
            _batchRepository = genericService;
            _partnerRepository = partnerRepository;
            _productRepository = productRepository;
            _accountRepository = accountRepository;
            _accountGroupRepository = accountGroupRepository;
            _groupRepository = groupRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
        }

        public async Task<int> CountBatch()
        {
            var batches = await _batchRepository.GetAllNoPaging();
            return batches.Count(b => !b.IsDeleted);
        }

        public override async Task<ServiceResponse> Get<TResult>()
        {
            var batches = await _genericRepository.GetAllNoPaging();

            List<TResult> mappedResults = _mapper.Map<List<TResult>>(batches);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = mappedResults
            };
        }

        public override async Task<ServiceResponse> Get<TResult>(string id)
        {
            var batch = await _genericRepository.GetByCondition(b => b.Id == id);

            if (batch == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Batch not found!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(batch);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = result
            };
        }

        public async Task<BatchDTO> AddBatch(BatchCreateDTO request)
        {

            var product = await _productRepository.GetByCondition(p => p.Id == request.ProductId);
            if (product == null)
                throw new Exception("Product không tồn tại");

            if (request.InboundDate >= DateOnly.FromDateTime(DateTime.Now))
                throw new Exception("InboundDate không được đặt ở tương lai");

            var batch = _mapper.Map<Batch>(request);

            await _genericRepository.Insert(batch);
            await _unitOfWork.SaveAsync();

            try
            {
                var userIds = (await _accountGroupRepository.GetAllNoPaging(x => x.GroupId == "4")).Select(x => x.AccountId).Distinct().ToList();
                await _firebaseService.SendNotificationToUsersAsync(userIds, "Batch mới vừa được tạo.", $"Batch Code: {batch.Code}", NotificationType.BATCH_CREATED, null);
            }
            catch
            {
                throw new Exception("Lỗi khi gửi thông báo batch");
            }


            return _mapper.Map<BatchDTO>(batch);
        }

        public async Task<BatchDTO> UpdateBatch(BatchUpdateDTO request)
        {
            var existingBatch = await _batchRepository.GetByCondition(p => p.Id == request.Id);
            if (existingBatch == null)
                throw new Exception("Batch not found");

            if (!string.IsNullOrEmpty(request.SupplierId))
            {
                var supplier = await _partnerRepository.GetByCondition(p => p.Id == request.SupplierId);
                if (supplier == null)
                    throw new Exception("Supplier not found");
                existingBatch.SupplierId = request.SupplierId;
            }
            if (!string.IsNullOrEmpty(request.ProductId))
            {
                var product = await _productRepository.GetByCondition(p => p.Id == request.ProductId);
                if (product == null)
                    throw new Exception("Product not found");
                existingBatch.ProductId = request.ProductId;
            }

            if (!string.IsNullOrEmpty(request.Code))
            {
                existingBatch.Code = request.Code;
            }
            if (!string.IsNullOrEmpty(request.Name))
            {
                existingBatch.Name = request.Name;
            }
            if (request.InboundDate.HasValue)
            {
                existingBatch.InboundDate = request.InboundDate.Value;
            }
            /*if (request.ExpDate.HasValue)
            {
                existingBatch.ExpDate = request.ExpDate.Value;
            }*/

            /*if (!string.IsNullOrEmpty(request.InventoryId))
            {
                existingBatch.InventoryId = request.InventoryId;
            }*/

            _batchRepository.Update(existingBatch);
            await _unitOfWork.SaveAsync();

            var updatedBatch = await _batchRepository.GetByCondition(p => p.Id == existingBatch.Id);
            if (updatedBatch == null)
                throw new Exception("Update failed, batch not found after update");

            try
            {
                var groupIds = new List<string> { "4", "2" };

                var userIds = (await _accountGroupRepository.GetAllNoPaging(x => groupIds.Contains(x.GroupId)))
                .Select(x => x.AccountId)
                .Distinct()
                .ToList();
                await _firebaseService.SendNotificationToUsersAsync(userIds, "Batch mới được chỉnh sửa.", $"Batch Code: {updatedBatch.Code}", NotificationType.BATCH_CREATED, null);
            }
            catch
            {
                throw new Exception("Lỗi khi gửi thông báo batch");
            }

            return _mapper.Map<BatchDTO>(updatedBatch);
        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null, string? productId = null)
        {

            Expression<Func<Batch, bool>> filter = p =>
            (string.IsNullOrEmpty(keyword)
                || p.Name.Contains(keyword)
                || p.Product.Name.Contains(keyword)
                || p.Code.Contains(keyword))
                &&
    (string.IsNullOrEmpty(productId) || p.ProductId == productId);

            var totalRecords = await _batchRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "Product,Product.ProductType,Product.ProductType.Category,Product.Brand,Product.Unit");

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
