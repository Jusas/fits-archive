using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Property, AllowMultiple = false)]
    public class MultiValueFitsFieldAttribute : Attribute
    {
        public MultiValueFitsFieldAttribute()
        {
        }
    }
}
