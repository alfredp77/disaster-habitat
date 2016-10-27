using System.Collections.Generic;
using Kastil.Common.Utils;
using Kastil.Shared.Models;
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
            var id = document.Id;
            var path = GetPath(id);
            FileStore.WriteFile(path, Serializer.Serialize(document));
        }

        private string GetPath(string id)
        {
            return FileStore.PathCombine(DataFolder, id.ToLowerInvariant());
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
            foreach (var file in files)
            {
                string contents;
                if (FileStore.TryReadTextFile(file, out contents))
                {
                    yield return Serializer.Deserialize<T>(contents);
                }
            }
        }

        public void DeleteAll()
        {
            FileStore.DeleteFolder(DataFolder, true);
            FileStore.EnsureFolderExists(DataFolder);
        }

        public void Delete(T document)
        {
            var path = GetPath(document.Id);
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