using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode
{
    public static class DateUtils
    {
        public static bool IsFirstBusinessDayOfMonth(DateTime currentDate)
        {
            var returnValue = false;
            var weekends = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday };

            int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

            IEnumerable<int> businessDaysInMonth = Enumerable.Range(1, daysInMonth)
                                    .Where(d => !weekends.Contains(new DateTime(currentDate.Year, currentDate.Month, d).DayOfWeek));

            int firstBusinessDayInMonth = businessDaysInMonth.FirstOrDefault<int>();
            if (firstBusinessDayInMonth == currentDate.Day)
            {
                returnValue = true;
            }
            return returnValue;
        }
    }
}