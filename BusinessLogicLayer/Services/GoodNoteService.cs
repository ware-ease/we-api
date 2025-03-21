using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using Data.Model.Request.GoodNote;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class GoodNoteService : GenericService<GoodNote>, IGoodNoteService
    {
        private readonly IGoodNoteRepository _goodNoteRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GoodNoteService(IGenericRepository<GoodNote> genericRepository, IUnitOfWork unitOfWork, IMapper mapper)
            : base(genericRepository, mapper, unitOfWork)
        {
            _goodNoteRepository = unitOfWork.GoodNoteRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse> SearchGoodNotes<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                      string? keyword = null, GoodNoteEnum? goodNoteType = null)
        {
            Expression<Func<GoodNoteDetail, bool>> filter = g =>(string.IsNullOrEmpty(keyword) || 
                                                                ((g.GoodNote.ShipperName != null && g.GoodNote.ShipperName.Contains(keyword)) ||
                                                                (g.GoodNote.ReceiverName != null && g.GoodNote.ReceiverName.Contains(keyword)) ||
                                                                (g.GoodNote.Code != null && g.GoodNote.Code.Contains(keyword)) ||
                                                                (g.Batch.Product.Name != null && g.Batch.Product.Name.Contains(keyword))
                                                                )) &&
                                                                (!goodNoteType.HasValue || g.GoodNote.NoteType == goodNoteType.Value);

            var results = await _unitOfWork.GoodNoteDetailRepository.Search(
                filter: filter,
                includeProperties: "GoodNote,Batch," +
                                    "Batch.Product," +
                                    "Batch.Product.Unit," +
                                    "Batch.Product.Brand",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var totalRecords = results.GroupBy(g => g.GoodNoteId).Count();

            var mappedResults = _mapper.Map<IEnumerable<TResult>>(results);
            var groupedResults = results.GroupBy(g => g.GoodNote.Id)
                                        .Select(group => new GoodNoteDTO
                                        {
                                            Id = group.Key,
                                            ReceiverName = group.First().GoodNote.ReceiverName,
                                            ShipperName = group.First().GoodNote.ShipperName,
                                            NoteType = group.First().GoodNote.NoteType,
                                            GoodRequestId = group.First().GoodNote.GoodRequestId,
                                            Code = group.First().GoodNote.Code,
                                            Date = group.First().GoodNote.Date,
                                            CreatedTime = group.First().GoodNote.CreatedTime.ToString(),
                                            CreatedBy = group.First().GoodNote.CreatedBy,
                                            GoodNoteDetails = _mapper.Map<List<GoodNoteDetailDTO>>(group.ToList())
                                        }).ToList();


            int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? 5));

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Search successful!",
                Data = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageIndex = pageIndex ?? 1,
                    PageSize = pageSize ?? 5,
                    Records = groupedResults
                }
            };
        }
        public async Task<ServiceResponse> GetById<TResult>(string id)
        {
            var entities = await _unitOfWork.GoodNoteDetailRepository.Search(g => g.GoodNoteId == id, includeProperties: "GoodNote," +
                                                                                                                         "Batch," +
                                                                                                                         "Batch.Product," +
                                                                                                                         "Batch.Product.Unit," +
                                                                                                                         "Batch.Product.Brand");
            if (entities == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Good note not found!",
                    Data = id
                };
            }

            var groupedResult = new GoodNoteDTO
            {
                Id = entities.First().GoodNote.Id,
                ReceiverName = entities.First().GoodNote.ReceiverName,
                ShipperName = entities.First().GoodNote.ShipperName,
                NoteType = entities.First().GoodNote.NoteType,
                GoodRequestId = entities.First().GoodNote.GoodRequestId,
                Code = entities.First().GoodNote.Code,
                Date = entities.First().GoodNote.Date,
                CreatedTime = entities.First().GoodNote.CreatedTime.ToString(),
                CreatedBy = entities.First().GoodNote.CreatedBy,
                GoodNoteDetails = _mapper.Map<List<GoodNoteDetailDTO>>(entities)
            };

            return new ServiceResponse
            {
                Status = SRStatus.Success,
                Message = "Get good note successfully!",
                Data = groupedResult
            };


        }

        public async Task<ServiceResponse> CreateAsync<TResult>(GoodNoteCreateDTO request)
        {
            if (!string.IsNullOrEmpty(request.GoodRequestId))
            {
                var warehouseExists = await _unitOfWork.GoodRequestRepository.GetByCondition(x => x.Id == request.GoodRequestId);
                if (warehouseExists == null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Good Request not found.",
                        Data = request.GoodRequestId
                    };
                }
                var goodNoteExists = await _unitOfWork.GoodNoteRepository.GetByCondition(x => x.GoodRequestId == request.GoodRequestId);
                if(goodNoteExists != null)
                {
                    return new ServiceResponse
                    {
                        Status = SRStatus.Error,
                        Message = "Good note already exists for this Good Request ID.",
                        Data = request.GoodRequestId
                    };
                }
            }

            if (request.GoodNoteDetails != null && request.GoodNoteDetails.Any())
            {
                foreach (var detail in request.GoodNoteDetails)
                {
                    if (string.IsNullOrEmpty(detail.BatchId))
                    {
                        return new ServiceResponse
                        {
                            Status = SRStatus.Error,
                            Message = "BatchId cannot be null or empty in GoodNoteDetails."
                        };
                    }

                    var batchExists = await _unitOfWork.BatchRepository.GetByCondition(x => x.Id == detail.BatchId);
                    if (batchExists == null)
                    {
                        return new ServiceResponse
                        {
                            Status = SRStatus.Error,
                            Message = $"Batch with ID {detail.BatchId} not found.",
                            Data = detail.BatchId
                        };
                    }
                }
            }
            try
            {
                var entity = _mapper.Map<GoodNote>(request);
                entity.CreatedTime = DateTime.Now;
                await _goodNoteRepository.Add(entity);
                await _unitOfWork.SaveAsync();

                var goodNoteDetails = new List<GoodNoteDetail>();
                foreach (var detail in request.GoodNoteDetails)
                {
                    detail.GoodNoteId = entity.Id;
                    detail.CreatedTime = DateTime.Now;
                    var goodNoteDetail = _mapper.Map<GoodNoteDetail>(detail);
                    goodNoteDetails.Add(goodNoteDetail);
                    await _unitOfWork.GoodNoteDetailRepository.Add(goodNoteDetail);
                }
                await _unitOfWork.SaveAsync();

                await UpdateInventories(entity, goodNoteDetails);

                await _unitOfWork.SaveAsync();

                var result = _mapper.Map<TResult>(entity);
                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Good note created successfully!",
                    Data = result
                };
            }
            catch (DbUpdateException dbEx)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Error creating good note: {ex.Message}"
                };
            }
        }

        private async Task UpdateInventories(GoodNote goodNote, List<GoodNoteDetail> details)
        {
            var batchIds = details.Select(d => d.BatchId).Distinct().ToList();
            var goodRequest = await _unitOfWork.GoodRequestRepository.GetByCondition(g => g.Id == goodNote.GoodRequestId);
            var warehouseId = goodRequest.WarehouseId;
            var requestedWarehouseId = goodRequest.RequestedWarehouseId;

            // Lấy danh sách Inventory cho tất cả Batch liên quan
            var inventories = await _unitOfWork.InventoryRepository.Search(i => batchIds.Contains(i.BatchId));

            foreach (var detail in details)
            {
                var inventory = inventories.FirstOrDefault(i => i.BatchId == detail.BatchId && i.WarehouseId == warehouseId);

                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        WarehouseId = warehouseId,
                        BatchId = detail.BatchId,
                        CurrentQuantity = 0
                    };
                }

                switch (goodNote.NoteType)
                {
                    case GoodNoteEnum.Receive:
                        inventory.CurrentQuantity += detail.Quantity;
                        await _unitOfWork.InventoryRepository.Add(inventory);
                        break;
                    case GoodNoteEnum.Return:
                        inventory.CurrentQuantity += detail.Quantity;
                        await _unitOfWork.InventoryRepository.Add(inventory);
                        break;

                    case GoodNoteEnum.Issue:
                        if (inventory.CurrentQuantity < detail.Quantity)
                        {
                            throw new Exception($"Not enough inventory in warehouse {warehouseId} for batch {detail.BatchId}.");
                        }
                        inventory.CurrentQuantity -= detail.Quantity;
                        await _unitOfWork.InventoryRepository.Add(inventory);
                        break;

                    case GoodNoteEnum.Transfer:
                        var sourceInventory = inventories.FirstOrDefault(i => i.BatchId == detail.BatchId && i.WarehouseId == requestedWarehouseId);
                        if (sourceInventory == null || sourceInventory.CurrentQuantity < detail.Quantity)
                        {
                            throw new Exception($"Not enough inventory in requested warehouse {requestedWarehouseId} for batch {detail.BatchId}.");
                        }
                        sourceInventory.CurrentQuantity -= detail.Quantity;
                        _unitOfWork.InventoryRepository.Update(sourceInventory);
                        //if (inventory == null)
                        //{
                        //    inventory = new Inventory
                        //    {
                        //        WarehouseId = warehouseId,
                        //        BatchId = detail.BatchId,
                        //        CurrentQuantity = 0
                        //    };
                        //    await _unitOfWork.InventoryRepository.Add(inventory);
                        //}
                        inventory.CurrentQuantity += detail.Quantity;
                        await _unitOfWork.InventoryRepository.Add(inventory);
                        break;
                }
            }
        }



        public async Task<ServiceResponse> DeleteAsync(string id)
        {
            var entity = await _goodNoteRepository.GetByCondition(x => x.Id == id);
            if (entity == null)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.NotFound,
                    Message = "Good note not found!",
                    Data = id
                };
            }

            try
            {
                _goodNoteRepository.Delete(entity);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse
                {
                    Status = SRStatus.Success,
                    Message = "Good note deleted successfully!",
                    Data = id
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse
                {
                    Status = SRStatus.Error,
                    Message = $"Error deleting good note: {ex.Message}"
                };
            }
        }
    }
}
