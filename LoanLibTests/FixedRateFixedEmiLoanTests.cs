using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoanLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanLib.Tests
{
    [TestClass()]
    public class FixedRateFixedEmiLoanTests
    {
        [TestMethod()]
        public void AddInvoiceTest()
        {
            var loan = new FixedRateFixedEmiLoan()
            {
                InterestRate = 10,
                CurrentPrincipal = 10000,
                StartAmount = 10000,
                PayoutDate = new DateTime(2017, 10, 01),
                Tenure = 10,
            };

            Invoice invoice = new Invoice();
            var rateCalculator = new Rate30360();
            var invoices = new List<Invoice>();
            var baseDate = loan.PayoutDate;
            while (loan.CurrentPrincipal > 0.0)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                invoice = loan.AddInvoice(date, 0.0, rateCalculator);
                loan.CurrentPrincipal -= invoice.Principal; // Fake payments on the loan.
                invoices.Add(invoice);
            }
            
            // Assert that the sum is correct.
            Assert.IsTrue(Math.Abs(invoices.Sum(s => s.Principal) - 10000) < 0.01d);
            Assert.IsTrue(Math.Abs(invoices.Sum(s => s.Interest) - 5858.08842581137) < 0.01d);

            Assert.IsTrue(Math.Abs(invoice.Principal - 131.06d) < 0.01d);
            Assert.IsTrue(Math.Abs(invoice.Interest - 1.09d) < 0.01d);
        }
    }
}