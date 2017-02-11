using System.Collections.Generic;
using Kastil.Common.Models;

namespace Kastil.Common.Utils
{
    public static class SyncResultExtensions
    {
        public static SyncResult<T> Merge<T>(this SyncResult<T> me, params SyncResult<T>[] other) where T : BaseModel
        {
            var all = new List<SyncResult<T>> {me};
            all.AddRange(other);

            var merged = new SyncResult<T>();
            foreach (var syncResult in all)
            {
                foreach (var item in syncResult.SuccessfulItems)
                {
                    merged.Success(item, syncResult.GetLocalId(item));
                }

                foreach (var item in syncResult.FailedItems)
                {
                    merged.Failed(item, syncResult.GetErrorMessage(item));
                }
            }
            return merged;
        }
    }

}