using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.AccountWarehouse;
using BusinessLogicLayer.Models.Warehouse;
using Data.Entity;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class AccountWarehouseService : IAccountWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public AccountWarehouseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountWarehouseDTO>> GetAllAsync()
        {
            var data = await _unitOfWork.AccountWarehouseRepository.Get();
            return _mapper.Map<IEnumerable<AccountWarehouseDTO>>(data);
        }

        public async Task<AccountWarehouseDTO> GetByIdAsync(string accountId, string warehouseId)
        {
            var entity = await _unitOfWork.AccountWarehouseRepository.GetByCondition(
                x => x.AccountId == accountId && x.WarehouseId == warehouseId);
            return _mapper.Map<AccountWarehouseDTO>(entity);
        }

        public async Task<AccountWarehouseDTO> CreateAsync(CreateAccountWarehouseDTO model)
        {
            var existingEntity = await _unitOfWork.AccountWarehouseRepository.GetByCondition(
                x => x.AccountId == model.AccountId && x.WarehouseId == model.WarehouseId);

            if (existingEntity != null)
            {
                throw new InvalidOperationException("Tài khoản này đã được liên kết với kho này.");
            }

            var entity = _mapper.Map<AccountWarehouse>(model);
            entity.JoinedDate = DateTime.Now;
            entity.Status = true;
            entity.CreatedTime = DateTime.Now;

            await _unitOfWork.AccountWarehouseRepository.Insert(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<AccountWarehouseDTO>(entity);
        }

        public async Task<bool> DeleteAsync(DeleteAccountWarehouseDTO model)
        {
            var entity = await _unitOfWork.AccountWarehouseRepository.GetByCondition(
                x => x.AccountId == model.AccountId && x.WarehouseId == model.WarehouseId);

            if (entity == null) return false;
            //entity.LeftDate = DateTime.Now;
            //entity.Status = false;
            entity.DeletedTime = DateTime.Now;
            entity.DeletedBy = model.DeletedBy;
            entity.IsDeleted = true;

            _unitOfWork.AccountWarehouseRepository.Update(entity);
            //_unitOfWork.AccountWarehouseRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<WarehouseDTO>> GetWarehousesByAccountIdAsync(string accountId)
        {
            var accountWarehouses = await _unitOfWork.AccountWarehouseRepository.Get(x => x.AccountId == accountId);
            var warehouseIds = accountWarehouses.Select(x => x.WarehouseId).ToList();

            var warehouses = await _unitOfWork.WarehouseRepository.Get(w => warehouseIds.Contains(w.Id));
            return _mapper.Map<IEnumerable<WarehouseDTO>>(warehouses);
        }

        public async Task<IEnumerable<AccountDTO>> GetAccountsByWarehouseIdAsync(string warehouseId)
        {
            var accountWarehouses = await _unitOfWork.AccountWarehouseRepository.Get(x => x.WarehouseId == warehouseId);
            var accountIds = accountWarehouses.Select(x => x.AccountId).ToList();

            var accounts = await _unitOfWork.AccountRepository.Get(a => accountIds.Contains(a.Id));
            return _mapper.Map<IEnumerable<AccountDTO>>(accounts);
        }
        public async Task<AccountWarehouseDTO> UpdateAsync(string accountId, string warehouseId, UpdateAccountWarehouseDTO model)
        {
            var entity = await _unitOfWork.AccountWarehouseRepository.GetByCondition(
                x => x.AccountId == accountId && x.WarehouseId == warehouseId);

            if (entity == null)
            {
                throw new KeyNotFoundException("Không tìm thấy dữ liệu để cập nhật.");
            }

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = DateTime.Now;
            entity.LastUpdatedBy = model.LastUpdatedBy;

            _unitOfWork.AccountWarehouseRepository.Update(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<AccountWarehouseDTO>(entity);
        }

    }
}
