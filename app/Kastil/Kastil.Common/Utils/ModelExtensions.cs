using System;
using Kastil.Common.Models;

namespace Kastil.Common.Utils
{
    public static class ModelExtensions
    {
        public static bool IsNew(this BaseModel model)
        {
			return string.IsNullOrEmpty(model.ObjectId) || model.ObjectId.StartsWith("LOCAL-", StringComparison.CurrentCultureIgnoreCase);
        }

        public static void StampNewId(this BaseModel model)
		{
            if (string.IsNullOrEmpty(model.ObjectId))
			    model.ObjectId = $"LOCAL-{Guid.NewGuid()}";
		}

		public static void RevokeNewId(this BaseModel model)
		{
			if (IsNew(model)) 
			{
				model.ObjectId = null;
			}
		}

    }
}