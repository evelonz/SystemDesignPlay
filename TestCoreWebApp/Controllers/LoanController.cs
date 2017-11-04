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
            var loan = CreateLoan(Loantypes.FixedEmiLoan, tenure, 10, 10000, new DateTime(2017,10,01));
            var res = CreateLoanPaymentPlan(loan, new Thirty360Isda());
            return res;
        }

        [HttpGet("[action]")]
        public List<LoanPayment> Loan(Loantypes loanType, int tenure, double interestRate, int principal, DateTime payoutDate)
        {
            var loan = CreateLoan(loanType, tenure, interestRate, principal, payoutDate);
            var res = CreateLoanPaymentPlan(loan, new Thirty360Isda());
            return res;
        }

        public enum Loantypes : int
        {
            FixedEmiLoan = 1,
            FixedAmortizationLoan = 2,
            FixedInterestLoan = 3,
        }

        private static Loan CreateLoan(Loantypes loanType, int tenure, double interestRate, int principal, DateTime payoutDate)
        {
            Loan loan;
            switch (loanType)
            {
                case Loantypes.FixedEmiLoan:
                    loan = new FixedEmiLoan();
                    break;
                case Loantypes.FixedAmortizationLoan:
                    loan = new FixedAmortizationLoan();
                    break;
                case Loantypes.FixedInterestLoan:
                    loan = new FixedInterestLoan();
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

        private List<LoanPayment> CreateLoanPaymentPlan(Loan loan, IDayCounter dayCalculator)
        {
            var invoices = new List<LoanPayment>();
            var baseDate = loan.PayoutDate;
            while (loan.CurrentPrincipal > 0.0)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                var invoice = loan.AddInvoice(date, new DateTime(date.Year, date.Month, 1), baseDate, 0.0, dayCalculator);
                loan.CurrentPrincipal -= invoice.Principal;
                invoices.Add(new LoanPayment
                {
                    PeriodFormatted = invoice.InvoiceDate.ToString("yyyy-MM-dd"),
                    Principal = (int)Math.Round(invoice.Principal, 0, MidpointRounding.AwayFromZero),
                    Interest = (int)Math.Round(invoice.Interest, 0, MidpointRounding.AwayFromZero),
                });
            }
            return invoices;
        }

        public class LoanPayment
        {
            public string PeriodFormatted { get; set; }
            public int Principal { get; set; }
            public int Interest { get; set; }
            public int Total { get { return Principal + Interest; } }
        }
    }
}
