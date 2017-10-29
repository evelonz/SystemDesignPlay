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
    public class InterestHelperTests
    {
        [TestMethod()]
        public void DaysTest()
        {
            var test = InterestHelper.Days(new DateTime(2017, 10, 15), new DateTime(2017, 11, 15));
            Assert.AreEqual<int>(test, 31, "One month");

            test = InterestHelper.Days(new DateTime(2017, 10, 15, 23, 59, 59), new DateTime(2017, 11, 15));
            Assert.AreEqual<int>(test, 31, "One month with time");

            test = InterestHelper.Days(new DateTime(2017, 10, 15), new DateTime(2017, 10, 15));
            Assert.AreEqual<int>(test, 0, "Same day");

            test = InterestHelper.Days(new DateTime(2017, 11, 15), new DateTime(2017, 10, 15));
            Assert.AreEqual<int>(test, -31, "Negative one month");
        }
    }
}