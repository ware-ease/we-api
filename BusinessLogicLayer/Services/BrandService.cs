using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class BrandService : GenericService<Brand>, IBrandService
    {
        public BrandService(IGenericRepository<Brand> genericRepository, IMapper mapper, IUnitOfWork unitOfWork)
            : base(genericRepository, mapper, unitOfWork)
        {
        }
    }
}
