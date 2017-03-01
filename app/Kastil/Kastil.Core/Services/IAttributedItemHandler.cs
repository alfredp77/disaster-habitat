using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Core.Services
{
    public interface IAttributedItemHandler
    {
        Attributed CurrentItem { get; }
        Task CommitChanges(IEnumerable<ValuedAttribute> modifiedAttributes, IEnumerable<ValuedAttribute> deletedAttributes);
        string NamePlaceholderText { get; }
        string LocationPlaceholderText { get; }
        string ItemType { get; }
		ValuedAttribute CreateAttributeFrom(Attribute source);
        Task<IEnumerable<ValuedAttribute>> GetAttributes();
        IEnumerable<Attribute> FilterAvailableAttributes(List<Attribute> availableAttributes);
    }
}
