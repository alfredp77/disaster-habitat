using Kastil.Core.Utils;
using Kastil.Shared.Models;
using MvvX.Plugins.CouchBaseLite;
using MvvX.Plugins.CouchBaseLite.Database;

namespace Kastil.Core.Services
{
    public class PersistenceContextFactory : BaseService, IPersistenceContextFactory
    {
        public const string DATA_PATH = "offline_data";

        private IJsonSerializer Serializer => Resolve<IJsonSerializer>();

        private IDatabase GetStorage<T>() where T : BaseModel
        {
            var cbLite = Resolve<ICouchBaseLite>();
            var options = cbLite.CreateDatabaseOptions();
            return cbLite.CreateConnection(DATA_PATH, $"db_{typeof(T).Name}".ToLowerInvariant(), options);
        }

        public IPersistenceContext<T> CreateFor<T>() where T : BaseModel
        {
            return new PersistenceContext<T>(GetStorage<T>(), Serializer);
        }

    }
}