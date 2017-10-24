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
            TestPayingOnLoan();
        }

        static void TestPayingOnLoan()
        {
            var loan = new LoanLib.FixedRateFixedEmiLoan()
            {
                APR = 10,
                CurrentPrincipal = 10000,
                StartAmount = 10000,
                PayoutDate = new DateTime(2017, 01, 01),
                Tenure = 10,
            };

            var invoices = new List<LoanLib.Invoice>();
            var rateCalculator = new LoanLib.SimpleFlatInterest();
            var baseDate = loan.PayoutDate;
            for (int i = 0; i < 3; i++)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                var invoice = loan.AddInvoice(date, 0.0, rateCalculator);
                loan.CurrentPrincipal -= invoice.Principal;
                invoices.Add(invoice);

                Console.WriteLine($"{invoice.InvoiceDate.ToString("MMM-yyyy")} | {Math.Round(invoice.FullInvoiceAmount, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Interest, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Principal, 0, MidpointRounding.AwayFromZero)} | {Math.Round(loan.CurrentPrincipal, 0, MidpointRounding.AwayFromZero)}");
            }

            loan.Invoices = invoices;
            var payment = loan.Pay(new DateTime(2017, 03, 31), 5000.0);
            Console.WriteLine(payment.ToString());

        }

        static void TestCreatingPayments()
        {
            var loan = new LoanLib.FixedRateFixedEmiLoan()
            {
                APR = 10,
                CurrentPrincipal = 10000,
                StartAmount = 10000,
                PayoutDate = new DateTime(2017, 10, 01),
                Tenure = 10,
            };

            var invoices = new List<LoanLib.Invoice>();
            var rateCalculator = new LoanLib.SimpleFlatInterest();
            var baseDate = loan.PayoutDate;
            while (loan.CurrentPrincipal > 0.0)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                var invoice = loan.AddInvoice(date, 0.0, rateCalculator);
                loan.CurrentPrincipal -= invoice.Principal;
                invoices.Add(invoice);

                Console.WriteLine($"{invoice.InvoiceDate.ToString("MMM-yyyy")} | {Math.Round(invoice.FullInvoiceAmount, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Interest, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Principal, 0, MidpointRounding.AwayFromZero)} | {Math.Round(loan.CurrentPrincipal, 0, MidpointRounding.AwayFromZero)}");
            }
        }
    }
}
