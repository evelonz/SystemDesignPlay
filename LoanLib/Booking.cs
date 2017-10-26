using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanLib
{
    public class BookCompany
    {
    }

    public class Book
    {
        public int Year { get; set; }
        public List<Voucher> Vouchers { get; set; }
    }

    public enum DebitCredit
    {
        Debit = 0,
        Credit = 1
    }

    public class Voucher
    {
        public DateTime _voucherDate { get; set; }
        public DateTime VoucherDate { get { return _voucherDate.Date; } set { _voucherDate = value.Date; } }
        public List<VoucherRow> VoucherRows { get; set; }
    }

    public class VoucherRow
    {
        public Account Account { get; set; }
        public DebitCredit Direction { get; set; }
        public double Amount { get; set; }
    }

    public class Account
    {
        public int AccountID { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
    }

    public interface IBookable
    {
        // TODO: Need to get the booking definitions here as well. Perhaps as an injectable?
        // The idea is to use this on BaseInvoiceProperties objects, but setting that as a parameter feels wrong.
        // At the same time we can't (don't want to) force this to only be implemented on BaseInvoiceProperties.
        Voucher Book();
    }
}
