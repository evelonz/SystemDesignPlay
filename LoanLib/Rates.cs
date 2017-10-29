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
    public interface IInterestCalculatorAdvanced
    {
        double GetInterest(double rate, double principal, int days);

    }

    public static class InterestHelper
    {
        /// <summary>
        /// returning the number of days between StartDate and EndDate on a Julian basis (i.e., all days are counted). For instance, Days(15 October 2007, 15 November 2007) returns 31.
        /// </summary>
        public static int Days(DateTime startDate, DateTime endDate)
        {
            return (endDate.Date - startDate.Date).Days;
        }
    }

    public interface IInterestCalculator
    {
        double GetMonthlyInterest(double rate, double principal, int days);
    }

    public class RateActual365 : IInterestCalculator
    {
        public double GetMonthlyInterest(double rate, double principal, int days)
        {
            var dailyRate = rate / 100.0 / 365; // Not acctually true, should be 366 on leap years.
            return dailyRate * days * principal;
        }
    }

    public class RateActual360 : IInterestCalculator
    {
        public double GetMonthlyInterest(double rate, double principal, int days)
        {
            var dailyRate = rate / 100.0 / 360; // Not acctually true, should be 366 on leap years.
            return dailyRate * days * principal;
        }
    }

    /// <summary>
    /// A simple rate that is equeal for each year. Same as doing rate/12 for monthly invoices.
    /// </summary>
    public class Rate30360 : IInterestCalculator
    {
        public double GetMonthlyInterest(double rate, double principal, int days)
        {
            days = 30;
            var dailyRate = rate / 100.0 / 360; // Not acctually true, should be 366 on leap years.
            return dailyRate * days * principal;
        }
    }
}
