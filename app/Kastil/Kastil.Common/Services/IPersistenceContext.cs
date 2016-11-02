using System.Collections.Generic;
using Kastil.Common.Models;

namespace Kastil.Common.Services
{
    public interface IPersistenceContext<T> where T : BaseModel
    {
        void Save(T document);
        void SaveAll(IEnumerable<T> documents);
        IEnumerable<T> LoadAll();
        void DeleteAll();
        void PersistJson(string id, string json);
        void PersistAllJson(IEnumerable<KeyValuePair<string, string>> kvps);
        void Delete(T document);
    }
}