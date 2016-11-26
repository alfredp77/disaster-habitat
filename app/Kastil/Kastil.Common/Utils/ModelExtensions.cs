using Kastil.Common.Models;

namespace Kastil.Common.Utils
{
    public static class ModelExtensions
    {
        public static bool IsNew(this BaseModel model)
        {
            return string.IsNullOrEmpty(model.ObjectId);
        }
    }
}