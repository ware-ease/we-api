using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Enum;
using Data.Model.DTO;
using Data.Model.Request.InventoryCount;
using Data.Model.Request.Schedule;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class InventoryCountService : GenericService<InventoryCount>, IInventoryCountService
    {
        private readonly IGenericRepository<Schedule> _scheduleRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<InventoryCountDetail> _inventoryCountDetailRepository;
        private readonly IGenericRepository<Inventory> _inventoryRepository;
        private readonly IGenericRepository<Batch> _batchRepository;
        private readonly IGenericRepository<Warehouse> _warehouseRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<AccountGroup> _accountGroupRepository;
        private readonly IFirebaseService _firebaseService;
        private readonly ICodeGeneratorService _codeGeneratorService;
        public InventoryCountService(IGenericRepository<InventoryCount> genericRepository,
            IGenericRepository<Schedule> scheduleRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<Inventory> inventoryRepository,
            IGenericRepository<Batch> batchRepository,
            IGenericRepository<Warehouse> warehouseRepository,
            IGenericRepository<Account> accountRepository,
            IGenericRepository<AccountGroup> accountGroupRepository,
            IGenericRepository<InventoryCountDetail> inventoryCountDetailRepository,
            IFirebaseService firebaseService,
            ICodeGeneratorService codeGeneratorService,
            IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _batchRepository = batchRepository;
            _warehouseRepository = warehouseRepository;
            _accountRepository = accountRepository;
            _accountGroupRepository = accountGroupRepository;
            _inventoryCountDetailRepository = inventoryCountDetailRepository;
            _firebaseService = firebaseService;
            _codeGeneratorService = codeGeneratorService;
        }


        public override async Task<ServiceResponse> Get<TResult>(string id)
        {
            var inventoryCount = await _genericRepository.GetByCondition(
                p => p.Id == id,
                includeProperties: "InventoryCheckDetails,Schedule.Warehouse,InventoryCheckDetails.Inventory.Batch.Product.Unit"
            );

            if (inventoryCount == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.NotFound,
                    Message = "Phiếu kiểm kê không tồn tại!",
                    Data = id
                };
            }

            TResult result = _mapper.Map<TResult>(inventoryCount);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Lấy thành công!",
                Data = result
            };
        }

        public async Task<InventoryCountDTO> AddInventoryCount(InventoryCountCreateDTO request)
        {
            /*if (request.Date > DateOnly.FromDateTime(DateTime.Now))
                throw new Exception("Date không được ở tương lai");*/

            if (request.EndTime < request.StartTime)
                throw new Exception("Ngày kết thúc không được ở trước ngày bắt đầu");

            /*var schedule = await _scheduleRepository.GetByCondition(p => p.Id == request.ScheduleId);
            if (schedule == null)
                throw new Exception("Schedule không tồn tại");
            var existedSchedule = await _genericRepository.GetByCondition(p => p.ScheduleId == request.ScheduleId);
            if (existedSchedule != null)
                throw new Exception("Schedule này đã có phiếu kiểm kê");*/

            //========================Temporary========================
            if (request.Date < DateOnly.FromDateTime(DateTime.Now))
                throw new Exception("Không được đặt lịch ở quá khứ");

            var warehouse = await _warehouseRepository.GetByCondition(p => p.Id == request.WarehouseId);
            if (warehouse == null)
                throw new Exception("Warehouse không tồn tại");
            var scheduleCreate = new ScheduleCreateDTO
            {
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                WarehouseId = request.WarehouseId
            };
            var schedule = _mapper.Map<Schedule>(scheduleCreate);
            await _scheduleRepository.Insert(schedule);
            await _unitOfWork.SaveAsync();

            request.ScheduleId = schedule.Id;
            //========================================================

            /*var location = await _locationRepository.GetByCondition(p => p.Id == request.LocationId);
            if (location != null)
            {
                if (location.Level != 0)
                    throw new Exception("Level phải bằng 0");
            }
            else
                throw new Exception("Location không tồn tại");*/
            /*if (location == null)
                throw new Exception("Location không tồn tại");*/

            /*var product = await _productRepository.GetByCondition(p => p.Id == request.InventoryCountDetailCreateDTO.ProductId);
            if (location == null)
                throw new Exception("Product không tồn tại");*/

            var inventoryCount = _mapper.Map<InventoryCount>(request);
            inventoryCount.Code = await _codeGeneratorService.GenerateCodeAsync(Data.Enum.CodeType.PKK);
            //inventoryCount.LocationId = schedule.LocationId;
            //inventoryCount.LocationId = request.LocationId;

            await _genericRepository.Insert(inventoryCount);
            await _unitOfWork.SaveAsync();

            if (request.InventoryCountDetails != null && request.InventoryCountDetails.Any())
            {
                var inventoryCountDetails = new List<InventoryCountDetail>();


                foreach (var detail in request.InventoryCountDetails)
                {
                    var employeeAccountIds = new List<string>();
                    var account = await _accountRepository.GetByCondition(a => a.Id == detail.AccountId);
                    if (account == null)
                        throw new Exception($"Tài khoản với ID {detail.AccountId} không tồn tại");
                    var accountGroup = await _accountGroupRepository.GetByCondition(ag => ag.AccountId == detail.AccountId && ag.GroupId == "3");
                    if (accountGroup == null)
                        throw new Exception($"Tài khoản với ID {detail.AccountId} không thuộc nhân viên kho");

                    employeeAccountIds.Add(detail.AccountId);

                    var inventory = await _inventoryRepository.GetByCondition(p => p.Id == detail.InventoryId);
                    if (inventory == null)
                        throw new Exception($"Inventory với ID {detail.InventoryId} không tồn tại");

                    if (inventoryCount.Schedule.WarehouseId != inventory.WarehouseId)
                        throw new Exception("Inventory phải nằm trong đúng kho");

                    var inventoryCountDetail = _mapper.Map<InventoryCountDetail>(detail);
                    inventoryCountDetail.InventoryCountId = inventoryCount.Id;
                    //inventoryCountDetail.ExpectedQuantity = expectedQuantity;
                    inventoryCountDetail.CreatedBy = inventoryCount.CreatedBy;
                    inventoryCountDetail.ExpectedQuantity = inventory.CurrentQuantity;

                    await _inventoryCountDetailRepository.Insert(inventoryCountDetail);
                    await _unitOfWork.SaveAsync();

                    await _firebaseService.SendNotificationToUsersAsync(employeeAccountIds, $"Nhân viên được sắp xếp kiểm kê",
                                                                    $"Nhân viên vừa được sắp xếp vào vai trò kiểm kê tại kho: {inventory.Warehouse.Name} với mã phiếu {inventoryCount.Code}",
                                                                    NotificationType.INVENTORY_COUNT_ASSIGNED, request.WarehouseId);
                }


            }

            return _mapper.Map<InventoryCountDTO>(inventoryCount);
        }

        public async Task<InventoryCountDTO> UpdateInventoryCount(InventoryCountUpdateDTO request)
        {
            var existingInventoryCount = await _genericRepository.GetByCondition(
                ic => ic.Id == request.Id,
                includeProperties: "InventoryCheckDetails");

            if (existingInventoryCount == null)
                throw new Exception("Phiếu kiểm kê không tìm thấy");

            /*if (request.Status.HasValue)
                existingInventoryCount.Status = request.Status.Value;*/

            /*if (!string.IsNullOrEmpty(request.Code))
                existingInventoryCount.Code = request.Code;*/

            if (!string.IsNullOrEmpty(request.Note))
                existingInventoryCount.Note = request.Note;

            if (request.Date.HasValue)
                existingInventoryCount.Date = request.Date;

            if (request.StartTime.HasValue)
                existingInventoryCount.StartTime = request.StartTime;

            if (request.EndTime.HasValue)
                existingInventoryCount.EndTime = request.EndTime;

            //==========================Temporary========================
            var existingSchedule = await _scheduleRepository.GetByCondition(p => p.Id == existingInventoryCount.ScheduleId);
            existingSchedule.Date = request.Date;
            existingSchedule.StartTime = request.StartTime;
            existingSchedule.EndTime = request.EndTime;

            _scheduleRepository.Update(existingSchedule);
            //===========================================================

            /*if (!string.IsNullOrEmpty(request.ScheduleId))
            {
                var schedule = await _scheduleRepository.GetByCondition(s => s.Id == request.ScheduleId);
                if (schedule == null)
                    throw new Exception("Schedule not found");

                existingInventoryCount.ScheduleId = request.ScheduleId;
                //existingInventoryCount.LocationId = schedule.LocationId;
            }*/


            if (request.InventoryCountDetails != null && request.InventoryCountDetails.Any())
            {
                foreach (var detailDto in request.InventoryCountDetails)
                {
                    // Nếu có Id, cập nhật chi tiết đã tồn tại
                    if (!string.IsNullOrEmpty(detailDto.Id))
                    {
                        var existingDetail = existingInventoryCount.InventoryCheckDetails
                                                .FirstOrDefault(d => d.Id == detailDto.Id);
                        if (existingDetail == null)
                            throw new Exception($"InventoryCountDetail with ID {detailDto.Id} not found");

                        bool changeEmployee = false;
                        var oldEmployee = existingDetail.AccountId;
                        if (!string.IsNullOrEmpty(detailDto.AccountId))
                        {
                            var account = await _accountRepository.GetByCondition(a => a.Id == detailDto.AccountId);
                            if (account == null)
                                throw new Exception($"Account với Id {detailDto.AccountId} không tồn tại");
                            var accountGroup = await _accountGroupRepository.GetByCondition(ag => ag.AccountId == detailDto.AccountId && ag.GroupId == "3");
                            if (accountGroup == null)
                                throw new Exception($"Account với ID {detailDto.AccountId} không thuộc nhân viên kho");
                            existingDetail.AccountId = detailDto.AccountId;
                            changeEmployee = true;
                        }

                        if (detailDto.CountedQuantity != null)
                        {
                            existingDetail.CountedQuantity = detailDto.CountedQuantity.Value;
                            switch (existingDetail.CountedQuantity)
                            {
                                case var c when c < existingDetail.ExpectedQuantity:
                                    existingDetail.Status = InventoryCountDetailStatus.Understock;
                                    break;

                                case var c when c > existingDetail.ExpectedQuantity:
                                    existingDetail.Status = InventoryCountDetailStatus.Overstock;
                                    break;

                                case var c when c == existingDetail.ExpectedQuantity:
                                    existingDetail.Status = InventoryCountDetailStatus.Balanced;
                                    break;
                            }
                        }
                        if (!string.IsNullOrEmpty(detailDto.Note))
                            existingDetail.Note = detailDto.Note;
                        if (!string.IsNullOrEmpty(detailDto.InventoryId))
                        {
                            var inventory = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId);
                            if (inventory == null)
                                throw new Exception($"Inventory với ID {detailDto.InventoryId} không tìm thấy");
                            existingDetail.InventoryId = detailDto.InventoryId;
                            //var expectedQuantity = await SumInventoryLocationQuantityByLocationLevel0AndInventory(existingInventoryCount.LocationId, existingDetail.InventoryId);
                            detailDto.ExpectedQuantity = inventory.CurrentQuantity;
                        }
                        if (!string.IsNullOrEmpty(detailDto.ErrorTicketId))
                            existingDetail.ErrorTicketId = detailDto.ErrorTicketId;

                        if (changeEmployee)
                        {
                            var inventory = await _inventoryRepository.GetByCondition(p => p.Id == existingDetail.InventoryId,
                            includeProperties: "Warehouse");
                            if (inventory == null)
                                throw new Exception($"Inventory với ID {existingDetail.InventoryId} không tìm thấy");
                            var employeeAccountIds = new List<string>();
                            var oldEmployeeAccountIds = new List<string>();
                            employeeAccountIds.Add(detailDto.AccountId);
                            oldEmployeeAccountIds.Add(oldEmployee);
                            await _firebaseService.SendNotificationToUsersAsync(employeeAccountIds, $"Nhân viên được sắp xếp kiểm kê",
                                                                    $"Nhân viên vừa được sắp xếp vào vai trò kiểm kê tại kho: {inventory.Warehouse.Name} với mã phiếu {existingInventoryCount.Code}",
                                                                    NotificationType.INVENTORY_COUNT_ASSIGNED, existingInventoryCount.Schedule.WarehouseId);

                            await _firebaseService.SendNotificationToUsersAsync(oldEmployeeAccountIds, $"Nhân viên đã thay đổi",
                                                                    $"Nhân viên vừa bị thay đổi khỏi vị trí kiểm kê, mã phiếu {existingInventoryCount.Code}",
                                                                    NotificationType.INVENTORY_COUNT_UNASSIGNED, existingInventoryCount.Schedule.WarehouseId);
                        }
                    }
                    /*else
                    {
                        var inventory = await _inventoryRepository.GetByCondition(p => p.Id == detailDto.InventoryId);
                        if (inventory == null)
                            throw new Exception($"Inventory with ID {detailDto.InventoryId} not found");

                        var newDetail = _mapper.Map<InventoryCountDetail>(detailDto);
                        newDetail.InventoryCountId = existingInventoryCount.Id;
                        await _inventoryCountDetailRepository.Insert(newDetail);
                    }*/
                }
            }

            bool allCounted = existingInventoryCount.InventoryCheckDetails
                .All(detail => detail.Status != InventoryCountDetailStatus.Uncounted);

            if (allCounted)
            {
                //existingInventoryCount.CheckStatus = InventoryCountCheckStatus.Completed;

                /*var combinedDateTime = existingInventoryCount.Date.Value.ToDateTime(existingInventoryCount.EndTime.Value);
                if (combinedDateTime < DateTime.Now)
                {
                    existingInventoryCount.Status = InventoryCountStatus.Overdue;
                }
                else
                {
                    existingInventoryCount.Status = InventoryCountStatus.OnTime;
                }*/
                existingInventoryCount.Status = InventoryCountStatus.Completed;
                var userOfRequestedWarehouseIds = await _unitOfWork.AccountRepository.GetUserIdsByWarehouseAndGroups(existingInventoryCount.Schedule.WarehouseId, new List<string> { "Thủ kho" });
                await _firebaseService.SendNotificationToUsersAsync(userOfRequestedWarehouseIds, $"Phiếu kiểm kê đã hoàn thành",
                                                                    $"Phiếu kiểm kê với mã phiếu: {existingInventoryCount.Code} đã hoàn thành",
                                                                    NotificationType.INVENTORY_COUNT_COMPLETED, existingInventoryCount.Schedule.WarehouseId);
            }

            _genericRepository.Update(existingInventoryCount);
            await _unitOfWork.SaveAsync();

            var updatedInventoryCount = await _genericRepository.GetByCondition(
                ic => ic.Id == existingInventoryCount.Id,
                includeProperties: "InventoryCheckDetails,Schedule.Warehouse,InventoryCheckDetails.Inventory.Batch.Product"
            );

            if (updatedInventoryCount == null)
                throw new Exception("Cập nhật lỗi, phiếu kiểm kê không được tìm thấy sau khi cập nhật");

            return _mapper.Map<InventoryCountDTO>(updatedInventoryCount);

        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null, InventoryCountStatus? status = null, string? WarehouseId = null)
        {
            Expression<Func<InventoryCount, bool>> filter;
            if (status.HasValue)
            {
                filter = p =>
                (p.Status == status &&
                (string.IsNullOrEmpty(keyword) || p.Code.Contains(keyword)
                    || p.Note.Contains(keyword)
                    || p.InventoryCheckDetails.Any(d => d.Note != null && d.Note.Contains(keyword)))
                    ) &&
                    (string.IsNullOrEmpty(WarehouseId) || p.Schedule.Warehouse.Id == WarehouseId);
            }
            else
            {

                filter = p =>
                (string.IsNullOrEmpty(keyword) || p.Code.Contains(keyword)
                    || p.Note.Contains(keyword)
                    || p.InventoryCheckDetails.Any(d => d.Note != null && d.Note.Contains(keyword))
                    ) &&
                    (string.IsNullOrEmpty(WarehouseId) || p.Schedule.Warehouse.Id == WarehouseId);
            }


            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "InventoryCheckDetails,Schedule.Warehouse,InventoryCheckDetails.Inventory.Batch.Product.Unit");

            var mappedResults = _mapper.Map<List<InventoryCountDTO>>(results);
            

            foreach (var dto in mappedResults)
            {
                if (!string.IsNullOrEmpty(dto.CreatedBy))
                {
                    var account = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == dto.CreatedBy, "Profile,AccountGroups,AccountGroups.Group");
                    if (account != null)
                    {
                        dto.CreatedByAvatarUrl = account.Profile!.AvatarUrl;
                        dto.CreatedByFullName = $"{account.Profile.FirstName} {account.Profile.LastName}";
                        dto.CreatedByGroup = account.AccountGroups.FirstOrDefault()?.Group?.Name;
                    }
                }

                if (dto.InventoryCountDetails != null && dto.InventoryCountDetails.Any())
                {
                    foreach (var detailDto in dto.InventoryCountDetails)
                    {
                        if (!string.IsNullOrEmpty(detailDto.AccountId))
                        {
                            var detailAccount = await _unitOfWork.AccountRepository.GetByCondition(
                                a => a.Id == detailDto.AccountId, "Profile,AccountGroups,AccountGroups.Group");

                            if (detailAccount != null)
                            {
                                detailDto.CreatedByAvatarUrl = detailAccount.Profile!.AvatarUrl;
                                detailDto.CreatedByFullName = $"{detailAccount.Profile.FirstName} {detailAccount.Profile.LastName}";
                                detailDto.CreatedByGroup = detailAccount.AccountGroups.FirstOrDefault()?.Group?.Name;
                            }
                        }
                    }
                }
            }



            /*var mappedResultsForCountDetail = _mapper.Map<List<InventoryCountDetailDTO>>(results);

            foreach (var dto in mappedResultsForCountDetail)
            {
                if (!string.IsNullOrEmpty(dto.CreatedBy))
                {
                    var account = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == dto.CreatedBy, "Profile,AccountGroups,AccountGroups.Group");
                    if (account != null)
                    {
                        dto.CreatedByAvatarUrl = account.Profile!.AvatarUrl;
                        dto.CreatedByFullName = $"{account.Profile.FirstName} {account.Profile.LastName}";
                        dto.CreatedByGroup = account.AccountGroups.FirstOrDefault()?.Group?.Name;
                    }
                }
            }*/

            //var mappedResults = _mapper.Map<IEnumerable<TResult>>(results);

            int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? totalRecords));

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Search successful!",
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
