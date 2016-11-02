using System.Threading.Tasks;
using Kastil.Common.Models;
using Attribute = Kastil.Common.Models.Attribute;

namespace Kastil.Core.Services
{
    public interface IItemEditContext
    {
        bool IsNew { get; }
        void AddOrUpdateAttribute(Attribute attribute, string value);
        void DeleteAttribute(string attributeName);
        Attribute SelectedAttribute { get; set; }        
        Task CommitChanges();
    }
}
