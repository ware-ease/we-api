using AutoMapper;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Utils;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Response;
using DataAccessLayer.UnitOfWork;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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

            if (user.Status == Data.Enum.AccountStatus.Locked)
            {
                return new ServiceResponse
                {
                    Status = Data.Enum.SRStatus.Unauthorized,
                    Message = "Account is locked!",
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
        public async Task<bool> HasPermissionAsync(string userId, string permissionKey)
        {
            var permission = await _unitOfWork.PermissionRepository
                .GetByCondition(p => p.Code == permissionKey);

            if (permission == null)
                return false;

            // Check trực tiếp permission gán cho account
            var userPermissions = await _unitOfWork.AccountPermissionRepository
                .GetByCondition(up => up.Id == userId && up.PermissionId == permission.Id);

            if (userPermissions != null)
                return true;

            // Lấy tất cả group của account
            var accountGroups = await _unitOfWork.AccountGroupRepository
                .Search(ag => ag.AccountId == userId);

            if (!accountGroups.Any())
                return false;

            var groupIds = accountGroups.Select(ag => ag.GroupId).ToList();

            // Check permission của các group đó
            var groupPermissions = await _unitOfWork.GroupPermissionRepository
                .Search(gp => groupIds.Contains(gp.GroupId) && gp.PermissionId == permission.Id);

            return groupPermissions.Any();
        }
    }
}
