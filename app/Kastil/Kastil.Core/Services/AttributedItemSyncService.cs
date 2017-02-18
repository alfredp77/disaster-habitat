using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;

namespace Kastil.Core.Services
{
    public class AttributedItemSyncService<TItem, TValuedAttribute> : ISyncService 
        where TItem : Attributed
        where TValuedAttribute : ValuedAttribute
    {
        private readonly IPushService _pushService;
        private readonly IPullService _pullService;
        private readonly IBackendlessQueryProvider _queryProvider;
        private readonly IPersistenceContextFactory _persistenceContextFactory;

        public AttributedItemSyncService(IPushService pushService, IPullService pullService,
            IBackendlessQueryProvider queryProvider, IPersistenceContextFactory persistenceContextFactory)
        {
            _pushService = pushService;
            _pullService = pullService;
            _queryProvider = queryProvider;
            _persistenceContextFactory = persistenceContextFactory;
        }

        public async Task<SyncResult> Sync(User user)
        {
            if (!await PushWith(user))
                return SyncResult.Failed($"Unable to completely push {typeof(TItem).Name}.");

            var query = _queryProvider.Where().OwnedBy(user.ObjectId).IsActive();
            if (!await PullAssessments(query))
                return SyncResult.Failed($"Unable to pull latest {typeof(TItem).Name} from server.");

            if (!await PullAttributes(query))
                return SyncResult.Failed($"Unable to pull latest {typeof(TValuedAttribute).Name} from server.");

            // remove all assessments that do not belong to this user
            RemoveOtherUsersItems(user);
            return SyncResult.Success();
        }

        private void RemoveOtherUsersItems(User user)
        {
            var itemContext = _persistenceContextFactory.CreateFor<TItem>();
            var existingItems = itemContext.LoadAll()
                .Where(a => !a.IsNew() && !user.ObjectId.Equals(a.OwnerId, StringComparison.CurrentCultureIgnoreCase));

            var attributeContext = _persistenceContextFactory.CreateFor<TValuedAttribute>();
            var existingAttributes = attributeContext.LoadAll()
                .GroupBy(a => a.ItemId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var existingItem in existingItems)
            {
                List<TValuedAttribute> attributes;
                if (existingAttributes.TryGetValue(existingItem.ObjectId, out attributes))
                {
                    foreach (var attribute in attributes)
                    {
                        attributeContext.Purge(attribute.ObjectId);
                    }
                }
                itemContext.Purge(existingItem.ObjectId);
            }
        }

        private async Task<bool> PushWith(User user)
        {
            var savedItems = await _pushService.Push<TItem>(user.Token);
			if (savedItems.FailedItems.Any())
				return false;
			
            var itemIds = new HashSet<string>(savedItems.SuccessfulItems.Select(savedItems.GetLocalId));
            var attributePushResult = await _pushService.Push<TValuedAttribute>(user.Token,
                a => itemIds.Contains(a.ItemId));

			return !attributePushResult.FailedItems.Any();
        }

        private async Task<bool> PullAttributes(IQuery query)
        {
            var pulledAttributesResult = await _pullService.Pull<TValuedAttribute>(query);
            return !pulledAttributesResult.FailedItems.Any();
        }

        private async Task<bool> PullAssessments(IQuery query)
        {
            var pulledAssessmentsResult = await _pullService.Pull<TItem>(query);
            return !pulledAssessmentsResult.FailedItems.Any();
        }
    }
}