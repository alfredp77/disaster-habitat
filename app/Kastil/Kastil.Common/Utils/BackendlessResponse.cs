using System.Collections.Generic;
using System.Linq;
using Kastil.Common.Models;

namespace Kastil.Common.Utils
{
    public class BackendlessResponse<T> where T: BaseModel
    {
        public static BackendlessResponse<T> Success(params T[] content)
        {
            return new BackendlessResponse<T>(content);
        }

        public static BackendlessResponse<T> Failed(string code = null, string message = null)
        {
            return new BackendlessResponse<T> {ErrorCode = code ?? "Unknown", ErrorMessage = message ?? "Something went wrong ..." };
        }

        private readonly List<T> _content;

        private BackendlessResponse(params T[] content)
        {
            _content = content.ToList();
        }

        public IEnumerable<T> Content => _content;

        public string ErrorMessage { get; private set; }

        public string ErrorCode { get; private set; }

        public bool IsSuccessful => string.IsNullOrEmpty(ErrorMessage);

        public override string ToString()
        {
            return IsSuccessful
                ? $"Content:{typeof(T).Name}, Id:{string.Join(",", _content.Select(c => c.ObjectId))}"
                : $"{ErrorCode}: {ErrorMessage}";
        }
    }



}