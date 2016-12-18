using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Core.Services
{
    public interface IAttributedItemHandler
    {
        Attributed CurrentItem { get; }
        Task CommitChanges();
        string NamePlaceholderText { get; }
        string LocationPlaceholderText { get; }
        string ItemType { get; }
		Attribute CreateAttributeFrom(Attribute source);
    }
}
