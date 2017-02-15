using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kastil.Common.Utils
{
    public interface IBackendlessQueryProvider
    {
        IBackendlessQuery Where();        
    }

    public interface IQuery
    {
    }

    public interface IBackendlessQuery : IQuery
    {
        IBackendlessQuery OwnedBy(string ownerId);
        IBackendlessQuery IsActive();
    }

    public class BackendlessQueryProvider : IBackendlessQueryProvider
    {
        public IBackendlessQuery Where()
        {
            return new BackendlessQuery();
        }
    }

    public class BackendlessQuery : IBackendlessQuery
    {
        private string _ownerId;
        private bool? _isActive;

        public IBackendlessQuery OwnedBy(string ownerId)
        {
            _ownerId = ownerId;
            return this;
        }

        public IBackendlessQuery IsActive()
        {
            _isActive = true;
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(_ownerId))
                sb.Append($"ownerId='{_ownerId}'");

            if (_isActive != null)
            {
                if (sb.Length > 0)
                    sb.Append(" AND ");
                sb.Append($"isActive={_isActive.Value.ToString().ToLower()}");
            }

            return $"?where={WebUtility.UrlEncode(sb.ToString()).Replace("+", "%20")}";
        }
    }
}
