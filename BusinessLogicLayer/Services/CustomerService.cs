using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Entity.Base;
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
    public class CustomerService : GenericService<Customer>, ICustomerService
    {
        ICustomerRepository _customerRepository;

        public CustomerService(IGenericRepository<Customer> genericRepository, IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _customerRepository = unitOfWork.CustomerRepository;
        }

        public void Test()
        {
            var customer = _customerRepository.Get();
            Console.WriteLine(customer);
        }
    }
}
