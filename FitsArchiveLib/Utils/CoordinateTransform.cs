using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib.Utils
{
    public static class CoordinateTransform
    {
        /// <summary>
        /// Converts right ascension HH MM SS.sss format to decimal degrees.
        /// </summary>
        /// <param name="ra"></param>
        /// <returns></returns>
        public static double HmsToDegrees(string ra)
        {
            if (string.IsNullOrEmpty(ra))
                return double.NaN;

            var parts = ra.Split(' ', ':');
            int hours = Int32.Parse(parts[0]);
            int minutes = parts.Length > 1 ? Int32.Parse(parts[1]) : 0;
            double seconds = parts.Length > 2 ? double.Parse(parts[2], CultureInfo.InvariantCulture) : 0;

            var hhh = HmsToHhh(hours, minutes, seconds);
            var deg = HhhToDeg(hhh);
            return deg;
        }

        /// <summary>
        /// Converts declination +/-DD MM SS.sss format to decimal degrees.
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static double DmsToDegrees(string dec)
        {
            if (string.IsNullOrEmpty(dec))
                return double.NaN;

            var parts = dec.Split(' ', ':');
            int degrees = Int32.Parse(parts[0]);
            int minutes = parts.Length > 1 ? Int32.Parse(parts[1]) : 0;
            double seconds = parts.Length > 2 ? double.Parse(parts[2], CultureInfo.InvariantCulture) : 0;

            var deg = HmsToHhh(degrees, minutes, seconds);
            return deg;
        }

        /// <summary>
        /// Converts arcminutes to degrees.
        /// </summary>
        /// <param name="arcmin"></param>
        /// <returns></returns>
        public static double ArcminToDegrees(int arcmin)
        {
            return arcmin / 60.0;
        }

        /// <summary>
        /// Converts degrees to arcminutes.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double DegreesToArcminutes(double degrees)
        {
            return 60.0 * degrees;
        }

        private static double HmsToHhh(int hours, int minutes, double seconds)
        {
            double sign = 1.0;

            if (hours < 0)
            {
                sign = -1.0;
                hours = -hours;
            }
            if (minutes < 0)
            {
                sign = -1.0;
                minutes = -minutes;
            }
            if (seconds < 0)
            {
                sign = -1.0;
                seconds = -seconds;
            }

            return sign * (hours + minutes / 60.0 + seconds / 3600.0);
        }

        private static double HhhToDeg(double hhh)
        {
            var deg = 15.0 * hhh;
            if (deg == 360.0)
                deg = 0.0;
            return deg;
        }

    }
}
