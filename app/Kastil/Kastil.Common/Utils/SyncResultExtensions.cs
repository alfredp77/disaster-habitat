using System.Collections.Generic;
using Kastil.Common.Models;

namespace Kastil.Common.Utils
{
    public static class SyncResultExtensions
    {
        public static UpdateResult<T> Merge<T>(this UpdateResult<T> me, params UpdateResult<T>[] other) where T : BaseModel
        {
            var all = new List<UpdateResult<T>> {me};
            all.AddRange(other);

            var merged = new UpdateResult<T>();
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