using System.Threading.Tasks;
using Api.Domain.Dtos;
using Api.Domain.Interfaces.Repository;
using Api.Domain.Interfaces.Services.Users;

namespace Api.Service.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _repository;

        public LoginService(IUserRepository repository)
        {
            this._repository = repository;
        }

        public async Task<object> FindByLogin(LoginDto login)
        {
            if(login != null && !string.IsNullOrWhiteSpace(login.Email))
                return await _repository.FindByLogin(login.Email);
            
            return null;
        }
    }
}