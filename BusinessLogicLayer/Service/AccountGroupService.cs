using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.AccountGroup;
using BusinessLogicLayer.Models.Group;
using Data.Entity;
using DataAccessLayer.UnitOfWork;

namespace BusinessLogicLayer.Service
{
    public class AccountGroupService : IAccountGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public AccountGroupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountGroupDTO>> GetAllAsync()
        {
            var data = await _unitOfWork.AccountGroupRepository.Get();
            return _mapper.Map<IEnumerable<AccountGroupDTO>>(data);
        }

        public async Task<AccountGroupDTO> GetByIdAsync(string accountId, string groupId)
        {
            var entity = await _unitOfWork.AccountGroupRepository.GetByCondition(
                filter: x => x.AccountId == accountId && x.GroupId == groupId);
            return _mapper.Map<AccountGroupDTO>(entity);
        }

        public async Task<AccountGroupDTO> CreateAsync(CreateAccountGroupDTO model)
        {
            // Kiểm tra xem quyền này đã được cấp cho account hay chưa
            var existingEntity = await _unitOfWork.AccountGroupRepository.GetByCondition(
                filter: x => x.AccountId == model.AccountId && x.GroupId == model.GroupId);

            if (existingEntity != null)
            {
                throw new InvalidOperationException("Nhóm này đã được thêm cho tài khoản này.");
            }

            var entity = _mapper.Map<AccountGroup>(model);
            entity.CreatedTime = DateTime.Now;
            await _unitOfWork.AccountGroupRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AccountGroupDTO>(entity);
        }

        public async Task<bool> DeleteAsync(string accountId, string groupId)
        {
            var entity = await _unitOfWork.AccountGroupRepository.GetByCondition(
                filter: x => x.AccountId == accountId && x.GroupId == groupId);

            if (entity == null) return false;

            _unitOfWork.AccountGroupRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<GroupDTO>> GetGroupsByAccountIdAsync(string accountId)
        {
            var accountGroups = await _unitOfWork.AccountGroupRepository.Get(x => x.AccountId == accountId);
            var groupIds = accountGroups.Select(x => x.GroupId).ToList();

            var groups = await _unitOfWork.GroupRepository.Get(g => groupIds.Contains(g.Id));
            return _mapper.Map<IEnumerable<GroupDTO>>(groups);
        }

        public async Task<IEnumerable<AccountDTO>> GetAccountsByGroupIdAsync(string groupId)
        {
            var accountGroups = await _unitOfWork.AccountGroupRepository.Get(x => x.GroupId == groupId);
            var accountIds = accountGroups.Select(x => x.AccountId).ToList();

            var accounts = await _unitOfWork.AccountRepository.Get(a => accountIds.Contains(a.Id));
            return _mapper.Map<IEnumerable<AccountDTO>>(accounts);
        }
    }
}