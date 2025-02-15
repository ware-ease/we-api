using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models.AccountGroup;
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
    }
}