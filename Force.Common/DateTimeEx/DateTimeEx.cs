using System;
using System.Collections.Generic;
using System.Text;

namespace Force.Common.DateTimeEx
{
    public static class DateTimeEx
    {
        public static long ToTimeStamp(this DateTime time)
        {
            return (time.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        public static DateTime ToDateTime(this long unix)
        {
            var timeStamp = new DateTime(1970, 1, 1);  //得到1970年的时间戳
            var t = (unix + 8 * 60 * 60) * 10000000 + timeStamp.Ticks;
            var dt = new DateTime(t);

            return dt;
        }
    }
}
