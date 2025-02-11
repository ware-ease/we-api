using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.Authentication;
using Data.Entity;
using DataAccessLayer.UnitOfWork;
using System.Linq.Expressions;

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

        public async Task<IEnumerable<Account>> GetAccountsAsync()
        {
            return await _unitOfWork.AccountRepository.GetAccountsAsync();
        }

        //public async Task<bool> checkCorrectPassword(int id, string password)
        //{
        //    Expression<Func<AppUser, bool>> filter = x => x.Password.Equals(PasswordHelper.ConvertToEncrypt(password)) && x.UserId == id;
        //    var rightPass = await _unitOfWork.AppUserRepository.GetByCondition(filter);
        //    if (rightPass == null)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        public async Task<AccountDTO?> CheckLoginAsync(string userName, string password)
        {
            var user = await _unitOfWork.AccountRepository
                .CheckLoginAsync(userName, password);

            var userDTO = _mapper.Map<AccountDTO>(user);
            return userDTO;
        }

        public async Task<TokenDTO> GenerateAccessTokenAsync(string id)
        {
            var token = await _unitOfWork.AccountRepository.GenerateAccessTokenAsync(id);
            var tokentDTO = _mapper.Map<TokenDTO>(token);
            return tokentDTO;
        }
    }
}
