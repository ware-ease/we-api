using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.Authentication;
using Data.Entity;
using DataAccessLayer.UnitOfWork;
using System.Linq.Expressions;
using Profile = Data.Entity.Profile;

namespace BusinessLogicLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountDTO>> GetAccountsAsync()
        {
            var accounts = await _unitOfWork.AccountRepository.GetAccountsAsync();
            return _mapper.Map<IEnumerable<AccountDTO>>(accounts);
        }

        public async Task<AccountDTO?> CheckLoginAsync(string userName, string password)
        {
            var user = await _unitOfWork.AccountRepository
                .CheckLoginAsync(userName, password);
            return _mapper.Map<AccountDTO>(user);
        }

        public async Task<TokenDTO> GenerateAccessTokenAsync(string id)
        {
            var token = await _unitOfWork.AccountRepository.GenerateAccessTokenAsync(id);
            var tokentDTO = _mapper.Map<TokenDTO>(token);
            return tokentDTO;
        }
        public async Task<AccountDTO?> GetAccountByIdAsync(string id)
        {
            var account = await _unitOfWork.AccountRepository.GetByID(id);
            return _mapper.Map<AccountDTO>(account);
        }

        public async Task<AccountDTO> CreateAccountAsync(AccountCreateDTO model)
        {
            var account = _mapper.Map<Account>(model);
            account.CreatedTime = DateTime.Now;
            //Data.Entity.Profile profile = new Profile();
            //profile.AccountId = account.Id;
            await _unitOfWork.AccountRepository.Insert(account);
            //await _unitOfWork.ProfileRepository.Insert(profile);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AccountDTO>(account);
        }

        public async Task<AccountDTO?> UpdateAccountAsync(string id, AccountUpdateDTO model)
        {
            var account = await _unitOfWork.AccountRepository.GetByID(id);
            if (account == null) return null;

            _mapper.Map(model, account); // Cập nhật dữ liệu từ DTO vào entity
            _unitOfWork.AccountRepository.Update(account);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AccountDTO>(account);
        }

        public async Task<bool> DeleteAccountAsync(string id)
        {
            var account = await _unitOfWork.AccountRepository.GetByID(id);
            if (account == null) return false;
            account.IsDeleted = true;
            _unitOfWork.AccountRepository.Update(account);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
