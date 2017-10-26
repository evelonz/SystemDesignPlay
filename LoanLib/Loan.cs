﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanLib
{
    

    public abstract class Loan : IInvoicable, IPayable
    {
        public int StartAmount { get; set; }
        public DateTime PayoutDate { get; set; }
        public double CurrentPrincipal { get; set; }
        public double APR { get; set; }
        public int Tenure { get; set; }
        public List<Invoice> Invoices { get; set; }

        public abstract Invoice AddInvoice(DateTime invoiceDate, double invoiceFee, IInterestCalculator interestCalculator);

        public abstract Payment Pay(DateTime payDate, double paymentAmount);
    }

    public class FixedRateFixedEmiLoan : Loan
    {
        private double _emi { get; set; }
        public double Emi { get
            {
                if (_emi == 0.0 && this.CurrentPrincipal > 0.0)
                    { SetEmi(); }
                return _emi;
            }
        }
        private void SetEmi()
        {
            var r = this.APR / 12.0 / 100.0;
            var n = this.Tenure * 12.0;
            var dividend = Math.Pow(1 + r, n);
            var divisor = dividend - 1;
            this._emi = this.StartAmount * r * (dividend / divisor);
        }
        public override Invoice AddInvoice(DateTime invoiceDate, double invoiceFee, IInterestCalculator interestCalculator)
        {
            var invoice = new Invoice();
            invoice.Interest = interestCalculator.GetMonthlyInterest(this.APR, this.CurrentPrincipal);
            var leftForPrincipal = this.Emi - invoice.Interest;
            invoice.Principal = (leftForPrincipal <= this.CurrentPrincipal) ? leftForPrincipal : this.CurrentPrincipal;
            invoice.InvoiceDate = invoiceDate;
            invoice.InvoiceFee = invoiceFee;
            return invoice;
        }

        public override Payment Pay(DateTime payDate, double paymentAmount)
        {
            Payment aggregatedPayment = new Payment();
            foreach (var invoice in this.Invoices.OrderBy(o => o.InvoiceDate))
            {
                var payment = invoice.Pay(payDate, paymentAmount);
                paymentAmount = payment.Reminder;
                aggregatedPayment += payment;
            }
            aggregatedPayment.Reminder = paymentAmount;
            return aggregatedPayment;
        }
    }

    public interface IInterestCalculator
    {
        double GetMonthlyInterest(double apr, double principal);
    }

    public class SimpleFlatInterest : IInterestCalculator
    {
        public double GetMonthlyInterest(double apr, double principal)
        {
            return ((apr / 12.0d) / 100.0) * principal;
        }
    }

}