using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Supplier;
using Data.Entity;
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
    public class SupplierService : GenericService<Supplier>, ISupplierService
    {

        /*private readonly ISupplierRepository _repository;
        private readonly IGenericPaginationService _genericPaginationService;
        private readonly IMapper _mapper;*/
        private readonly IGenericRepository<Supplier> _supplierRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;


        public SupplierService(IGenericRepository<Supplier> genericRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _supplierRepository = genericRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public override async Task<ServiceResponse> Update<TResult, TRequest>(TRequest request)
        {
            var supplierUpdateDto = request as SupplierUpdateDTO;
            if (supplierUpdateDto == null || string.IsNullOrEmpty(supplierUpdateDto.Id))
                throw new Exception("Invalid supplier update request: missing Id");
            var existingSupplier = await _supplierRepository.Get(supplierUpdateDto.Id);
            if (existingSupplier == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Không thể tìm thấy Supplier với Id này",
                    Data = supplierUpdateDto.Id
                };
            }
            _mapper.Map(request, existingSupplier);

            try
            {
                _supplierRepository.Update(existingSupplier);
                await _unitOfWork.SaveAsync();

                TResult result = _mapper.Map<TResult>(existingSupplier);
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Update successfully!",
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
