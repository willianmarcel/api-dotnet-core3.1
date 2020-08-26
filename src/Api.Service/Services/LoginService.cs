using System;
using System.Security.Principal;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Domain.Dtos;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Repository;
using Api.Domain.Interfaces.Services.Users;
using Api.Domain.Security;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Api.Service.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _repository;
        private SigningConfigurations _signingConfigurations;
        private TokenConfigurations _tokenConfigurations;
        private IConfiguration _configuration{ get; }

        public LoginService(
            IUserRepository repository, 
            SigningConfigurations signingConfigurations, 
            TokenConfigurations tokenConfigurations,
            IConfiguration configuration)
        {
            this._repository = repository;
            this._signingConfigurations = signingConfigurations;
            this._tokenConfigurations = tokenConfigurations;
            this._configuration = configuration;
        }

        public async Task<object> FindByLogin(LoginDto login)
        {
            var baseUser = new UserEntity();

            if(login != null && !string.IsNullOrWhiteSpace(login.Email))
            {
                baseUser = await _repository.FindByLogin(login.Email);

                if(baseUser == null)
                    return new { 
                        authenticated = false,
                        message = "Falha ao autenticar"
                    };
                
                var identity = new ClaimsIdentity(
                    new GenericIdentity(baseUser.Email),
                    new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, login.Email),
                    }
                );

                var createDate = DateTime.Now;
                var expirationDate = createDate + TimeSpan.FromSeconds(_tokenConfigurations.Seconds);

                var handler = new JwtSecurityTokenHandler();
                var token = CreateToken(identity, createDate, expirationDate, handler);

                return SuccessObject(createDate, expirationDate, token, login);
            }
                
            
            return null;
        }

        private string CreateToken(ClaimsIdentity identity, DateTime createDate, DateTime expirationDate, JwtSecurityTokenHandler handler)
        {
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = createDate,
                Expires = expirationDate,
            });

            var token = handler.WriteToken(securityToken);
            return token;
        }

        private object SuccessObject(DateTime createDate, DateTime expirationDate, string token, LoginDto user)
        {
            return new {
                authenticated = true,
                created = createDate.ToString("yyyy-MM-dd HH:mm:ss"),
                expiration = expirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                accessToken = token,
                userName = user.Email,
                message = "Usuário logado com sucesso"
            };
        }
    }
}