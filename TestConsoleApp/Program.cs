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
            TestLoanEvents(CreateLoan(Loantypes.FixedEmiLoan, 10, 10, 10000, new DateTime(2017, 10, 01), new Thirty360Psa()), "");
            TestLoanEvents(CreateLoan(Loantypes.FixedEmiCapitalizeLoan, 10, 10, 10000, new DateTime(2017, 10, 01), new Thirty360Psa()), "");
            //TestPayingOnLoan(new Thirty360Isda());
            //TestCreatingInvoices(new Actual365F(), "Actual365F");
            //TestCreatingInvoices(new Thirty360Psa(), "Thirty360Psa");
            //TestCreatingInvoices(new Thirty360Isda(), "Thirty360Isda");
            TestCreatingInvoices2(new Thirty360Isda(), "FlatAThirty360Isda");
            TestCreatingInvoices3(new Thirty360Isda(), "FlatInterstThirty360Isda");
        }

        public enum Loantypes : int
        {
            FixedEmiLoan = 1,
            FixedAmortizationLoan = 2,
            FixedInterestLoan = 3,
            FixedEmiCapitalizeLoan = 4,
        }

        private static Loan CreateLoan(Loantypes loanType, int tenure, double interestRate, int principal, DateTime payoutDate, IDayCounter dayCalculator)
        {
            Loan loan;
            switch (loanType)
            {
                case Loantypes.FixedEmiLoan:
                    loan = new FixedEmiLoan(dayCalculator);
                    break;
                case Loantypes.FixedAmortizationLoan:
                    loan = new FixedAmortizationLoan(dayCalculator);
                    break;
                case Loantypes.FixedInterestLoan:
                    loan = new FixedInterestLoan(dayCalculator);
                    break;
                case Loantypes.FixedEmiCapitalizeLoan:
                    loan = new FixedEmiCapitalizeLoan(dayCalculator);
                    break;
                default: throw new ArgumentException("Loan type not implemented");
            };
            loan.TenureYears = tenure;
            loan.InterestRate = interestRate;
            loan.StartAmount = principal;
            loan.CurrentPrincipal = principal;
            loan.PayoutDate = payoutDate;

            return loan;
        }

        static void TestLoanEvents(Loan loan, string filename)
        {
            var invoiceDate = new DateTime(2017, 01, 01);
            var events = new List<LoanEvent>()
            {
                new LoanEvent() { Type = LoanEvent.EventTypes.Invoice, Amount = 0.0d, EventTime = invoiceDate },
                new LoanEvent() { Type = LoanEvent.EventTypes.Invoice, Amount = 0.0d, EventTime = invoiceDate.AddMonths(1) },
                new LoanEvent() { Type = LoanEvent.EventTypes.Invoice, Amount = 0.0d, EventTime = invoiceDate.AddMonths(2) },
                //new LoanEvent() { Type = LoanEvent.EventTypes.Payment, Amount = 5000.0d, EventTime = new DateTime(2017, 03, 31)}
            };

            LoanEventRunner.RunEvents(loan, events);

            var invoices = loan.Invoices;
            Console.WriteLine($"SUM: Principal {invoices.Sum(s => s.Principal)}, Interest: {invoices.Sum(s => s.Interest)}, InvoiceFee: {invoices.Sum(s => s.InvoiceFee)}, LateFee: {invoices.Sum(s => s.LateFee)}");
            Console.WriteLine(loan.ToString());

            //TestOutput.CreateCSV(loan.Invoices, filename);
        }

        static void TestPayingOnLoan(IDayCounter dayCalculator)
        {
            var loan = new FixedEmiLoan(dayCalculator)
            {
                InterestRate = 10,
                CurrentPrincipal = 10000,
                StartAmount = 10000,
                PayoutDate = new DateTime(2017, 01, 01),
                TenureYears = 10,
            };

            var invoices = new List<Invoice>();
            var baseDate = loan.PayoutDate;
            for (int i = 0; i < 3; i++)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                var invoice = loan.AddInvoice(date, new DateTime(date.Year, date.Month, 1), baseDate, 0.0);
                //loan.CurrentPrincipal -= invoice.Principal;
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
            var loan = new FixedEmiLoan(dayCalculator)
            {
                InterestRate = 10,
                CurrentPrincipal = 10000,
                StartAmount = 10000,
                PayoutDate = new DateTime(2017, 10, 01),
                TenureYears = 10,
            };

            var invoices = new List<Invoice>();
            var baseDate = loan.PayoutDate;
            while (loan.CurrentPrincipal > 0.0)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                var invoice = loan.AddInvoice(date, new DateTime(date.Year, date.Month, 1), baseDate, 0.0);
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
            var loan = new FixedAmortizationLoan(dayCalculator)
            {
                InterestRate = 10,
                CurrentPrincipal = 10000,
                StartAmount = 10000,
                PayoutDate = new DateTime(2017, 10, 01),
                TenureYears = 10,
            };

            var invoices = new List<Invoice>();
            var baseDate = loan.PayoutDate;
            while (loan.CurrentPrincipal > 0.0)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                var invoice = loan.AddInvoice(date, new DateTime(date.Year, date.Month, 1), baseDate, 0.0);
                loan.CurrentPrincipal -= invoice.Principal;
                invoices.Add(invoice);

                //Console.WriteLine(invoice.ToString());
                //Console.WriteLine($"{invoice.InvoiceDate.ToString("MMM-yyyy")} | {Math.Round(invoice.FullInvoiceAmount, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Interest, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Principal, 0, MidpointRounding.AwayFromZero)} | {Math.Round(loan.CurrentPrincipal, 0, MidpointRounding.AwayFromZero)}");
            }
            Console.WriteLine($"SUM: Principal {invoices.Sum(s => s.Principal)}, Interest: {invoices.Sum(s => s.Interest)}, InvoiceFee: {invoices.Sum(s => s.InvoiceFee)}, LateFee: {invoices.Sum(s => s.LateFee)}");
            TestOutput.CreateCSV(invoices, filename);
        }

        static void TestCreatingInvoices3(IDayCounter dayCalculator, string filename)
        {
            var loan = new FixedInterestLoan(dayCalculator)
            {
                InterestRate = 10,
                CurrentPrincipal = 10000,
                StartAmount = 10000,
                PayoutDate = new DateTime(2017, 10, 01),
                TenureYears = 10,
            };

            var invoices = new List<Invoice>();
            var baseDate = loan.PayoutDate;
            while (loan.CurrentPrincipal > 0.0)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                var invoice = loan.AddInvoice(date, new DateTime(date.Year, date.Month, 1), baseDate, 0.0);
                loan.CurrentPrincipal -= invoice.Principal;
                invoices.Add(invoice);

                //Console.WriteLine(invoice.ToString());
                //Console.WriteLine($"{invoice.InvoiceDate.ToString("MMM-yyyy")} | {Math.Round(invoice.FullInvoiceAmount, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Interest, 0, MidpointRounding.AwayFromZero)} | {Math.Round(invoice.Principal, 0, MidpointRounding.AwayFromZero)} | {Math.Round(loan.CurrentPrincipal, 0, MidpointRounding.AwayFromZero)}");
            }
            Console.WriteLine($"SUM: Principal {invoices.Sum(s => s.Principal)}, Interest: {invoices.Sum(s => s.Interest)}, InvoiceFee: {invoices.Sum(s => s.InvoiceFee)}, LateFee: {invoices.Sum(s => s.LateFee)}");
            TestOutput.CreateCSV(invoices, filename);
        }

    }

    class LoanEventRunner
    {
        public static void RunEvents(Loan loan, List<LoanEvent> events)
        {
            foreach (var loanEvent in events.OrderBy(o => o.EventTime))
            {
                switch (loanEvent.Type)
                {
                    case LoanEvent.EventTypes.Invoice:
                        InvoiceEvent(loan, loanEvent);
                        break;
                    case LoanEvent.EventTypes.Payment:
                        PayEvent(loan, loanEvent);
                        break;
                    default:
                        break;
                }
            }
        }

        private static void InvoiceEvent(Loan loan, LoanEvent loanEvent)
        {
            var firstDayOfMonth = new DateTime(loanEvent.EventTime.Year, loanEvent.EventTime.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);
            loan.AddInvoice(loanEvent.EventTime, firstDayOfMonth, firstDayOfNextMonth, loanEvent.Amount);
        }

        private static void PayEvent(Loan loan, LoanEvent loanEvent)
        {
            var payment = loan.Pay(loanEvent.EventTime, loanEvent.Amount);
            Console.WriteLine(payment.ToString());
        }
    }

    class LoanEvent
    {
        public EventTypes Type { get; set; }
        public DateTime EventTime { get; set; }
        public double Amount { get; set; }

        public enum EventTypes: int
        {
            Invoice = 1,
            Payment = 2
        }
    }
}
