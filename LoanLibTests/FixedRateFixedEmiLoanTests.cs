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
            var loan = new FixedEmiLoan()
            {
                InterestRate = 10,
                CurrentPrincipal = 10000,
                StartAmount = 10000,
                PayoutDate = new DateTime(2017, 10, 01),
                TenureYears = 10,
            };

            Invoice invoice = new Invoice();
            var dayCalculator = new Thirty360Isda();
            var invoices = new List<Invoice>();
            var baseDate = loan.PayoutDate;
            while (loan.CurrentPrincipal >= 0.01)
            {
                baseDate = baseDate.AddMonths(1);
                var date = baseDate.AddDays(-1);
                invoice = loan.AddInvoice(date, new DateTime(date.Year, date.Month, 1), baseDate, 0.0, dayCalculator);
                loan.CurrentPrincipal -= invoice.Principal; // Fake payments on the loan.
                invoices.Add(invoice);
            }

            // Assert that the sum is correct.
            var sumPrincipal = invoices.Sum(s => s.Principal);
            Assert.IsTrue(Math.Abs(sumPrincipal - 10000) <= 0.01d, $"Principal: Expected less than 0.01, found {sumPrincipal}");
            var sumInterest = invoices.Sum(s => s.Interest);
            Assert.IsTrue(Math.Abs(invoices.Sum(s => s.Interest) - 5858.1799999999994) <= 0.01d, $"Interest: Expected 5858.179, found {sumInterest}");

            Assert.IsTrue(Math.Abs(invoice.Principal - 0.18d) < 0.01d, $"Last invoice principal: Expected 0.180, found {invoice.Principal}.");
            Assert.IsTrue(Math.Abs(invoice.Interest - 0.00d) < 0.01d, $"Last invoice Interest: Expected 0.000, found {invoice.Interest}.");
        }
    }
}