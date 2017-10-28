using LoanLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            
            TestPayingOnLoan(new SimpleFlatInterest());
            TestPayingOnLoan(new Actual365());
            TestPayingOnLoan(new Actual360());
            TestPayingOnLoan(new Rate30360());
            TestCreatingInvoices(new SimpleFlatInterest());
            TestCreatingInvoices(new Actual365());
            TestCreatingInvoices(new Actual360());
            TestCreatingInvoices(new Rate30360());

        }

        static void TestPayingOnLoan(IInterestCalculator rateCalculator)
        {
            var loan = new FixedRateFixedEmiLoan()
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
                var invoice = loan.AddInvoice(date, 0.0, rateCalculator);
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

        static void TestCreatingInvoices(IInterestCalculator rateCalculator)
        {
            var loan = new FixedRateFixedEmiLoan()
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
                var invoice = loan.AddInvoice(date, 0.0, rateCalculator);
                loan.CurrentPrincipal -= invoice.Principal;
                invoices.Add(invoice);

                Console.WriteLine(invoice.ToString());
                //Console.WriteLine($"{invoice.InvoiceDate.ToString("MMM-yyyy")} | {Math.Round(invoice.FullInvoiceAmount, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Interest, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Principal, 0, MidpointRounding.AwayFromZero)} | {Math.Round(loan.CurrentPrincipal, 0, MidpointRounding.AwayFromZero)}");
            }
            Console.WriteLine($"SUM: Principal {invoices.Sum(s => s.Principal)}, Interest: {invoices.Sum(s => s.Interest)}, InvoiceFee: {invoices.Sum(s => s.InvoiceFee)}, LateFee: {invoices.Sum(s => s.LateFee)}");
        }
    }
}
