using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Core.Services
{
    public interface ISyncService
    {
        Task<SyncResult> Sync(User user);
    }

    public class SyncResult
    {
        public static SyncResult Success()
        {
            return new SyncResult();
        }

        public static SyncResult Failed(string errorMessage)
        {
            return new SyncResult {ErrorMessage = errorMessage};
        }

        public bool HasErrors => !string.IsNullOrEmpty(ErrorMessage);
        public string ErrorMessage { get; private set; }
    }
}