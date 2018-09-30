using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Entities;
using FitsArchiveLib.Interfaces;
using FitsArchiveLib.Services;
using FitsArchiveLib.Tests.Mocks;
using NUnit.Framework;
using Moq;

namespace FitsArchiveLib.Tests
{
    [TestFixture]
    public class FitsDatabaseTests
    {
        private List<string> _perTestDeletables = new List<string>();

        [OneTimeTearDown]
        public void AfterAllTests()
        {

        }

        [TearDown]
        public void AfterEachTest()
        {
            foreach (var deletable in _perTestDeletables)
            {
                if (File.Exists(deletable))
                {
                    File.Delete(deletable);
                }
            }
            _perTestDeletables.Clear();
        }

        [Test(Description = "Test autocreating a new database file at path when it doesn't exist")]
        public void TestDbAutoCreate()
        {
            var fitsCreator = new FitsFileInfoFactory();
            var file = TestUtils.GenerateRandomFileName();
            _perTestDeletables.Add(file);
            using (var newFitsDb = new FitsDatabase(fitsCreator, null, file, true));
            Assert.AreEqual(true, File.Exists(file));
        }

        [Test(Description = "Test inserting/indexing a FITS file into a new test database")]
        public async Task TestIndexingAFitsFile()
        {
            var fitsCreator = new FitsFileInfoFactory();
            var file = TestUtils.GenerateRandomFileName();
            _perTestDeletables.Add(file);
            using (var newFitsDb = new FitsDatabase(fitsCreator, null, file, true))
            {
                var fitsFilePath = Path.Combine(TestUtils.GetTestPath(), "Resources", "FitsFiles", "Light_001.fits");
                await newFitsDb.AddFiles(new[] {fitsFilePath});
                Assert.AreEqual(1, newFitsDb.FileCount);
            }
        }

        [Test(Description = "Test updating a FITS file with identical data and path in a new test database")]
        public async Task TestIndexingAnExistingFitsFileInDb()
        {
            var fitsCreator = new FitsFileInfoFactory();
            var file = TestUtils.GenerateRandomFileName();
            _perTestDeletables.Add(file);
            using (var newFitsDb = new FitsDatabase(fitsCreator, null, file, true))
            {
                var fitsFilePath = Path.Combine(TestUtils.GetTestPath(), "Resources", "FitsFiles", "Light_001.fits");
                await newFitsDb.AddFiles(new[] { fitsFilePath, fitsFilePath });
                Assert.AreEqual(1, newFitsDb.FileCount);
            }
        }

        [Test(Description = "Test updating a FITS file that exists in DB but has changed")]
        public async Task TestIndexingAChangedFitsFileInDb()
        {

            var fitsCreator = new Mock<IFitsFileInfoFactory>();
            fitsCreator.Setup(x => x.CreateFitsFileInfo(It.IsAny<string>())).Returns(
                (string filePath) => new MockFitsFileInfo(filePath));

            var file = TestUtils.GenerateRandomFileName();
            using (var newFitsDb = new FitsDatabase(fitsCreator.Object, null, file, true))
            {
                var fitsFilePath = "test.fits";
                // MockFitsFileInfo generates a new comment value and therefore new checksum for every instance,
                // so when it's added a second time it should be recognized as changed.
                await newFitsDb.AddFiles(new[] { fitsFilePath, fitsFilePath });
                Assert.AreEqual(1, newFitsDb.FileCount);
            }
        }
    }
}
