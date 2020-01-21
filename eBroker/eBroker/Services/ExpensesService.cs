using System.Collections.Generic;
using System.Data;
using System.IO;
using ClosedXML.Excel;

namespace eBroker.Services
{
    public static class ExpensesService
    {
        public static  MemoryStream GetExcelData(IEnumerable<Vw_Expense_Voucher> expenseVouchers)
        {
            var dt = new DataTable("Grid");
            dt.Columns.AddRange(new[]
            {
                new DataColumn("Date"),
                new DataColumn("Expense Ledger"),
                new DataColumn("Payment Ledger"),
                new DataColumn("Currency"),
                new DataColumn("Amount"),
                new DataColumn("Reversed"),
                new DataColumn("Description"),
                new DataColumn("Branch")
            });
            

            foreach (var expenseVoucher in expenseVouchers)
            {
                dt.Rows.Add(
                    expenseVoucher.ExpenseDate.ToString("yyyy MM dd"),
                    expenseVoucher.ExpenseLedger,
                    expenseVoucher.PaymentLedger,
                    expenseVoucher.IsoCode,
                    expenseVoucher.Amount,
                    expenseVoucher.Reversal ? "Yes" : "No",
                    expenseVoucher.Description);
            }
            using (var wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return stream;
                }
            }
        }
    }
}