using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Entity.Base;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.Category;
using Data.Model.Request.Customer;
using Data.Model.Request.Supplier;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class CustomerService : GenericService<Partner>, ICustomerService
    {
        //ICustomerRepository _customerRepository;

        public CustomerService(IGenericRepository<Partner> genericRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            //_customerRepository = unitOfWork.CustomerRepository;
        }

        public override async Task<ServiceResponse> Get<TResult>()
        {
            var entities = await _genericRepository.Search(p => p.PartnerType == PartnerEnum.Customer);

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
            var customer = await _genericRepository.GetByCondition(b => b.Id == id);

            if (customer == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Customer not found!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(customer);

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
            entity.PartnerType = PartnerEnum.Customer;

            TResult result = _mapper.Map<TResult>(entity);

            try
            {
                entity.CreatedTime = DateTime.Now;
                await _genericRepository.Add(entity);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Add thành công!",
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



        public async Task<CustomerDTO> UpdateCustomer(CustomerUpdateDTO request)
        {
            var existingCustomer = await _genericRepository.GetByCondition(p => p.Id == request.Id);
            if (existingCustomer == null)
                throw new Exception("Customer không tìm thấy");

            if (!string.IsNullOrEmpty(request.Name))
            {
                existingCustomer.Name = request.Name;
            }
            if (!string.IsNullOrEmpty(request.Phone))
            {
                existingCustomer.Phone = request.Phone;
            }
            if (request.Status.HasValue)
                existingCustomer.Status = request.Status.Value;

            _genericRepository.Update(existingCustomer);
            await _unitOfWork.SaveAsync();

            var updatedCustomer = await _genericRepository.GetByCondition(p => p.Id == existingCustomer.Id);
            if (updatedCustomer == null)
                throw new Exception("Update lỗi, customer không tìm thấy sau khi update");

            return _mapper.Map<CustomerDTO>(updatedCustomer);
        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null)
        {

            Expression<Func<Partner, bool>> filter = p =>
            (p.PartnerType == PartnerEnum.Customer &&
            (string.IsNullOrEmpty(keyword)
            || p.Name.Contains(keyword)
            || p.Phone.Contains(keyword)));

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize);

            var mappedResults = _mapper.Map<IEnumerable<TResult>>(results);

            int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? totalRecords));

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Search thành công!",
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