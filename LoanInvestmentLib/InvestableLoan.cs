using LoanLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanInvestmentLib
{
    class InvestableLoan<T> where T : Loan, IPayable
    {
        private T Loan { get; set; }
        private List<Investment> Investments { get; set; }
        public double InvestedAmount { get; set; }
        public double LeftToInvest { get { return InvestedAmount - Loan.StartAmount; } }
        public bool IsFullyInvested { get { return Loan.StartAmount == InvestedAmount; } }

        public InvestableLoan(T loan)
        {
            Loan = loan;
        }

        /// <summary>
        /// Add a new investment to the loan.
        /// </summary>
        /// <param name="amount">Amoun to invest.</param>
        /// <returns>-1 if investment went ok. Else returns available left to invest in loan.</returns>
        public double AddInvestment(double amount, DateTime InvestmentDate)
        {
            if (amount < 0) throw new ArgumentException($"Amount cannot be zero or negative. Amount: {amount}");
            if(amount > LeftToInvest)
            {
                return LeftToInvest;
            }
            InvestedAmount += amount;
            var investment = new Investment(amount);
            Investments.Add(investment);
            if(IsFullyInvested)
            {
                Investments.ForEach(f => f.InvestmentDate = InvestmentDate);
            }
            return -1;
        }

        public Payment Pay(DateTime payDate, double paymentAmount)
        {
            var payment = Loan.Pay(payDate, paymentAmount);
            Investments.ForEach(e => e.Pay(payment, Loan.StartAmount));
            return payment;
        }
    }
}
