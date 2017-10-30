using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanLib
{
    /// <summary>
    /// Trying to make a interface to implement daily rate calculations.
    /// https://en.wikipedia.org/wiki/Day_count_convention#Actual_methods
    /// Good inspiration: http://strata.opengamma.io/day_counts/
    /// // DayCounter -> year fraction -> Interest rate calculator -> monetary interest base on principal and rate fraction.
    /// Since some of them require differemt parameters, this is a bit tricky.
    /// </summary>
    public interface IDayCounter
    {
        int GetDayCount(DateTime firstDate, DateTime secondDate /*, ScheduleInfo scheduleInfo*/);

        double GetYearFraction(DateTime firstDate, DateTime secondDate);

    }

    public static class DayCountHelper
    {
        /// <summary>
        /// returning the number of days between StartDate and EndDate on a Julian basis (i.e., all days are counted). For instance, Days(15 October 2007, 15 November 2007) returns 31.
        /// </summary>
        public static int Days(DateTime startDate, DateTime endDate)
        {
            return (endDate.Date - startDate.Date).Days;
        }
    }

    public abstract class Thirty360 : IDayCounter
    {
        public abstract int GetDayCount(DateTime firstDate, DateTime secondDate);

        public double GetYearFraction(DateTime firstDate, DateTime secondDate)
        {
            return GetDayCount(firstDate, secondDate) / 360.0d;
        }

        protected int Thrity360Days(DateTime firstDate, DateTime secondDate, int d1, int d2)
        {
            return 360 * (secondDate.Year - firstDate.Year) + 30 * (secondDate.Month - firstDate.Month) + (d2 - d1);
        }
    }

    public class Thirty360Psa : Thirty360
    {
        public override int GetDayCount(DateTime firstDate, DateTime secondDate)
        {
            int d1 = firstDate.Day;
            int d2 = secondDate.Day;
            if (d1 == 31 || IsLastDayOfFebruary(firstDate))
            {
                d1 = 30;
            }
            if (d2 == 31 && d1 == 30)
            {
                d2 = 30;
            }
            return Thrity360Days(firstDate, secondDate, d1, d2);
        }

        private bool IsLastDayOfFebruary(DateTime date)
        {
            return (date.Month == 2 && DateTime.DaysInMonth(date.Year, date.Month) == date.Day);
        }
    }

    public class Thirty360Isda : Thirty360
    {
        public override int GetDayCount(DateTime firstDate, DateTime secondDate)
        {
            int d1 = firstDate.Day;
            int d2 = secondDate.Day;
            if (d1 == 31)
            {
                d1 = 30;
            }
            if (d2 == 31 && d1 == 30)
            {
                d2 = 30;
            }
            return Thrity360Days(firstDate, secondDate, d1, d2);
        }
    }

    public class Actual365F : IDayCounter
    {
        public int GetDayCount(DateTime firstDate, DateTime secondDate)
        {
            return DayCountHelper.Days(firstDate, secondDate);
        }

        public double GetYearFraction(DateTime firstDate, DateTime secondDate)
        {
            return GetDayCount(firstDate, secondDate) / 365.0;
        }
    }
}
