using Kastil.Core.Utils;
using Kastil.Shared.Models;
using MvvmCross.Plugins.File;

namespace Kastil.Core.Services
{
    public class FileBasedPersistenceContextFactory : BaseService, IPersistenceContextFactory
    {
        public const string DATA_PATH = "offline_data";

        private IJsonSerializer Serializer => Resolve<IJsonSerializer>();
        private FolderProvider FolderProvider => Resolve<FolderProvider>();
        private IMvxFileStore FileStore => Resolve<IMvxFileStore>();

        private string GetStorageFolder<T>(string name) where T : BaseModel
        {
            var path = FileStore.PathCombine(FolderProvider.MyDocuments, DATA_PATH);
            var folderName = typeof (T).Name;
            if (!string.IsNullOrEmpty(name))
                folderName = $"{name}_{folderName}";
            path = FileStore.PathCombine(path, folderName.ToLowerInvariant());
            FileStore.EnsureFolderExists(path);
            return path;
        }

        public IPersistenceContext<T> CreateFor<T>(string name="") where T : BaseModel
        {
            return new FileBasedPersistenceContext<T>(GetStorageFolder<T>(name), Serializer, FileStore);
        }
    }
}