using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.Request.Area;
using Data.Model.Request.Warehouse;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.UnitOfWork;

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
                includeProperties: "Locations"
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
            var warehouse = await _unitOfWork.WarehouseRepository.Get(request.Id!);

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
                foreach (var locationDto in request.Locations)
                {
                    // 🔥 Kiểm tra nếu location có ParentId thì phải tồn tại trong DB
                    if (!string.IsNullOrWhiteSpace(locationDto.ParentId))
                    {
                        var parentExists = await _unitOfWork.LocationRepository.GetByCondition(l => l.Id == locationDto.ParentId);
                        if (parentExists == null)
                        {
                            return new ServiceResponse
                            {
                                Status = Data.Enum.SRStatus.Error,
                                Message = $"Parent location with ID '{locationDto.ParentId}' not found.",
                                Data = locationDto.ParentId
                            };
                        }
                    }
                    var location = _mapper.Map<Location>(locationDto);
                    location.WarehouseId = request.Id!;
                    location.Warehouse = warehouse;
                    // 🔥 Kiểm tra ParentId: nếu là "" thì chuyển thành null
                    location.ParentId = string.IsNullOrWhiteSpace(locationDto.ParentId) ? null : locationDto.ParentId;
                    await _unitOfWork.LocationRepository.Add(location);
                                                             
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

        //public async Task<ServiceResponse> GetWarehouseAreas(string warehouseId)
        //{
        //    var areas = await _unitOfWork.AreaRepository.GetAllNoPaging(a => a.WarehouseId == warehouseId);

        //    if (areas == null)
        //    {
        //        return new ServiceResponse
        //        {
        //            Status = Data.Enum.SRStatus.NotFound,
        //            Message = "No areas found for this warehouse",
        //            Data = warehouseId
        //        };
        //    }

        //    var result = _mapper.Map<List<AreaDTO>>(areas);

        //    return new ServiceResponse
        //    {
        //        Status = Data.Enum.SRStatus.Success,
        //        Message = "Warehouse areas retrieved successfully",
        //        Data = result
        //    };
        //}

//        public async Task<ServiceResponse> AddWarehouseArea(CreateAREADto request)
//        {
//            var warehouse = await _unitOfWork.WarehouseRepository.Get(request.WarehouseId!);

//            if (warehouse == null)
//            {
//                return new ServiceResponse
//                {
//                    Status = Data.Enum.SRStatus.NotFound,
//                    Message = "Warehouse not found",
//                    Data = request.WarehouseId
//                };
//            }

//            var area = new Area
//            {
//                Name = request.Name,
//                WarehouseId = request.WarehouseId
//            };

//            await _unitOfWork.AreaRepository.Add(area);
//            await _unitOfWork.SaveAsync();

//            return new ServiceResponse
//            {
//                Status = Data.Enum.SRStatus.Success,
//                Message = "Area added successfully",
//                Data = area.Id
//            };
//        }

//        public async Task<ServiceResponse> GetShelvesByArea(string areaId)
//        {

//            var shelves = await _unitOfWork.ShelfRepository.GetAllNoPaging(s => s.AreaId == areaId, includeProperties: "Floors.Cells"
//);

//            if (shelves == null || !shelves.Any())
//            {
//                return new ServiceResponse
//                {
//                    Status = Data.Enum.SRStatus.NotFound,
//                    Message = "No shelves found for this area",
//                    Data = areaId
//                };
//            }

//            var result = _mapper.Map<List<ShelfDto>>(shelves);

//            return new ServiceResponse
//            {
//                Status = Data.Enum.SRStatus.Success,
//                Message = "Shelves retrieved successfully",
//                Data = result
//            };
//        }

//        public async Task<ServiceResponse> AddShelfWithStructure(CreateShelfDto request, string areaId)
//        {
//            var area = await _unitOfWork.AreaRepository.Get(areaId);

//            if (area == null)
//            {
//                return new ServiceResponse
//                {
//                    Status = Data.Enum.SRStatus.NotFound,
//                    Message = "Area not found",
//                    Data = areaId
//                };
//            }

//            var shelf = new Shelf
//            {
//                Code = request.Code,
//                AreaId = areaId
//            };

//            await _unitOfWork.ShelfRepository.Add(shelf);

//            foreach (var floorDto in request.Floors)
//            {
//                var floor = new Floor
//                {
//                    Number = floorDto.Number,
//                    ShelfId = shelf.Id
//                };

//                await _unitOfWork.FloorRepository.Add(floor);

//                foreach (var cellDto in floorDto.Cells)
//                {
//                    var cell = new Cell
//                    {
//                        Number = cellDto.Number,
//                        Length = cellDto.Length,
//                        Height = cellDto.Height,
//                        MaxLoad = cellDto.MaxLoad,
//                        FloorId = floor.Id
//                    };

//                    await _unitOfWork.CellRepository.Add(cell);
//                }
//            }

//            await _unitOfWork.SaveAsync();

//            return new ServiceResponse
//            {
//                Status = Data.Enum.SRStatus.Success,
//                Message = "Shelf structure created successfully",
//                Data = shelf.Id
//            };
//        }

//        public async Task<ServiceResponse> GetShelfDetails(string shelfId)
//        {
//            var shelf = await _unitOfWork.ShelfRepository.GetByCondition(
//                s => s.Id == shelfId,
//                includeProperties: "Floors.Cells"
//            );

//            var area = await _unitOfWork.AreaRepository.Get(shelf.AreaId);
//            area.Shelves.Add(shelf);
//            var warehouse = await _unitOfWork.WarehouseRepository.Get(area.WarehouseId);
//            warehouse.Areas.Add(area);

//            if (shelf == null)
//            {
//                return new ServiceResponse
//                {
//                    Status = Data.Enum.SRStatus.NotFound,
//                    Message = "Shelf not found",
//                    Data = shelfId
//                };
//            }

//            var result = _mapper.Map<WarehouseFullInfoDTO>(warehouse);

//            return new ServiceResponse
//            {
//                Status = Data.Enum.SRStatus.Success,
//                Message = "Shelf details retrieved successfully",
//                Data = result
//            };
//        }

    }
}