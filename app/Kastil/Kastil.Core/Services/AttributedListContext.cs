using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Core.Services
{
    public class AttributedListContext
    {
        public IAttributedListHandler ListHandler { get; private set; }
        public string DisasterId { get; private set; }
        public void Initialize(IAttributedListHandler handler, string disasterId)
        {
            ListHandler = handler;
            DisasterId = disasterId;
        }

        public IAttributedItemHandler CreateItemHandler(Attributed item)
        {
            return ListHandler.CreateItemHandler(item, DisasterId);
        }

        public Task<IEnumerable<Attributed>> Load()
        {
            return ListHandler.Load(DisasterId);
        }

        public string ItemType => ListHandler.ItemType;
    }
}