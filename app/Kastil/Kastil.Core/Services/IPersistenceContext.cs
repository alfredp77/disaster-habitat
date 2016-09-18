using System.Collections.Generic;
using Kastil.Shared.Models;

namespace Kastil.Core.Services
{
    public interface IPersistenceContext<T> where T : BaseModel
    {
        void Save(T document);
        void SaveAll(IEnumerable<T> documents);
        IEnumerable<T> LoadAll();
        void DeleteAll();
        void PersistJson(string id, string json);
        void PersistAllJson(IEnumerable<KeyValuePair<string, string>> kvps);
    }
}