using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using Moq;
using MvvX.Plugins.CouchBaseLite.Database;
using MvvX.Plugins.CouchBaseLite.Queries;
using NUnit.Framework;

namespace Kastil.Core.Tests.Services
{
    [TestFixture]
    public class PersistenceContextTests
    {
        private MockRepository _mockRepository;
        private PersistenceContext<TestModel> _context;

        private Mock<IDatabase> _db;
        private Mock<IJsonSerializer> _serializer;

        private TestModel _model1, _model2;
        private Dictionary<string, object> _props1, _props2;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _db = _mockRepository.Create<IDatabase>();
            _serializer = _mockRepository.Create<IJsonSerializer>();

            _context = new PersistenceContext<TestModel>(_db.Object, _serializer.Object);

            _model1 = new TestModel { Id = "abc", Name = "Model1"};
            _props1 = new Dictionary<string, object>();
            _model2 = new TestModel { Id = "def", Name = "Model2" };
            _props2 = new Dictionary<string, object>();

            _serializer.Setup(s => s.Serialize(_model1)).Returns("model1");
            _serializer.Setup(s => s.Deserialize<Dictionary<string, object>>("model1")).Returns(_props1);

            _serializer.Setup(s => s.Serialize(_model2)).Returns("model2");
            _serializer.Setup(s => s.Deserialize<Dictionary<string, object>>("model2")).Returns(_props2);
        }

        [Test]
        public void Should_Save_Individual_Document()
        {
            _context.Save(_model1);
            _db.Verify(d => d.PutLocalDocument(_model1.Id, _props1));
        }

        [Test]
        public void Should_Save_Multiple_Documents()
        {
            _context.SaveAll(new[] { _model1, _model2});
            _db.Verify(d => d.PutLocalDocument(_model1.Id, _props1));
            _db.Verify(d => d.PutLocalDocument(_model2.Id, _props2));
        }

        [Test]
        public void Should_Delete_Db()
        {
            _context.DeleteAll();
            _db.Verify(d => d.Delete());
        }

        [Test]
        public void Should_Persist_Individual_Json()
        {
            _context.PersistJson("abc", "model1");
            _db.Verify( d=> d.PutLocalDocument("abc", _props1));
        }

        [Test]
        public void Should_Persist_Multiple_Jsons()
        {
            _context.PersistAllJson(new []
            {
                new KeyValuePair<string, string>("abc", "model1"),
                new KeyValuePair<string, string>("def", "model2"),
            });
            _db.Verify(d => d.PutLocalDocument("abc", _props1));
            _db.Verify(d => d.PutLocalDocument("def", _props2));
        }

        [Test]
        public void Should_Load_All_Objects()
        {
            var query = _mockRepository.Create<IQuery>();
            _db.Setup(d => d.CreateAllDocumentsQuery()).Returns(query.Object);
            var queryEnumerator = _mockRepository.Create<IQueryEnumerator>();
            query.Setup(q => q.Run()).Returns(queryEnumerator.Object);

            var row = _mockRepository.Create<IQueryRow>();
            row.SetupGet(r => r.DocumentProperties).Returns(_props1);
            _serializer.Setup(s => s.Serialize(_props1)).Returns("model1");
            _serializer.Setup(s => s.Deserialize<TestModel>("model1")).Returns(_model1);

            var rows = new List<IQueryRow> {row.Object};
            queryEnumerator.Setup(q => q.GetEnumerator()).Returns(rows.GetEnumerator());

            var result = _context.LoadAll().ToList();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(_model1));
        }
    }

    class TestModel : BaseModel
    {
        public string Name { get; set; }
    }
}