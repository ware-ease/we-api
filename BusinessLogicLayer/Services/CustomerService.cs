using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Entity.Base;
using Data.Enum;
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
            var entities = await _genericRepository.Get(p => p.PartnerType == PartnerEnum.Customer);

            List<TResult> mappedResults = _mapper.Map<List<TResult>>(entities);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = mappedResults
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



        public override async Task<ServiceResponse> Update<TResult, TRequest>(TRequest request)
        {
            var customerUpdateDto = request as CustomerUpdateDTO;
            if (customerUpdateDto == null || string.IsNullOrEmpty(customerUpdateDto.Id))
                throw new Exception("Invalid supplier update request: missing Id");
            var existingCustomer = await _genericRepository.Get(customerUpdateDto.Id);
            if (existingCustomer == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Không thể tìm thấy Customer với Id này",
                    Data = customerUpdateDto.Id
                };
            }
            _mapper.Map(request, existingCustomer);

            try
            {
                _genericRepository.Update(existingCustomer);
                await _unitOfWork.SaveAsync();

                TResult result = _mapper.Map<TResult>(existingCustomer);
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
    }
}
