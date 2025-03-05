using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
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
    public class WarehouseService : GenericService<Warehouse>, IWarehouseService
    {
        IWarehouseRepository _warehouseRepository;
        IMapper _mapper;
        IUnitOfWork _unitOfWork;

        public WarehouseService(IGenericRepository<Warehouse> genericRepository, IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _warehouseRepository = unitOfWork.WarehouseRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse> GetFullWarehouseInfo<TResult>(string id)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByCondition(
                w => w.Id == id,
                includeProperties: "Areas.Shelves.Floors.Cells"
                //includeProperties: "Areas,Areas.Shelves,Areas.Shelves.Floors,Areas.Shelves.Floors.Cells,AccountWarehouses,ReceivingNotes,IssueNotes,Inventories,StockBooks"

            );

            if (warehouse == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Warehouse not found!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(warehouse);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get full warehouse info successfully!",
                Data = result
            };
        }

    }
}
