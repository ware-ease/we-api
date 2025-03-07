using BusinessLogicLayer.Generic;
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
        Task<ServiceResponse> Add(AccountCreateDTO request);
        Task<ServiceResponse> ChangePassword(string id, string password);
    }
}
