using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    static class TestOutput
    {
        public static void CreateCSV(List<LoanLib.Invoice> invoices, string filename)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Month\tPrincipal\tRate");
            foreach (var invoice in invoices)
            {
                sb.AppendLine($"{invoice.InvoiceDate.ToString("yyyy-MM-01")}\t{invoice.Principal.ToString("N2")}\t{invoice.Interest.ToString("N2")}");
            }
            var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + $"\\JunkFolder\\{filename}.csv";
            File.WriteAllText(path, sb.ToString());
        }
    }
}
