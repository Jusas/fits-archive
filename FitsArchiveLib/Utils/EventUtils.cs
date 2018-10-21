using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib.Utils
{
    public static class EventUtils
    {
        public static void IgnoreExceptions(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch
            {
                // Do nothing.
            }
        }
    }
}
