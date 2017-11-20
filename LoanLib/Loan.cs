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
        public IDayCounter DayCounter { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<Payment> Payments { get; set; }

        public Loan(IDayCounter dayCounter)
        {
            this.DayCounter = dayCounter;
            Invoices = new List<Invoice>();
            Payments = new List<Payment>();
        }

        public virtual Invoice AddInvoice(DateTime invoiceDate, DateTime invoiceStartDate, DateTime invoiceEndDate, double invoiceFee)
        {
            var invoice = new Invoice();
            // Rate. Could probaply extract this some more.
            var yearFraction = this.DayCounter.GetYearFraction(invoiceStartDate, invoiceEndDate);
            var fractionRate = yearFraction * this.InterestRate / 100.0d;
            invoice.Interest = Math.Round(this.GetInterest(fractionRate), 2, MidpointRounding.AwayFromZero);

            var leftForPrincipal = this.GetPrincipal(invoice.Interest);
            invoice.Principal = (leftForPrincipal <= this.CurrentPrincipal) ? leftForPrincipal : this.CurrentPrincipal;
            invoice.Principal = Math.Round(invoice.Principal, 2, MidpointRounding.AwayFromZero);
            invoice.InvoiceDate = invoiceDate;
            invoice.InvoiceFee = invoiceFee;
            Invoices.Add(invoice);
            return invoice;
        }

        protected abstract double GetInterest(double fractionRate);
        protected abstract double GetPrincipal(double interest);

        public virtual Payment Pay(DateTime payDate, double paymentAmount)
        {
            Payment aggregatedPayment = new Payment();
            aggregatedPayment.PayDate = payDate;
            foreach (var invoice in this.Invoices.OrderBy(o => o.InvoiceDate))
            {
                var payment = invoice.Pay(payDate, paymentAmount);
                paymentAmount = payment.Reminder;
                aggregatedPayment += payment;
                this.CurrentPrincipal -= payment.Principal;
            }
            aggregatedPayment.Reminder = paymentAmount;
            this.CurrentPrincipal -= paymentAmount;

            // TODO: Here we can add things like overpayments.
            Payments.Add(aggregatedPayment);
            return aggregatedPayment;
        }

        public override string ToString()
        {
            return $"Type: {this.GetType().Name}, Current principal: {Math.Round(this.CurrentPrincipal, 2, MidpointRounding.AwayFromZero)}, Day counter: {DayCounter.GetType().Name}.";
        }
    }

    public class FixedEmiLoan : Loan
    {
        public FixedEmiLoan(IDayCounter dayCounter) : base(dayCounter)
        {
        }
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
            this._emi = Math.Round(this._emi, 2, MidpointRounding.AwayFromZero);
        }

        protected override double GetInterest(double fractionRate)
        {
            return fractionRate * this.CurrentPrincipal;
        }

        protected override double GetPrincipal(double interest)
        {
            return this.Emi - interest;
        }
    }

    public class FixedAmortizationLoan: Loan
    {
        public FixedAmortizationLoan(IDayCounter dayCounter) : base(dayCounter)
        {
        }
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
            this._monthlyPrincipal = this.StartAmount / (double)(TenureYears * 12);
            this._monthlyPrincipal = Math.Round(this._monthlyPrincipal, 2, MidpointRounding.AwayFromZero);
        }

        protected override double GetInterest(double fractionRate)
        {
            return fractionRate * this.CurrentPrincipal;
        }

        protected override double GetPrincipal(double interest)
        {
            return this.MontlyPrincipal;
        }
    }

    public class FixedInterestLoan : Loan
    {
        public FixedInterestLoan(IDayCounter dayCounter) : base(dayCounter)
        {
        }
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
            this._monthlyPrincipal = this.StartAmount / (double)(TenureYears * 12);
            this._monthlyPrincipal = Math.Round(this._monthlyPrincipal, 2, MidpointRounding.AwayFromZero);
        }

        protected override double GetInterest(double fractionRate)
        {
            return fractionRate * this.StartAmount;
        }

        protected override double GetPrincipal(double interest)
        {
            return this.MontlyPrincipal;
        }
    }

    public class FixedEmiCapitalizeLoan : Loan
    {
        public FixedEmiCapitalizeLoan(IDayCounter dayCounter) : base(dayCounter)
        {
        }
        private double _emi { get; set; }
        public double Emi
        {
            get
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
            this._emi = Math.Round(this._emi, 2, MidpointRounding.AwayFromZero);
        }

        public override Invoice AddInvoice(DateTime invoiceDate, DateTime invoiceStartDate, DateTime invoiceEndDate, double invoiceFee)
        {
            var lastInvoice = Invoices.OrderByDescending(o => o.InvoiceDate).FirstOrDefault();
            if(lastInvoice != null)
            {
                var paidInterest = Payments.Where(x => x.PayDate >= lastInvoice.InvoiceDate).Sum(s => s.Interest);
                this.CurrentPrincipal += (lastInvoice.Interest - paidInterest);
            }
            return base.AddInvoice(invoiceDate, invoiceStartDate, invoiceEndDate, invoiceFee);
        }

        protected override double GetInterest(double fractionRate)
        {
            return fractionRate * this.CurrentPrincipal;
        }

        protected override double GetPrincipal(double interest)
        {
            return this.Emi - interest;
        }
    }

}
