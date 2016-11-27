using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Core.Services
{
    public interface ILoginService
    {
        Task<User> Login(string login, string password);
    }
}