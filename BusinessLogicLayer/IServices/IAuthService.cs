using Data.Model.DTO;
using Data.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IAuthService
    {
        Task<ServiceResponse> Login(string username, string password);
        Task<ServiceResponse> Logout(TokenDTO tokens);
        Task<TokenDTO> GenerateTokens(string id);
        Task<bool> CheckRefreshTokenExpiry(string? refreshToken);
    }
}
