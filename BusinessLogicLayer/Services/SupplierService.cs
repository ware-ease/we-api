using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Supplier;
using Data.Entity;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.Customer;
using Data.Model.Request.Supplier;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class SupplierService : GenericService<Partner>, ISupplierService
    {

        /*private readonly ISupplierRepository _repository;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;*/
        //private readonly IGenericRepository<Partner> _partnerRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;


        public SupplierService(IGenericRepository<Partner> genericRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public override async Task<ServiceResponse> Get<TResult>()
        {
            var entities = await _genericRepository.Get(p => p.PartnerType == PartnerEnum.Supplier);

            List<TResult> mappedResults = _mapper.Map<List<TResult>>(entities);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = mappedResults
            };
        }


        public override async Task<ServiceResponse> Get<TResult>(string id)
        {
            var supplier = await _genericRepository.GetByCondition(b => b.Id == id);

            if (supplier == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Supplier not found!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(supplier);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = result
            };
        }


        public override async Task<ServiceResponse> Add<TResult, TRequest>(TRequest request)
        {
            Partner entity = _mapper.Map<Partner>(request);

            // Gán giá trị cụ thể cho biến mong muốn
            entity.PartnerType = PartnerEnum.Supplier;

            TResult result = _mapper.Map<TResult>(entity);

            try
            {
                entity.CreatedTime = DateTime.Now;
                await _genericRepository.Add(entity);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Add successfully!",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = ex.Message,
                    Data = request
                };
            }
        }



        public async Task<SupplierDTO> UpdateSupplier(SupplierUpdateDTO request)
        {
            var existingSupplier = await _genericRepository.Get(request.Id);
            if (existingSupplier == null)
                throw new Exception("Supplier not found");

            if (!string.IsNullOrEmpty(request.Name))
            {
                existingSupplier.Name = request.Name;
            }
            if (!string.IsNullOrEmpty(request.Phone))
            {
                existingSupplier.Phone = request.Phone;
            }
            if (request.Status.HasValue)
                existingSupplier.Status = request.Status.Value;

            _genericRepository.Update(existingSupplier);
            await _unitOfWork.SaveAsync();

            var updatedSupplier = await _genericRepository.Get(existingSupplier.Id);
            if (updatedSupplier == null)
                throw new Exception("Update failed, supplier not found after update");

            return _mapper.Map<SupplierDTO>(updatedSupplier);
        }

        /*public SupplierService(ISupplierRepository supplierRepository, IGenericPaginationService genericPaginationService, IMapper mapper)
        {
            _repository = supplierRepository;
            _genericPaginationService = genericPaginationService;
            _mapper = mapper;
        }
        public async Task<PagedResult<Supplier>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = _repository.GetAllQueryable();
            return await _genericPaginationService.GetPagedDataAsync(query, pageNumber, pageSize);
        }

        public async Task<Supplier> GetByIdAsync(string id)
        {
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id");
            }
            return supplier;
        }

        public async Task<Supplier> AddAsync(CreateSupplierDTO supplierDTO)
        {
            if (string.IsNullOrWhiteSpace(supplierDTO.Name))
            {
                throw new ArgumentException("Tên Supplier không được để trống");
            }
            if (string.IsNullOrWhiteSpace(supplierDTO.Phone))
            {
                throw new ArgumentException("Số điện thoại không được để trống");
            }
            if (!Regex.IsMatch(supplierDTO.Phone, @"^(0[1-9]{1}[0-9]{8,9})$"))
            {
                throw new ArgumentException("Số điện thoại không hợp lệ, nhập đúng định dạng số Việt Nam (10-11 chữ số)");
            }
            if ((supplierDTO.Status != true) && (supplierDTO.Status != false))
            {
                throw new ArgumentException("Status chỉ được true hoặc false");
            }
            if (string.IsNullOrWhiteSpace(supplierDTO.CreatedBy))
            {
                throw new ArgumentException("Người tạo không được để trống");
            }
            var supplier = _mapper.Map<Supplier>(supplierDTO);
            supplier.CreatedTime = DateTime.Now;
            await _repository.AddAsync(supplier);
            return supplier;
        }

        public async Task<Supplier> UpdateAsync(string id, UpdateSupplierDTO supplierDTO)
        {
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier == null)
            {
                throw new ArgumentException("Không thể tìm thấy Supplier với ID này");
            }

            if (!string.IsNullOrWhiteSpace(supplierDTO.Name))
            {
                supplier.Name = supplierDTO.Name;
            }
            if (!string.IsNullOrWhiteSpace(supplierDTO.Phone))
            {
                if (!Regex.IsMatch(supplierDTO.Phone, @"^(0[1-9]{1}[0-9]{8,9})$"))
                {
                    throw new ArgumentException("Số điện thoại không hợp lệ, nhập đúng định dạng số Việt Nam (10-11 chữ số)");
                }
                supplier.Phone = supplierDTO.Phone;
            }
            if ((supplierDTO.Status != true) && (supplierDTO.Status != false))
            {
                throw new ArgumentException("Status chỉ được true hoặc false");
            }

            supplier.Status = supplierDTO.Status;
            supplier.LastUpdatedBy = supplierDTO.LastUpdatedBy;
            supplier.LastUpdatedTime = DateTime.Now;

            await _repository.UpdateAsync(supplier);
            return supplier;
        }

        public async Task DeleteAsync(string Id, string deletedBy)
        {
            var supplier = await _repository.GetByIdAsync(Id);
            if (supplier == null)
            {
                throw new ArgumentException("Không thể tìm thấy Id để Delete");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Người xóa không được để trống");
            }
            supplier.DeletedBy = deletedBy;
            supplier.DeletedTime = DateTime.Now;
            supplier.IsDeleted = true;
            await _repository.UpdateAsync(supplier);
        }*/
    }
}
