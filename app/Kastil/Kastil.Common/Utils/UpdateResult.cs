using System;
using System.Collections.Generic;
using Kastil.Common.Models;

namespace Kastil.Common.Utils
{
    public class UpdateResult<T> where T : BaseModel
    {
        private readonly Dictionary<string, string> _errorMessages = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _localIds = new Dictionary<string, string>();
        private readonly List<T> _successfulItems = new List<T>();
        private readonly List<T> _failedItems = new List<T>();

        public void Failed(T item, string errorMessage)
        {
            _failedItems.Add(item);
            _errorMessages[item?.ObjectId ?? Guid.NewGuid().ToString()] = errorMessage;
        }

        public void Success(T item, string localId)
        {
            _successfulItems.Add(item);
            _localIds[item.ObjectId] = localId;
        }

        public IEnumerable<T> SuccessfulItems => _successfulItems;
        public IEnumerable<T> FailedItems => _failedItems;

        public string GetLocalId(T item)
        {
            string localId;
            _localIds.TryGetValue(item.ObjectId, out localId);
            return localId;
        }

        public string GetErrorMessage(T item)
        {
            string errorMessage;
            _errorMessages.TryGetValue(item.ObjectId, out errorMessage);
            return errorMessage;
        }
    }
}