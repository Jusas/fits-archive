using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Entities;
using NUnit.Framework;

namespace FitsArchiveLib.Tests
{
    [TestFixture]
    public class FitsFileTests
    {
        [Test(Description = "Reading a FITS file, expecting headers to be read correctly with correct type information")]
        public void TestReadingFitsFile()
        {
            var fitsPath = Path.Combine(TestUtils.GetTestPath(), @"Resources\FitsFiles\Light_001.fits");
            FitsFileInfo ff = new FitsFileInfo(fitsPath);
            var keys = ff.HeaderKeys;

            var telescope = ff.GetSingleHeaderValue("TELESCOP");
            var binningX = ff.GetSingleHeaderValue("XBINNING");
            var extend = ff.GetSingleHeaderValue("EXTEND");
            Assert.AreEqual("Celestron NexStar", telescope.Value);
            Assert.AreEqual((double)2, binningX.Value);
            Assert.AreEqual(true, extend.Value);

        }
    }
}
