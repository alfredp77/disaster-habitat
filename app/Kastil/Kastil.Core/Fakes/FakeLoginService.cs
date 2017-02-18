using System;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Core.Services;

namespace Kastil.Core.Fakes
{
    public class FakeLoginService : ILoginService
    {
        public Task<User> Login(string login, string password)
        {
            return
                Task.FromResult(new User
                {
                    ObjectId = Guid.NewGuid().ToString(),
                    Email = login,
                    Token = Guid.NewGuid().ToString()
                });
        }
    }
}