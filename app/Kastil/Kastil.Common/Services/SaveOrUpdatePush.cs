using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Utils;

namespace Kastil.Common.Services
{
    public class SaveOrUpdatePushService : BaseService, IPushService
    {
        private IRestServiceCaller Caller => Resolve<IRestServiceCaller>();
        private IPersistenceContextFactory PersistenceContextFactory => Resolve<IPersistenceContextFactory>();
        private IJsonSerializer Serializer => Resolve<IJsonSerializer>();
        private Connection Connection => Resolve<Connection>();
        private IBackendlessResponseParser ResponseParser => Resolve<IBackendlessResponseParser>();

        public async Task<SyncResult<T>>  Push<T>(string userToken, Predicate<T> criteria=null) where T : BaseModel
        {            
            var headers = new Dictionary<string, string>(Connection.Headers) { { "user-token", userToken } };
            criteria = criteria ?? (a => true);

            var context = PersistenceContextFactory.CreateFor<T>();
            var items = await Asyncer.Async(() => context.LoadAll());
            var result = new SyncResult<T>();
            foreach (var item in items.Where(i => criteria(i)))
            {                
                var json = await PushItem(item, headers);
                var parsed = ResponseParser.Parse<T>(json);
                if (parsed.IsSuccessful)
                {
                    context.Save(parsed.Content);
                    context.Purge(item.ObjectId);
                    result.Success(parsed.Content, item.ObjectId);
                }
                else
                {
                    result.Failed(item, parsed.ToString());
                }
            }
            return result;
        }

        private async Task<string> PushItem<T>(T item, Dictionary<string, string> headers) where T : BaseModel
        {            
            var payload = Serializer.Serialize(PrepareItem(item));
			Task<string> pushTask;
			if (item.IsNew()) 
			{
				pushTask = Caller.Post(Connection.GenerateTableUrl<T>(), headers, payload);
			} 
			else 
			{
				pushTask = Caller.Put(Connection.GenerateTableUrl<T>(item.ObjectId), headers, payload);
			}

            return await pushTask;
        }

        private T PrepareItem<T>(T item) where T : BaseModel
        {
            var cloned = Serializer.Clone(item);
            cloned.ObjectId = null;
            cloned.OwnerId = null;
            return cloned;
        }
    }

    public class SyncResult<T> where T : BaseModel
    {
        private readonly Dictionary<string, string> _errorMessages = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _localIds = new Dictionary<string, string>();
        private readonly List<T> _successfulItems = new List<T>();
        private readonly List<T> _failedItems = new List<T>();

        public void Failed(T item, string errorMessage)
        {
            _failedItems.Add(item);
            _errorMessages[item.ObjectId] = errorMessage;
        }

        public void Success(T item, string localId)
        {
            _successfulItems.Add(item);
            _localIds[item.ObjectId] = localId;
        }

        public IEnumerable<T> SuccessfulItems => _successfulItems;
        public IEnumerable<T> FailedItems => _failedItems;

        public string GetLocalId(T item)
        {
            string localId;
            _localIds.TryGetValue(item.ObjectId, out localId);
            return localId;
        }

        public string GetErrorMessage(T item)
        {
            string errorMessage;
            _errorMessages.TryGetValue(item.ObjectId, out errorMessage);
            return errorMessage;
        }
    }


    public static class SyncResultExtensions
    {
        public static SyncResult<T> Merge<T>(this SyncResult<T> me, params SyncResult<T>[] other) where T : BaseModel
        {
            var all = new List<SyncResult<T>> {me};
            all.AddRange(other);

            var merged = new SyncResult<T>();
            foreach (var syncResult in all)
            {
                foreach (var item in syncResult.SuccessfulItems)
                {
                    merged.Success(item, syncResult.GetLocalId(item));
                }

                foreach (var item in syncResult.FailedItems)
                {
                    merged.Failed(item, syncResult.GetErrorMessage(item));
                }
            }
            return merged;
        }
    }

}