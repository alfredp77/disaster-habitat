using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kinvey;
using User = Kinvey.User;

namespace Kastil.Common.Utils
{
    public static class KinveyHelpers
    {
        public static string CollectionName<T>() where T : BaseModel
        {
            return typeof(T).Name.ToLower();
        }

        public static async Task<User> EnsureLogin()
        {
            if (Client.SharedClient.IsUserLoggedIn())
                return Client.SharedClient.ActiveUser;

            return await User.LoginAsync();
        }

        public static DataStore<T> GetDataStore<T>() where T : BaseModel
        {
            return DataStore<T>.Collection(CollectionName<T>(), DataStoreType.SYNC);
        }
    }
}
