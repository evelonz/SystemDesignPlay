using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanLib
{
    public interface IInvoicable
    {
        Invoice AddInvoice(DateTime invoiceDate, double invoiceFee, IInterestCalculator interestCalculator);
    }

    public interface IPayable
    {
        Payment Pay(DateTime payDate, double paymentAmount);
    }

    public class Payment : BaseInvoiceProperties
    {
        public DateTime PayDate { get; set; }
        /// <summary>
        /// If the payment amount is larger then what can be placed, the remainging amount is placed here.
        /// </summary>
        public double Reminder { get; set; }

        public override string ToString()
        {
            return base.ToString() + $", Reminder: {Math.Round(this.Reminder, 0, MidpointRounding.AwayFromZero)}";
        }

        /// <summary>
        /// Add two payments, but not their remainging amount.
        /// </summary>
        public static Payment operator +(Payment a, Payment b)
        {
            var payment = new Payment();
            payment.Principal = a.Principal + b.Principal;
            payment.Interest = a.Interest + b.Interest;
            payment.InvoiceFee = a.InvoiceFee + b.InvoiceFee;
            payment.LateFee = a.LateFee + b.LateFee;
            return payment;
        }
    }

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

    public class Invoice : BaseInvoiceProperties, IPayable
    {
        public DateTime InvoiceDate { get; set; }
        public double FullInvoiceAmount { get { return Principal + Interest + InvoiceFee + LateFee; } }
        
        public Payment Pay(DateTime payDate, double paymentAmount)
        {
            var payment = new Payment();
            payment.PayDate = payDate;
            // TODO: Perhaps implement some sort of injectable to pick payback order. Should they traverse multiple invoices?
            // Order: LateFee -> InvoiceFee -> Interest -> Principal.
            payment.LateFee = Math.Min(this.LateFee, paymentAmount);
            paymentAmount -= payment.LateFee;

            payment.InvoiceFee = Math.Min(this.InvoiceFee, paymentAmount);
            paymentAmount -= payment.InvoiceFee;

            payment.Interest = Math.Min(this.Interest, paymentAmount);
            paymentAmount -= payment.Interest;

            payment.Principal = Math.Min(this.Principal, paymentAmount);
            paymentAmount -= payment.Principal;

            payment.Reminder = paymentAmount;
            return payment;
        }
    }

    public abstract class BaseInvoiceProperties
    {
        public double Principal { get; set; }
        public double Interest { get; set; }
        public double InvoiceFee { get; set; }
        public double LateFee { get; set; }

        public override string ToString()
        {
            return $"Principal: {Math.Round(this.Principal, 0, MidpointRounding.AwayFromZero)}, Interest: {Math.Round(this.Interest, 0, MidpointRounding.AwayFromZero)}, InvoiceFee: {Math.Round(this.InvoiceFee, 0, MidpointRounding.AwayFromZero)}, LateFee: {Math.Round(this.LateFee, 0, MidpointRounding.AwayFromZero)}";
        }
    }

}
