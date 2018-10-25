using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Property, AllowMultiple = false)]
    public class FitsFieldAttribute : Attribute
    {
        /// <summary>
        /// True if the fits header is a multi-value header.
        /// </summary>
        public bool MultiValue { get; set; }
        /// <summary>
        /// FITS header field name.
        /// </summary>
        public string Name { get; set; }
        public bool Numeric { get; set; }
        public bool DateLike { get; set; }
        public bool VarianceValue { get; set; }

        public FitsFieldAttribute()
        {
        }
    }
}
