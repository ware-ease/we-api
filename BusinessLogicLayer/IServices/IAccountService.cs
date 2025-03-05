using BusinessLogicLayer.Generic;
using BusinessLogicLayer.Models.AccountAction;
using BusinessLogicLayer.Models.AccountGroup;
using BusinessLogicLayer.Models.Authentication;
using BusinessLogicLayer.Models.Pagination;
using Data.Model.DTO;
using Data.Model.Request.Account;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IAccountService : IGenericService
    {
        Task<AccountDTO?> CheckLoginAsync(string userName, string password);
        Task<TokenDTO> GenerateAccessTokenAsync(string id);
        Task<ServiceResponse> Add(BusinessLogicLayer.Models.Account.AccountCreateDTO request);
    }
}
