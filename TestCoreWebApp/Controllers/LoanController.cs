using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoanLib;

namespace TestCoreWebApp.Controllers
{
    [Route("api/[controller]")]
    public class LoanController : Controller
    {
        [HttpGet("[action]")]
        public List<LoanPayment> LoanPayments(int tenure = 10)
        {
            var loan = CreateLoan(Loantypes.FixedEmiLoan, tenure, 10, 10000, new DateTime(2017,10,01), new Thirty360Isda());
            var res = CreateLoanPaymentPlan(loan);
            return res;
        }

        [HttpGet("[action]")]
        public List<LoanPayment> Loan(Loantypes loanType, int tenure, double interestRate, int principal, DateTime payoutDate)
        {
            var loan = CreateLoan(loanType, tenure, interestRate, principal, payoutDate, new Thirty360Isda());
            var res = CreateLoanPaymentPlan(loan);
            return res;
        }

        public enum Loantypes : int
        {
            FixedEmiLoan = 1,
            FixedAmortizationLoan = 2,
            FixedInterestLoan = 3,
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
                default: throw new ArgumentException("Loan type not implemented");
            };
            loan.TenureYears = tenure;
            loan.InterestRate = interestRate;
            loan.StartAmount = principal;
            loan.CurrentPrincipal = principal;
            loan.PayoutDate = payoutDate;

            return loan;
        }

        private List<LoanPayment> CreateLoanPaymentPlan(Loan loan)
        {
            // 4 year, 10 percentage, 16000 principal. Gives 0.05 left after 4 years. What is the worst outcome?
            // 1 year, 10 percentage, 10000 principal. Gives a ton of rounding errors. Have to be fixed with rounding in both setting rate, principal, and total.
            var invoices = new List<LoanPayment>();
            var baseDate = loan.PayoutDate;
            Invoice tmp = null;
            while (loan.CurrentPrincipal >= 0.01)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                var invoice = loan.AddInvoice(date, new DateTime(date.Year, date.Month, 1), baseDate, 0.0);
                loan.CurrentPrincipal -= invoice.Principal;
                invoices.Add(new LoanPayment
                {
                    PeriodFormatted = invoice.InvoiceDate.ToString("yyyy-MM-dd"),
                    Principal = invoice.Principal,
                    Interest = invoice.Interest,
                });
                tmp = invoice;
            }
            var q = invoices.Sum(s => s.Principal);
            var w = invoices.Sum(s => s.Interest);
            var e = tmp.Principal;
            var f = tmp.Interest;
            return invoices;
        }

        public class LoanPayment
        {
            public string PeriodFormatted { get; set; }
            public double Principal { get; set; }
            public double Interest { get; set; }
            public double Total { get { return Math.Round(Principal + Interest, 2, MidpointRounding.AwayFromZero); } }
        }
    }
}
