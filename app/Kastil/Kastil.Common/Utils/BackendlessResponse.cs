using Kastil.Common.Models;

namespace Kastil.Common.Utils
{
    public class BackendlessResponse<T> where T: BaseModel
    {
        public static BackendlessResponse<T> Success(T content)
        {
            return new BackendlessResponse<T> {Content = content};
        }

        public static BackendlessResponse<T> Failed(string code = null, string message = null)
        {
            return new BackendlessResponse<T> {ErrorCode = code ?? "Unknown", ErrorMessage = message ?? "Something went wrong ..." };
        }

        private BackendlessResponse()
        {}

        public T Content { get; private set; }

        public string ErrorMessage { get; private set; }

        public string ErrorCode { get; private set; }

        public bool IsSuccessful => !string.IsNullOrEmpty(ErrorMessage);

        public override string ToString()
        {
            return IsSuccessful
                ? $"Content:{typeof(T).Name}, Id:{Content.ObjectId}"
                : $"{ErrorCode}: {ErrorMessage}";
        }
    }
}