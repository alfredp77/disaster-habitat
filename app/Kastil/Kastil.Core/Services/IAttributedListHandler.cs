using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Core.Services
{
    public interface IAttributedListHandler
    {
        IAttributedItemHandler CreateItemHandler(Attributed item, string disasterId);
        Task<IEnumerable<Attributed>> Load();
        string ItemType { get; }
    }
}