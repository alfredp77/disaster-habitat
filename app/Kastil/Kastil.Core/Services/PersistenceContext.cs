using System.Collections.Generic;
using System.Linq;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using MvvX.Plugins.CouchBaseLite.Database;

namespace Kastil.Core.Services
{
    public class PersistenceContext<T> : IPersistenceContext<T> where T : BaseModel
    {
        private readonly IDatabase _db;
        private readonly IJsonSerializer _serializer;

        public PersistenceContext(IDatabase db, IJsonSerializer serializer)
        {
            _db = db;
            _serializer = serializer;
        }

        private Dictionary<string, object> Convert(T document)
        {
            var json = _serializer.Serialize(document);
            var values = _serializer.Deserialize<Dictionary<string, object>>(json);
            return values;
        }

        public void Save(T document)
        {
            _db.PutLocalDocument(document.Id, Convert(document));
        }

        public void SaveAll(IEnumerable<T> documents)
        {
            foreach (var document in documents)
            {
                Save(document);
            }
        }
        
        public IEnumerable<T> LoadAll()
        {
            var query = _db.CreateAllDocumentsQuery();
            
            var queryResult = query.Run();

            return queryResult.Select(row => _serializer.Serialize(row.DocumentProperties))
                .Select(json => _serializer.Deserialize<T>(json))
                .ToList();
        }

        public void DeleteAll()
        {
            _db.Delete();
        }

        public void PersistJson(string id, string json)
        {
            var values = _serializer.Deserialize<Dictionary<string, object>>(json);
            _db.PutLocalDocument(id, values);
        }

        public void PersistAllJson(IEnumerable<KeyValuePair<string, string>> kvps)
        {
            foreach (var kvp in kvps)
            {
                PersistJson(kvp.Key, kvp.Value);                
            }
        }
    }
}