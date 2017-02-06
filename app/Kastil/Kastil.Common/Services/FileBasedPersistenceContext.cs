using System;
using System.Collections.Generic;
using System.Linq;
using Kastil.Common.Utils;
using Kastil.Common.Models;
using MvvmCross.Plugins.File;

namespace Kastil.Common.Services
{
    public class FileBasedPersistenceContext<T> : IPersistenceContext<T> where T : BaseModel
    {
        public string DataFolder { get; }
        public IJsonSerializer Serializer { get; }
        public IMvxFileStore FileStore { get; }

        public FileBasedPersistenceContext(string dataFolder, IJsonSerializer serializer, IMvxFileStore fileStore)
        {
            DataFolder = dataFolder;
            Serializer = serializer;
            FileStore = fileStore;
        }

        public void Save(T document)
        {
            var id = document.ObjectId;
            var path = GetPath(id);
            FileStore.WriteFile(path, Serializer.Serialize(document));
        }

        private string GetPath(string id)
        {
            return FileStore.PathCombine(DataFolder, $"{id.ToLowerInvariant()}.json");
        }

        private string GetDeletedFolder()
        {
            var deletedFolder = FileStore.PathCombine(DataFolder, "deleted");
            FileStore.EnsureFolderExists(deletedFolder);
            return deletedFolder;
        }

        private string GetDeletedPath(string id)
        {
            return FileStore.PathCombine(GetDeletedFolder(), $"{id.ToLowerInvariant()}.json");
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
            var files = FileStore.GetFilesIn(DataFolder);
			foreach (var file in files.Where(f => f.EndsWith(".json")))
            {
                string contents;
                if (FileStore.TryReadTextFile(file, out contents))
                {
                    yield return Serializer.Deserialize<T>(contents);
                }
            }
        }

        public void PurgeAll()
        {
            FileStore.DeleteFolder(DataFolder, true);
            FileStore.EnsureFolderExists(DataFolder);
        }

        public void MarkDeleted(T document)
        {
            var filePath = GetPath(document.ObjectId);
            var deleted = GetDeletedPath(document.ObjectId);
            FileStore.TryMove(filePath, deleted, true);
        }

        public void Purge(string id)
        {
            var path = GetPath(id);
            if (FileStore.Exists(path))
                FileStore.DeleteFile(path);
        }

        public void PersistJson(string id, string json)
        {
            var path = GetPath(id);
            FileStore.WriteFile(path, json);
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