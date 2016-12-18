using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Utils;
using Newtonsoft.Json.Serialization;
using Attribute = Kastil.Common.Models.Attribute; 

namespace Kastil.Common.Services
{
    public abstract class AttributedItemPushService<T> : BaseService, IPushService2 where T : Attributed
    {
        private IRestServiceCaller Caller => Resolve<IRestServiceCaller>();
        private Connection Connection => Resolve<Connection>();
        private IPersistenceContextFactory PersistenceContextFactory => Resolve<IPersistenceContextFactory>();
        private IJsonSerializer Serializer => Resolve<IJsonSerializer>();


        private async Task<Dictionary<string, List<T>>> Initialize()
        {
            var context = PersistenceContextFactory.CreateFor<T>();
            var items = await Asyncer.Async(() => context.LoadAll().GroupBy(k => k.DisasterId).ToDictionary(g => g.Key, v => v.ToList()));
            return items.ToDictionary(k => k.Key, v => v.Value.Select(i => Serializer.Clone(i)).ToList());
        }

        public async Task Push(string userToken)
        {
			var itemsMap = await Initialize();
            foreach (var kvp in itemsMap)
            {
				var newItems = kvp.Value.Where(v => v.IsNew()).Select(i => Serializer.Clone(i)).ToList();
                var updatedItems = kvp.Value.Except(newItems).Select(i => Serializer.Clone(i)).ToList();

                await SaveNewItems(userToken, newItems, itemsMap, kvp.Key);
                await SaveUpdatedItems(userToken, updatedItems);
            }
        }

        private async Task SaveNewItems(string userToken, List<T> newItems, Dictionary<string, List<T>> itemsMap, 
            string disasterId)
        {
            var url = Connection.GenerateTableUrl<Disaster>();
            var headers = new Dictionary<string, string>(Connection.Headers) { { "user-token", userToken } };

            foreach (var item in newItems)
            {
                item.RevokeNewId();
                item.DisasterId = null;
            }

            var toSend = ToJson(newItems);
            var jsonResult = await Caller.Put($"{url}/{disasterId}", headers, toSend);
            await SaveResults(disasterId, jsonResult, itemsMap);
        }

        private async Task SaveUpdatedItems(string userToken, List<T> updatedItems)
        {
            var url = Connection.GenerateTableUrl(TableName);
            var headers = new Dictionary<string, string>(Connection.Headers) { { "user-token", userToken } };

            var context = PersistenceContextFactory.CreateFor<T>();
            foreach (var item in updatedItems)
            {
                foreach (var attr in item.Attributes)
                {
                    attr.RevokeNewId();    
                }

                var toSend = AttributesToJson(item.Attributes);
                var jsonResult = await Caller.Put($"{url}/{item.ObjectId}", headers, toSend);

                var savedAttributes = Serializer.ParseAsObjectArray<Attribute>(jsonResult, AttributeTagName, "objectId").ToList();
                var savedAttributeIds = new HashSet<string>(savedAttributes.Select(a => a.ObjectId));
                item.Attributes.RemoveAll(i => string.IsNullOrEmpty(i.ObjectId) || savedAttributeIds.Contains(i.ObjectId));
                item.Attributes.AddRange(savedAttributes);
                context.Save(item);
            }

        }

        protected abstract string AttributesToJson(IEnumerable<Attribute> attributes);
        protected abstract string ToJson(IEnumerable<T> contents);        
        protected abstract string TagName { get; }
        protected abstract string AttributeTagName { get; }
        protected abstract string TableName { get; }

        private async Task SaveResults(string disasterId, string jsonResult, Dictionary<string, List<T>> originalItemsMap)
        {
            var contents = Serializer.ParseAsObjectArray<T>(jsonResult, TagName, "objectId",
                a => a.DisasterId = disasterId).ToList();
            if (!contents.Any())
                return;

            var context = PersistenceContextFactory.CreateFor<T>();
            await Asyncer.Async(() => context.SaveAll(contents));

            List<T> localItems;
            if (originalItemsMap.TryGetValue(disasterId, out localItems))
            {
                var oldLocal = localItems.Where(a => a.IsNew());
                foreach (var item in oldLocal)
                {
                    context.Delete(item);
                }
            }
        }
    }

    public class AssessmentPushService : AttributedItemPushService<Assessment>
    {
        private IJsonSerializer Serializer => Resolve<IJsonSerializer>();

        protected override string AttributesToJson(IEnumerable<Attribute> attributes)
        {
            var AssessmentAttributes = attributes.ToList();
            return Serializer.Serialize(new {AssessmentAttributes});
        }

        protected override string ToJson(IEnumerable<Assessment> contents)
        {
            var assessments = contents.ToList();
            return Serializer.Serialize(new {assessments});
        }

        protected override string TagName => "assessments";
        protected override string AttributeTagName => "AssessmentAttributes";
        protected override string TableName => Assessment.BACKENDLESSCLASSNAME;
    }

    public class ShelterPushService : AttributedItemPushService<Shelter>
    {
        private IJsonSerializer Serializer => Resolve<IJsonSerializer>();

        protected override string AttributesToJson(IEnumerable<Attribute> attributes)
        {
            var shelterAttributes = attributes.ToList();
            return Serializer.Serialize(new { shelterAttributes });
        }

        protected override string ToJson(IEnumerable<Shelter> contents)
        {
            var shelters = contents.ToList();
            return Serializer.Serialize(new { shelters });
        }

        protected override string TagName => "shelters";
        protected override string AttributeTagName => "shelterAttributes";
        protected override string TableName => typeof(Shelter).Name;
    }
}