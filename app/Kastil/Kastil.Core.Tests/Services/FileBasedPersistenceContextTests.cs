using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Core.Services;
using Moq;
using MvvmCross.Plugins.File;
using NUnit.Framework;

namespace Kastil.Core.Tests.Services
{

    [TestFixture]
    public class FileBasedPersistenceContextTests
    {
        private MockRepository _mockRepository;
        private FileBasedPersistenceContext<TestModel> _context;
        private Mock<IMvxFileStore> _fileStore;
        private Mock<IJsonSerializer> _serializer;        
        private const string DATA_FOLDER = "data";
        private TestModel _tm1, _tm2;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _fileStore = _mockRepository.Create<IMvxFileStore>();
            _serializer = _mockRepository.Create<IJsonSerializer>();

            _context = new FileBasedPersistenceContext<TestModel>(DATA_FOLDER, _serializer.Object, _fileStore.Object);

            _tm1 = new TestModel { Id = "1", Name = "x" };
            _tm2 = new TestModel { Id = "2", Name = "y" };
        }


        private string SetupFileStore(TestModel tm)
        {
            var fileName = Guid.NewGuid().ToString();
            _fileStore.Setup(f => f.PathCombine(DATA_FOLDER, tm.Id))
                .Returns(fileName);
            return fileName;
        }

        private string SetupSerializer(TestModel tm)
        {
            var json = Guid.NewGuid().ToString();
            _serializer.Setup(s => s.Serialize(tm)).Returns(json);
            return json;
        }


        [Test]
        public void Should_Save_Document_To_File()
        {            
            var fileName = SetupFileStore(_tm1);
            var json = SetupSerializer(_tm1);
            _context.Save(_tm1);
            _fileStore.Verify(f => f.WriteFile(fileName, json), Times.Once);
        }

        [Test]
        public void Should_Save_All_Documents()
        {
            var fileName1 = SetupFileStore(_tm1);
            var fileName2 = SetupFileStore(_tm2);

            var json1 = SetupSerializer(_tm1);
            var json2 = SetupSerializer(_tm2);

            _context.SaveAll(new[] {_tm1, _tm2});

            _fileStore.Verify(f => f.WriteFile(fileName1, json1), Times.Once);
            _fileStore.Verify(f => f.WriteFile(fileName2, json2), Times.Once);
        }


        private string SetupDeserialize(TestModel tm, bool canReadFile=true)
        {
            var json1 = Guid.NewGuid().ToString();
            var fileName1 = Guid.NewGuid().ToString();
            _fileStore.Setup(f => f.TryReadTextFile(fileName1, out json1)).Returns(canReadFile);
            _serializer.Setup(s => s.Deserialize<TestModel>(json1)).Returns(tm);
            return fileName1;
        }

        [Test]
        public void Should_Load_All_Documents()
        {
            var fileName1 = SetupDeserialize(_tm1);
            var fileName2 = SetupDeserialize(_tm2);
            _fileStore.Setup(f => f.GetFilesIn(DATA_FOLDER)).Returns(new[] { fileName1, fileName2 });

            var result = _context.LoadAll();

            Assert.That(result, Is.EquivalentTo(new[] {_tm1, _tm2}));
        }

        [Test]
        public void Should_Ignore_Unreadable_Files()
        {
            var fileName1 = SetupDeserialize(_tm1);
            var fileName2 = SetupDeserialize(_tm2, false);
            _fileStore.Setup(f => f.GetFilesIn(DATA_FOLDER)).Returns(new[] { fileName1, fileName2 });

            var result = _context.LoadAll();

            Assert.That(result, Is.EquivalentTo(new[] { _tm1 }));
        }

        [Test]
        public void Should_Delete_And_Recreate_Folder_On_DeleteAll()
        {
            _context.DeleteAll();

            _fileStore.Verify(f => f.DeleteFolder(DATA_FOLDER, true), Times.Once);
            _fileStore.Verify(f => f.EnsureFolderExists(DATA_FOLDER));
        }

        [Test]
        public void Should_Delete_File_When_Exists()
        {
            var fileName = SetupFileStore(_tm1);
            _fileStore.Setup(f => f.Exists(fileName)).Returns(true);

            _context.Delete(_tm1);

            _fileStore.Verify(f => f.DeleteFile(fileName), Times.Once);
        }

        [Test]
        public void Should_Not_Try_To_Delete_File_When_Not_Exists()
        {
            var fileName = SetupFileStore(_tm1);
            _fileStore.Setup(f => f.Exists(fileName)).Returns(false);

            _context.Delete(_tm1);

            _fileStore.Verify(f => f.DeleteFile(fileName), Times.Never);
        }

        [Test]
        public void Should_Persist_Single_Json()
        {
            var fileName = SetupFileStore(_tm1);
            var json = SetupSerializer(_tm1);

            _context.PersistJson(_tm1.Id, json);

            _fileStore.Verify(f => f.WriteFile(fileName, json), Times.Once());
            _serializer.Verify(s => s.Serialize(_tm1), Times.Never);
        }

        [Test]
        public void Should_Persist_Multiple_Jsons()
        {
            var fileName1 = SetupFileStore(_tm1);
            var json1 = SetupSerializer(_tm1);

            var fileName2 = SetupFileStore(_tm2);
            var json2 = SetupSerializer(_tm2);

            _context.PersistAllJson(new [] { new KeyValuePair<string, string>(_tm1.Id, json1), new KeyValuePair<string, string>(_tm2.Id, json2) });

            _fileStore.Verify(f => f.WriteFile(fileName1, json1), Times.Once());
            _fileStore.Verify(f => f.WriteFile(fileName2, json2), Times.Once());
            _serializer.Verify(s => s.Serialize(_tm1), Times.Never);
            _serializer.Verify(s => s.Serialize(_tm2), Times.Never);
        }
    }
}
