using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Core.Fakes;
using Kastil.Shared.Models;

namespace Kastil.Core.Services
{
    public interface IUserService
    {
        Task<User> Login(string staffCode);
        Task Logout();
        User CurrentUser { get; }
    }

    public class UserService : FakeUserService
    {
        
    }
}
