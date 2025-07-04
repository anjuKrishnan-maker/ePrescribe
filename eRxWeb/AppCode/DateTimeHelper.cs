using System;

namespace eRxWeb.AppCode
{
    public class DateTimeHelper
    {
        public static DateTime GetValidFutureDateOrToday(object date)
        {
            DateTime datetime;

            if (DateTime.TryParse(Convert.ToString(date), out datetime) && datetime > DateTime.Today)
            {
                return datetime;
            }

            return DateTime.Today;
        }
    }
}