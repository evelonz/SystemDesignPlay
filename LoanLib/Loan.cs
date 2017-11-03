using System;
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
        public double InterestRate { get; set; }
        public int TenureYears { get; set; }
        public List<Invoice> Invoices { get; set; }

        public abstract Invoice AddInvoice(DateTime invoiceDate, DateTime invoiceStartDate, DateTime invoiceEndDate, double invoiceFee, IDayCounter dayCounter);

        public abstract Payment Pay(DateTime payDate, double paymentAmount);
    }

    public class FixedEmiLoan : Loan
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
            var r = this.InterestRate / 12.0 / 100.0;
            var n = this.TenureYears * 12.0;
            var dividend = Math.Pow(1 + r, n);
            var divisor = dividend - 1;
            this._emi = this.StartAmount * r * (dividend / divisor);
        }
        public override Invoice AddInvoice(DateTime invoiceDate, DateTime invoiceStartDate, DateTime invoiceEndDate, double invoiceFee, IDayCounter dayCounter)
        {
            var invoice = new Invoice();
            // Rate. Could probaply extract this some more.
            var yearFraction = dayCounter.GetYearFraction(invoiceStartDate, invoiceEndDate);
            var fractionRate = yearFraction * this.InterestRate / 100.0d;
            invoice.Interest = fractionRate * this.CurrentPrincipal;

            var leftForPrincipal = this.Emi - invoice.Interest;
            invoice.Principal = (leftForPrincipal <= this.CurrentPrincipal) ? leftForPrincipal : this.CurrentPrincipal;
            invoice.InvoiceDate = invoiceDate;
            invoice.InvoiceFee = invoiceFee;
            return invoice;
        }

        public override Payment Pay(DateTime payDate, double paymentAmount)
        {
            Payment aggregatedPayment = new Payment();
            aggregatedPayment.PayDate = payDate;
            foreach (var invoice in this.Invoices.OrderBy(o => o.InvoiceDate))
            {
                var payment = invoice.Pay(payDate, paymentAmount);
                paymentAmount = payment.Reminder;
                aggregatedPayment += payment;
            }
            aggregatedPayment.Reminder = paymentAmount;
            
            // TODO: Here we can add things like overpayments.
            return aggregatedPayment;
        }
    }


    public class FixedAmortizationLoan: Loan
    {
        public FixedAmortizationLoan(double amortization) : base()
        {
            _amortization = amortization;
        }
        private double _amortization { get; set; }
        public double Amortization { get { return _amortization; } }
        
        public override Invoice AddInvoice(DateTime invoiceDate, DateTime invoiceStartDate, DateTime invoiceEndDate, double invoiceFee, IDayCounter dayCounter)
        {
            var invoice = new Invoice();
            // Rate. Could probaply extract this some more.
            var yearFraction = dayCounter.GetYearFraction(invoiceStartDate, invoiceEndDate);
            var fractionRate = yearFraction * this.InterestRate / 100.0d;
            invoice.Interest = fractionRate * this.CurrentPrincipal;

            var leftForPrincipal = this.Amortization;
            invoice.Principal = (leftForPrincipal <= this.CurrentPrincipal) ? leftForPrincipal : this.CurrentPrincipal;
            invoice.InvoiceDate = invoiceDate;
            invoice.InvoiceFee = invoiceFee;
            return invoice;
        }

        public override Payment Pay(DateTime payDate, double paymentAmount)
        {
            Payment aggregatedPayment = new Payment();
            aggregatedPayment.PayDate = payDate;
            foreach (var invoice in this.Invoices.OrderBy(o => o.InvoiceDate))
            {
                var payment = invoice.Pay(payDate, paymentAmount);
                paymentAmount = payment.Reminder;
                aggregatedPayment += payment;
            }
            aggregatedPayment.Reminder = paymentAmount;

            // TODO: Here we can add things like overpayments.
            return aggregatedPayment;
        }
    }

    public class FixedInterestLoan : Loan
    {

        private double _monthlyPrincipal { get; set; }
        public double MontlyPrincipal
        {
            get
            {
                if (_monthlyPrincipal == 0.0 && this.CurrentPrincipal > 0.0)
                { SetMonthlyPrincipal(); }
                return _monthlyPrincipal;
            }
        }
        private void SetMonthlyPrincipal()
        {
            this._monthlyPrincipal = this.StartAmount / (TenureYears * 12);
        }
        public override Invoice AddInvoice(DateTime invoiceDate, DateTime invoiceStartDate, DateTime invoiceEndDate, double invoiceFee, IDayCounter dayCounter)
        {
            var invoice = new Invoice();
            // Rate. Could probaply extract this some more.
            var yearFraction = dayCounter.GetYearFraction(invoiceStartDate, invoiceEndDate);
            var fractionRate = yearFraction * this.InterestRate / 100.0d;
            invoice.Interest = fractionRate * this.StartAmount;

            var leftForPrincipal = this.MontlyPrincipal;
            invoice.Principal = (leftForPrincipal <= this.CurrentPrincipal) ? leftForPrincipal : this.CurrentPrincipal;
            invoice.InvoiceDate = invoiceDate;
            invoice.InvoiceFee = invoiceFee;
            return invoice;
        }

        public override Payment Pay(DateTime payDate, double paymentAmount)
        {
            Payment aggregatedPayment = new Payment();
            aggregatedPayment.PayDate = payDate;
            foreach (var invoice in this.Invoices.OrderBy(o => o.InvoiceDate))
            {
                var payment = invoice.Pay(payDate, paymentAmount);
                paymentAmount = payment.Reminder;
                aggregatedPayment += payment;
            }
            aggregatedPayment.Reminder = paymentAmount;

            // TODO: Here we can add things like overpayments.
            return aggregatedPayment;
        }
    }
}
