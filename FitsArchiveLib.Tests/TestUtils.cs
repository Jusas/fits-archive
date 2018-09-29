using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib.Tests
{
    class TestUtils
    {
        public static string GetTestPath()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            return new System.Uri(path).LocalPath;
        }

        public static string GenerateRandomFileName()
        {
            return Path.Combine(GetTestPath(), Guid.NewGuid().ToString());
        }
    }
}
