using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanLib;

namespace LoanInvestmentLib
{
    public class Investment
    {
        private double InvestedAmount { get; set; }
        private double GainedInterest { get; set; }
        private double ReturnedPrincipal { get; set; }
        public DateTime InvestmentDate { get; set; }
        public List<Payment> Payments { get; set; }

        public Investment(double amount)
        {
            InvestedAmount = amount;
            Payments = new List<Payment>();
        }

        public double LoanPercantage(double loanAmount)
        {
            return this.InvestedAmount / loanAmount;
        }

        public void Pay(Payment payment, double loanAmount)
        {
            Payments.Add(payment);
            var loanPercentage = LoanPercantage(loanAmount);
            GainedInterest += payment.Interest * loanPercentage;
            ReturnedPrincipal += payment.Principal * loanPercentage;
        }

    }
}
