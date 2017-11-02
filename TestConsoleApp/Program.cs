using LoanLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            //TestPayingOnLoan(new Actual365F());
            //TestPayingOnLoan(new Thirty360Psa());
            //TestPayingOnLoan(new Thirty360Isda());
            //TestCreatingInvoices(new Actual365F(), "Actual365F");
            //TestCreatingInvoices(new Thirty360Psa(), "Thirty360Psa");
            //TestCreatingInvoices(new Thirty360Isda(), "Thirty360Isda");
            TestCreatingInvoices2(new Thirty360Isda(), "FlatAThirty360Isda");
        }

        static void TestPayingOnLoan(IDayCounter dayCalculator)
        {
            var loan = new FixedEmiLoan()
            {
                InterestRate = 10,
                CurrentPrincipal = 10000,
                StartAmount = 10000,
                PayoutDate = new DateTime(2017, 01, 01),
                Tenure = 10,
            };

            var invoices = new List<Invoice>();
            var baseDate = loan.PayoutDate;
            for (int i = 0; i < 3; i++)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                var invoice = loan.AddInvoice(date, new DateTime(date.Year, date.Month, 1), baseDate, 0.0, dayCalculator);
                loan.CurrentPrincipal -= invoice.Principal;
                invoices.Add(invoice);

                //Console.WriteLine(invoice.ToString());
                //Console.WriteLine($"{invoice.InvoiceDate.ToString("MMM-yyyy")} | {Math.Round(invoice.FullInvoiceAmount, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Interest, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Principal, 0, MidpointRounding.AwayFromZero)} | {Math.Round(loan.CurrentPrincipal, 0, MidpointRounding.AwayFromZero)}");
            }

            loan.Invoices = invoices;
            var payment = loan.Pay(new DateTime(2017, 03, 31), 5000.0);
            Console.WriteLine(payment.ToString());

            Console.WriteLine($"SUM: Principal {invoices.Sum(s => s.Principal)}, Interest: {invoices.Sum(s => s.Interest)}, InvoiceFee: {invoices.Sum(s => s.InvoiceFee)}, LateFee: {invoices.Sum(s => s.LateFee)}");
            Console.WriteLine("Current Principal: " + loan.CurrentPrincipal);
        }

        static void TestCreatingInvoices(IDayCounter dayCalculator, string filename)
        {
            var loan = new FixedEmiLoan()
            {
                InterestRate = 10,
                CurrentPrincipal = 10000,
                StartAmount = 10000,
                PayoutDate = new DateTime(2017, 10, 01),
                Tenure = 10,
            };

            var invoices = new List<Invoice>();
            var baseDate = loan.PayoutDate;
            while (loan.CurrentPrincipal > 0.0)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                var invoice = loan.AddInvoice(date, new DateTime(date.Year, date.Month, 1), baseDate, 0.0, dayCalculator);
                loan.CurrentPrincipal -= invoice.Principal;
                invoices.Add(invoice);

                //Console.WriteLine(invoice.ToString());
                //Console.WriteLine($"{invoice.InvoiceDate.ToString("MMM-yyyy")} | {Math.Round(invoice.FullInvoiceAmount, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Interest, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Principal, 0, MidpointRounding.AwayFromZero)} | {Math.Round(loan.CurrentPrincipal, 0, MidpointRounding.AwayFromZero)}");
            }
            Console.WriteLine($"SUM: Principal {invoices.Sum(s => s.Principal)}, Interest: {invoices.Sum(s => s.Interest)}, InvoiceFee: {invoices.Sum(s => s.InvoiceFee)}, LateFee: {invoices.Sum(s => s.LateFee)}");
            TestOutput.CreateCSV(invoices, filename);
        }

        static void TestCreatingInvoices2(IDayCounter dayCalculator, string filename)
        {
            var loan = new FixedAmortizationLoan(70)
            {
                InterestRate = 10,
                CurrentPrincipal = 10000,
                StartAmount = 10000,
                PayoutDate = new DateTime(2017, 10, 01),
                Tenure = 10,
            };

            var invoices = new List<Invoice>();
            var baseDate = loan.PayoutDate;
            while (loan.CurrentPrincipal > 0.0)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                var invoice = loan.AddInvoice(date, new DateTime(date.Year, date.Month, 1), baseDate, 0.0, dayCalculator);
                loan.CurrentPrincipal -= invoice.Principal;
                invoices.Add(invoice);

                //Console.WriteLine(invoice.ToString());
                //Console.WriteLine($"{invoice.InvoiceDate.ToString("MMM-yyyy")} | {Math.Round(invoice.FullInvoiceAmount, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Interest, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Principal, 0, MidpointRounding.AwayFromZero)} | {Math.Round(loan.CurrentPrincipal, 0, MidpointRounding.AwayFromZero)}");
            }
            Console.WriteLine($"SUM: Principal {invoices.Sum(s => s.Principal)}, Interest: {invoices.Sum(s => s.Interest)}, InvoiceFee: {invoices.Sum(s => s.InvoiceFee)}, LateFee: {invoices.Sum(s => s.LateFee)}");
            TestOutput.CreateCSV(invoices, filename);
        }

    }
}
