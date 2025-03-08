using AutoMapper;
using Azure.Core;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Utils;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Response;
using DataAccessLayer.UnitOfWork;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse> Login(string username, string password)
        {
            var user = await _unitOfWork.AccountRepository
                .GetByCondition(a => a.Username.ToLower() == username.ToLower() && a.Password == PasswordHelper.ConvertToEncrypt(password));

            if (user == null)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Unauthorized,
                    Message = "Username or password is incorrect!",
                    Data = { }
                };
            }

            var result = _mapper.Map<AccountDTO>(user);
            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Login successfully!",
                Data = user.Id,
            };
        }

        public async Task<ServiceResponse> Logout(TokenDTO tokens)
        {
            var refreshToken = await _unitOfWork.RefreshTokenRepository.GetByCondition(r => r.Token == tokens.refreshToken);

            if (refreshToken != null)
            {
                _unitOfWork.RefreshTokenRepository.DeletePermanently(refreshToken);
                await _unitOfWork.SaveAsync();
            }

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Logout successfully!",
                Data = { }
            };
        }

        public async Task<TokenDTO> GenerateTokens(string id)
        {
            var accessToken = GenerateAccessToken(id);
            var refreshToken = GenerateRefreshToken();

            var account = await _unitOfWork.AccountRepository.GetByCondition(a => a.Id == id);

            if (account != null)
            {
                await _unitOfWork.RefreshTokenRepository.Add(new RefreshToken
                {
                    Id = Guid.NewGuid().ToString(),
                    Account = account,
                    AccountId = account.Id,
                    Token = refreshToken,
                    ExpiryTime = DateTime.UtcNow.AddDays(2),
                });

                await _unitOfWork.SaveAsync();
            }

            return new TokenDTO
            {
                accessToken = accessToken,
                refreshToken = refreshToken,
            };
        }

        private string GenerateAccessToken(string userId)
        {
            Env.Load();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var authClaims = new List<Claim>
            {
                new Claim("uid", userId),
            };

            var token = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_VALID_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE"),
                expires: DateTime.UtcNow.AddMinutes(30),
                claims: authClaims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<bool> CheckRefreshTokenExpiry(string? refreshToken)
        {
            var savedToken = await _unitOfWork.RefreshTokenRepository.GetByCondition(t => t.Token == refreshToken);

            if (savedToken != null)
            {
                if (savedToken.ExpiryTime >= DateTime.UtcNow)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
