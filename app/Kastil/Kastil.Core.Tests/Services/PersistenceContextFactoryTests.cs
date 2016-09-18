using Kastil.Core.Services;
using Kastil.Core.Utils;
using Moq;
using MvvX.Plugins.CouchBaseLite;
using MvvX.Plugins.CouchBaseLite.Database;
using NUnit.Framework;

namespace Kastil.Core.Tests.Services
{
    [TestFixture]
    public class PersistenceContextFactoryTests : BaseTest
    {
        private IPersistenceContextFactory _factory;
        private Mock<ICouchBaseLite> _cbLite;
        private Mock<IJsonSerializer> _serializer;

        public override void CreateTestableObject()
        {           
            _factory = new PersistenceContextFactory();
            _cbLite = CreateMock<ICouchBaseLite>();
            _serializer = CreateMock<IJsonSerializer>();

            Ioc.RegisterSingleton(_cbLite.Object);
            Ioc.RegisterSingleton(_serializer.Object);
        }

        [Test]
        public void Should_Create_PersistenceContext_Correctly()
        {
            var options = CreateMock<IDatabaseOptions>();
            _cbLite.Setup(c => c.CreateDatabaseOptions()).Returns(options.Object);
            var db = CreateMock<IDatabase>();
            _cbLite.Setup(c => c.CreateConnection(PersistenceContextFactory.DATA_PATH, $"db_testmodel", options.Object)).Returns(db.Object);

            var context = _factory.CreateFor<TestModel>() as PersistenceContext<TestModel>;

            Assert.That(context, Is.Not.Null);
            Assert.That(context.Db, Is.EqualTo(db.Object));
            Assert.That(context.Serializer, Is.EqualTo(_serializer.Object));
        }
    }
}