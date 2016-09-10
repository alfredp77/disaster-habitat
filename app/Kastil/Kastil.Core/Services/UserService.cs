using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Shared.Models;

namespace Kastil.Core.Services
{
    public interface IUserService
    {
        Task<User> Login(string staffCode);
    }

    public class UserService : IUserService
    {
        public Task<User> Login(string staffCode)
        {
            throw new NotImplementedException();
        }
    }
}
