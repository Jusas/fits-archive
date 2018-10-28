using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Entities;
using NUnit.Framework;

namespace FitsArchiveLib.Tests
{
    [TestFixture]
    public class FitsQueryBuilderTests
    {
        [Test]
        public void TestExpressionBuilding()
        {
            var qb = new FitsQueryBuilder();
            var expression = qb.KeywordMatching("AUTHOR", "Jussi");
        }
    }
}
