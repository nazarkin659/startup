using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperFunctions
{
    public static class DateTimeExtentions
    {
        public static DateTime GetClosestDateTime(DateTime datetime)
        {
            //DateTime fileDate, closestDate;
            //long min = long.MaxValue;

            //if (Math.Abs(DateTime.Now.Ticks - fileDate.Ticks) < min)
            //{
            //    min = date.Ticks - fileDate.Ticks;
            //    closestDate = date;
            //}
            return DateTime.Now;
        }
    }
}
