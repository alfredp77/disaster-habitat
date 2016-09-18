using System.Collections.Generic;
using System.Linq;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using MvvX.Plugins.CouchBaseLite.Database;

namespace Kastil.Core.Services
{
    public class PersistenceContext<T> : IPersistenceContext<T> where T : BaseModel
    {
        public IDatabase Db { get; }
        public IJsonSerializer Serializer { get; }

        public PersistenceContext(IDatabase db, IJsonSerializer serializer)
        {
            Db = db;
            Serializer = serializer;
        }

        private Dictionary<string, object> Convert(T document)
        {
            var json = Serializer.Serialize(document);
            var values = Serializer.Deserialize<Dictionary<string, object>>(json);
            return values;
        }

        public void Save(T document)
        {
            Db.PutLocalDocument(document.Id, Convert(document));
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
            var query = Db.CreateAllDocumentsQuery();
            
            var queryResult = query.Run();

            return queryResult.Select(row => Serializer.Serialize(row.DocumentProperties))
                .Select(json => Serializer.Deserialize<T>(json))
                .ToList();
        }

        public void DeleteAll()
        {
            Db.Delete();
        }

        public void PersistJson(string id, string json)
        {
            var values = Serializer.Deserialize<Dictionary<string, object>>(json);
            Db.PutLocalDocument(id, values);
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