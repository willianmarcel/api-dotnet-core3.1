using System.Threading.Tasks;
using Api.Domain.Dtos;

namespace Api.Domain.Interfaces.Services.Users
{
    public interface ILoginService
    {
         Task<object> FindByLogin(LoginDto login);
    }
}