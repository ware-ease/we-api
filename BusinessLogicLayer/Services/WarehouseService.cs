using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.Request.Warehouse;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.UnitOfWork;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ServiceResponse> CreateStructureAsync(CreateWarehouseStructureRequest request)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByID(request.Id!);

            if (warehouse == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Warehouse not found",
                    Data = request.Id
                };
            }

            try
            {
                foreach (var areaDto in request.Areas)
                {
                    var area = new Area
                    {
                        Name = areaDto.Name,
                        WarehouseId = warehouse.Id
                    };

                    await _unitOfWork.AreaRepository.Add(area);

                    foreach (var shelfDto in areaDto.Shelves)
                    {
                        var shelf = new Shelf
                        {
                            Code = shelfDto.Code,
                            AreaId = area.Id
                        };

                        await _unitOfWork.ShelfRepository.Add(shelf);

                        foreach (var floorDto in shelfDto.Floors)
                        {
                            var floor = new Floor
                            {
                                Number = floorDto.Number,
                                ShelfId = shelf.Id
                            };

                            await _unitOfWork.FloorRepository.Add(floor);

                            foreach (var cellDto in floorDto.Cells)
                            {
                                var cell = new Cell
                                {
                                    Number = cellDto.Number,
                                    Length = cellDto.Length,
                                    Height = cellDto.Height,
                                    MaxLoad = cellDto.MaxLoad,
                                    FloorId = floor.Id
                                };

                                await _unitOfWork.CellRepository.Add(cell);
                            }
                        }
                    }
                }

                await _unitOfWork.SaveAsync();
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Success,
                    Message = "Warehouse structure created successfully",
                    Data = warehouse.Id
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Error,
                    Message = $"Error creating warehouse structure: {ex.Message}"
                };
            }
        }
    }
}