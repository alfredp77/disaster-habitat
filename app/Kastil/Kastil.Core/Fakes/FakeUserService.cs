using System.Threading.Tasks;
using Kastil.Core.Services;
using Kastil.Shared.Models;

namespace Kastil.Core.Fakes
{
    public class FakeUserService : IUserService
    {
        public Task<User> Login(string staffCode)
        {
            if (staffCode == "x")
                return Task.FromResult<User>(null);

            var user = new User {StaffCode = staffCode, Id = staffCode, FirstName = "Test", LastName = "User"};
            CurrentUser = user;
            return Task.FromResult(user);
        }

        public Task Logout()
        {
            CurrentUser = null;
            return Task.Delay(0);
        }

        public User CurrentUser { get; private set; }
    }
}