using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanLib
{
    public abstract class BaseInvoiceProperties
    {
        public double Principal { get; set; }
        public double Interest { get; set; }
        public double InvoiceFee { get; set; }
        public double LateFee { get; set; }

        public override string ToString()
        {
            return $"Principal: {Math.Round(this.Principal, 2, MidpointRounding.AwayFromZero)}, Interest: {Math.Round(this.Interest, 2, MidpointRounding.AwayFromZero)}, InvoiceFee: {Math.Round(this.InvoiceFee, 2, MidpointRounding.AwayFromZero)}, LateFee: {Math.Round(this.LateFee, 2, MidpointRounding.AwayFromZero)}";
        }
    }

    public interface IInvoicable
    {
        Invoice AddInvoice(DateTime invoiceDate, DateTime invoiceStartDate, DateTime invoiceEndDate, double invoiceFee, IDayCounter dayCounter);
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

        public override string ToString()
        {
            return $"Invoice date: {this.InvoiceDate.ToString("yyyy-MM-dd")}, " + base.ToString() + $", Full Amount: {Math.Round(this.FullInvoiceAmount, 2, MidpointRounding.AwayFromZero)}";
        }
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
            return $"Pay date: {this.PayDate.ToString("yyyy-MM-dd")}, " + base.ToString() + $", Reminder: {Math.Round(this.Reminder, 2, MidpointRounding.AwayFromZero)}";
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
}
