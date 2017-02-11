using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using NUnit.Framework;

namespace Kastil.Common.Tests.Services
{
    [TestFixture]
    public class RemovalPushServiceTests
    {
        private RemovalPushService _service;
        private IJsonSerializer _jsonSerializer;
        private IPersistenceContextFactory _persistenceContextFactory;
        private IPersistenceContext<Assessment> _persistenceContext;
        private IBackendlessResponseParser _responseParser;
        private Connection _connection = new Connection();

    }
}
