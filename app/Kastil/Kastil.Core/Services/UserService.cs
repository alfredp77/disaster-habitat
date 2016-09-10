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
        Task Logout();
        User CurrentUser { get; }
    }

    public class UserService : IUserService
    {
        public Task<User> Login(string staffCode)
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }

        public User CurrentUser { get; }
    }

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
