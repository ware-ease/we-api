using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.AccountPermission;
using BusinessLogicLayer.Models.Permission;
using Data.Entity;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class AccountPermissionService : IAccountPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public AccountPermissionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<AccountPermissionDTO>> GetAllAsync()
        {
            var data = await _unitOfWork.AccountPermissionRepository.Get();
            return _mapper.Map<IEnumerable<AccountPermissionDTO>>(data);
        }

        public async Task<AccountPermissionDTO> GetByIdAsync(string accountId, string permissionId)
        {
            //var entity = await _unitOfWork.AccountPermissionRepository.GetByCondition(
            //    filter: x => x.AccountId == accountId && x.PermissionId == permissionId);
            //return _mapper.Map<AccountPermissionDTO>(entity);

            return new AccountPermissionDTO();
        }

        public async Task<AccountPermissionDTO> CreateAsync(CreateAccountPermissionDTO model)
        {
            // Kiểm tra xem quyền này đã được cấp cho account hay chưa
            //var existingEntity = await _unitOfWork.AccountPermissionRepository.GetByCondition(
            //    filter: x => x.AccountId == model.AccountId && x.PermissionId == model.PermissionId);

            //if (existingEntity != null)
            //{
            //    throw new InvalidOperationException("Quyền này đã được cấp cho tài khoản này.");
            //}

            var entity = _mapper.Map<AccountAction>(model);
            entity.CreatedTime = DateTime.Now;
            await _unitOfWork.AccountPermissionRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AccountPermissionDTO>(entity);
        }

        public async Task<bool> DeleteAsync(string accountId, string permissionId)
        {
            //var entity = await _unitOfWork.AccountPermissionRepository.GetByCondition(
            //    filter: x => x.AccountId == accountId && x.PermissionId == permissionId);

            //if (entity == null) return false;

            //_unitOfWork.AccountPermissionRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<PermissionDTO>> GetPermissionsByAccountIdAsync(string accountId)
        {
            //var accountPermissions = await _unitOfWork.AccountPermissionRepository.Get(x => x.AccountId == accountId);
            //var permissionIds = accountPermissions.Select(x => x.PermissionId).ToList();

            //var permissions = await _unitOfWork.PermissionRepository.Get(p => permissionIds.Contains(p.Id));
            //return _mapper.Map<IEnumerable<PermissionDTO>>(permissions);

            return null;
        }

        public async Task<IEnumerable<AccountDTO>> GetAccountsByPermissionIdAsync(string permissionId)
        {
            //var accountPermissions = await _unitOfWork.AccountPermissionRepository.Get(x => x.PermissionId == permissionId);
            //var accountIds = accountPermissions.Select(x => x.AccountId).ToList();

            //var accounts = await _unitOfWork.AccountRepository.Get(a => accountIds.Contains(a.Id));
            //return _mapper.Map<IEnumerable<AccountDTO>>(accounts);

            return null;
        }
    }
}
