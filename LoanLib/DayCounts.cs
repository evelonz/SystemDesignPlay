using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanLib
{
    // TODO: Remove this and do an actual implementation. Just testing it out.

    public class NewCool30360Loan
    {
        public double InterestRate { get; set; }
        public double CurrentPrincipal { get; set; }

        private double _emi { get; set; }
        public double Emi
        {
            get
            {
                if (_emi == 0.0 && this.CurrentPrincipal > 0.0)
                { SetEmi(); }
                return _emi;
            }
        }

        public double StartAmount { get; set; }
        public double Tenure { get; set; }

        private void SetEmi()
        {
            var r = this.InterestRate / 12.0 / 100.0;
            var n = this.Tenure * 12.0;
            var dividend = Math.Pow(1 + r, n);
            var divisor = dividend - 1;
            this._emi = this.StartAmount * r * (dividend / divisor);
        }

        public Invoice AddInvoice(DateTime invoiceDate, DateTime invoiceStartDate, DateTime invoiceEndDate, double invoiceFee, IDayCounter dayCounter)
        {
            var invoice = new Invoice();
            // Rate. Could probaply extract this some more.
            var yearFraction = dayCounter.GetYearFraction(invoiceStartDate, invoiceEndDate);
            var fractionRate = yearFraction * this.InterestRate / 100.0d;
            invoice.Interest = fractionRate * this.CurrentPrincipal;

            var leftForPrincipal = this.Emi - invoice.Interest;
            invoice.Principal = (leftForPrincipal <= this.CurrentPrincipal) ? leftForPrincipal : this.CurrentPrincipal;
            invoice.InvoiceDate = invoiceDate;
            invoice.InvoiceFee = invoiceFee;
            return invoice;
        }
    }

    public interface IDayCounter
    {
        int GetDayCount(DateTime firstDate, DateTime secondDate /*, ScheduleInfo scheduleInfo*/);

        double GetYearFraction(DateTime firstDate, DateTime secondDate);

    }

    public class Thirty360Psa : IDayCounter
    {
        public int GetDayCount(DateTime firstDate, DateTime secondDate)
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
            return 360 * (secondDate.Year - firstDate.Year) + 30 * (secondDate.Month - firstDate.Month) + (d2 - d1);
        }

        private bool IsLastDayOfFebruary(DateTime date)
        {
            return (date.Month == 2 && DateTime.DaysInMonth(date.Year, date.Month) == date.Day);
        }

        public double GetYearFraction(DateTime firstDate, DateTime secondDate)
        {
            return GetDayCount(firstDate, secondDate) / 360.0d;
        }
    }
}
