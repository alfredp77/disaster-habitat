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
        private const string DELETED_FOLDER = "data/deleted";
        private TestModel _tm1, _tm2;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _fileStore = _mockRepository.Create<IMvxFileStore>();
            _serializer = _mockRepository.Create<IJsonSerializer>();

            _context = new FileBasedPersistenceContext<TestModel>(DATA_FOLDER, _serializer.Object, _fileStore.Object);

            _tm1 = new TestModel { ObjectId = "1", Name = "x" };
            _tm2 = new TestModel { ObjectId = "2", Name = "y" };
        }


        private string SetupFileStore(TestModel tm)
        {
            var fileName = $"{tm.ObjectId}.json";
            var fullPath = $"{DATA_FOLDER}/{fileName}";
            _fileStore.Setup(f => f.PathCombine(DATA_FOLDER, fileName))
                .Returns(fullPath);
            return fullPath;
        }

        private string SetupDeletedFileStore(TestModel tm)
        {
            var fileName = $"{tm.ObjectId}.json";
            var fullPath = $"{DELETED_FOLDER}/{fileName}";
            _fileStore.Setup(f => f.PathCombine(DATA_FOLDER, "deleted")).Returns(DELETED_FOLDER);
            _fileStore.Setup(f => f.PathCombine(DELETED_FOLDER, fileName)).Returns(fullPath);
            return fullPath;
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


        private void SetupDeserialize(TestModel tm, string pathToFile, bool canReadFile=true)
        {
            var json1 = Guid.NewGuid().ToString();
            _fileStore.Setup(f => f.TryReadTextFile(pathToFile, out json1)).Returns(canReadFile);
            _serializer.Setup(s => s.Deserialize<TestModel>(json1)).Returns(tm);
        }

        [Test]
        public void Should_Load_All_Documents()
        {
            var fileName1 = SetupFileStore(_tm1);
            SetupDeserialize(_tm1, fileName1);
            var fileName2 = SetupFileStore(_tm2);
            SetupDeserialize(_tm2, fileName2);
            _fileStore.Setup(f => f.GetFilesIn(DATA_FOLDER)).Returns(new[] { fileName1, fileName2 });

            var result = _context.LoadAll();

            Assert.That(result, Is.EquivalentTo(new[] {_tm1, _tm2}));
        }

        [Test]
        public void Should_Ignore_Unreadable_Files()
        {
            var fileName1 = SetupFileStore(_tm1);
            SetupDeserialize(_tm1, fileName1);
            var fileName2 = SetupFileStore(_tm2);
            SetupDeserialize(_tm2, fileName2, false);
            _fileStore.Setup(f => f.GetFilesIn(DATA_FOLDER)).Returns(new[] { fileName1, fileName2 });

            var result = _context.LoadAll();

            Assert.That(result, Is.EquivalentTo(new[] { _tm1 }));
        }

        [Test]
        public void Should_Purge_And_Recreate_Folder_On_PurgeAll()
        {
            _context.PurgeAll();

            _fileStore.Verify(f => f.DeleteFolder(DATA_FOLDER, true), Times.Once);
            _fileStore.Verify(f => f.EnsureFolderExists(DATA_FOLDER));
        }

        [Test]
        public void Should_Purge_File_When_Exists()
        {
            var fileName = SetupFileStore(_tm1);
            _fileStore.Setup(f => f.Exists(fileName)).Returns(true);
            var deletedFileName = SetupDeletedFileStore(_tm1);
            _fileStore.Setup(f => f.Exists(deletedFileName)).Returns(true);

            _context.Purge(_tm1.ObjectId);

            _fileStore.Verify(f => f.DeleteFile(fileName), Times.Once);
            _fileStore.Verify(f => f.DeleteFile(deletedFileName), Times.Once);
        }

        [Test]
        public void Should_Not_Try_To_Purge_File_When_Not_Exists()
        {
            var fileName = SetupFileStore(_tm1);
            _fileStore.Setup(f => f.Exists(fileName)).Returns(false);

            _context.Purge(_tm1.ObjectId);

            _fileStore.Verify(f => f.DeleteFile(fileName), Times.Never);
        }

        [Test]
        public void Should_Persist_Single_Json()
        {
            var fileName = SetupFileStore(_tm1);
            var json = SetupSerializer(_tm1);

            _context.PersistJson(_tm1.ObjectId, json);

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

            _context.PersistAllJson(new [] { new KeyValuePair<string, string>(_tm1.ObjectId, json1), new KeyValuePair<string, string>(_tm2.ObjectId, json2) });

            _fileStore.Verify(f => f.WriteFile(fileName1, json1), Times.Once());
            _fileStore.Verify(f => f.WriteFile(fileName2, json2), Times.Once());
            _serializer.Verify(s => s.Serialize(_tm1), Times.Never);
            _serializer.Verify(s => s.Serialize(_tm2), Times.Never);
        }

        [Test]
        public void Should_Move_File_To_Deleted_Folder_When_Marking_Object_As_Deleted()
        {
            var fileName = SetupFileStore(_tm1);
            _fileStore.Setup(f => f.Exists(fileName)).Returns(true);
            var deletedFileName = SetupDeletedFileStore(_tm1);

            _context.MarkDeleted(_tm1);
            
            _fileStore.Verify(f => f.EnsureFolderExists(DELETED_FOLDER), Times.Once);
            _fileStore.Verify(f => f.TryMove(fileName, deletedFileName, true));
        }

        [Test]
        public void Should_Return_Deleted_Objects()
        {
            var deletedTm1 = SetupDeletedFileStore(_tm1);
            SetupDeserialize(_tm1, deletedTm1);
            var deletedTm2 = SetupDeletedFileStore(_tm2);
            SetupDeserialize(_tm2, deletedTm2);

            _fileStore.Setup(f => f.GetFilesIn(DELETED_FOLDER)).Returns(new[] { deletedTm1, deletedTm2 });

            var result = _context.LoadDeletedObjects().ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Contains(_tm1));
            Assert.That(result.Contains(_tm2));
        }
    }
}
