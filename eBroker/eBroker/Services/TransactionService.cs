using System.Collections.Generic;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using eBroker.DAL;

namespace eBroker.Services
{
    public static class TransactionService
    {
        public static MemoryStream TransactionToExcel(IEnumerable<Vw_Transaction> transactions)
        {
            var dt = new DataTable("Grid");
            dt.Columns.AddRange(new[]
            {
                new DataColumn("Date"),
                new DataColumn("Customer"),
                new DataColumn("Nationality"),
                new DataColumn("ID Number"),
                new DataColumn("Resident"),
                new DataColumn("Mobile"),
                new DataColumn("Ref Number"),
                new DataColumn("Serial Number"),
                new DataColumn("Type"),
                new DataColumn("Exchange Type"),
                new DataColumn("Currency"),
                new DataColumn("Amount"),
                new DataColumn("Rate"),
                new DataColumn("Rwf"),
                new DataColumn("Reversed"),
                new DataColumn("Branch"),
                new DataColumn("Served By")
            });
            foreach (var transaction in transactions)
            {
                dt.Rows.Add(
                    transaction.TranDate.ToString("yyyy MM dd"),
                    transaction.CustomerName,
                    transaction.CustomerNationality,
                    transaction.CustomerIDNumber,
                    transaction.IsResident ? "Yes" : "No",
                    transaction.Mobile,
                    transaction.ReferenceNo,
                    transaction.SerialNumber,
                    transaction.TranType,
                    transaction.GetTransactionType,
                    transaction.Currency,
                    transaction.Amount,
                    transaction.Rate,
                    transaction.RWF,
                    transaction.Reversed ? "Yes" : "No",
                    transaction.BranchName,
                    transaction.Employee);
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
        public static MemoryStream PolicyToExcel(IEnumerable<InsurancePolicy> transactions)
        {
            var dt = new DataTable("Grid");
            dt.Columns.AddRange(new[]
            {
                new DataColumn("contract_id"),
                new DataColumn("policy_no"),
                new DataColumn("policy_type"),
                new DataColumn("amendment_no"),
                new DataColumn("effective_dt"),
                new DataColumn("expiry_dt"),
                new DataColumn("net_premium"),
                new DataColumn("product_name"),
                new DataColumn("Insurer"),
                new DataColumn("client_name"),
                new DataColumn("invoice_status"),
                new DataColumn("commission_perc"),
                new DataColumn("renewable"),
                new DataColumn("language"),
                new DataColumn("mobile"),
                new DataColumn("total_paid"),
                new DataColumn("relationship_manager"),
                new DataColumn("interest_transfer"),
                new DataColumn("BankName"),
                new DataColumn("Renewed"),
                new DataColumn("renewal_policy_id"),
            });
            foreach (var transaction in transactions)
            {
                var t = transaction;
                var p = t.InsuranceProducts != null ? t.InsuranceProducts.product_name : "";
                var partner = t.Partners != null ? t.Partners.company_name : "";
                var client = t.Clients != null ? t.Clients.client_name : "";
                var lang = t.Clients != null ? t.Clients.language : "";
                var mob = t.Clients != null ? t.Clients.mobile : "";
                dt.Rows.Add(
                    transaction.Id,
                    transaction.policy_no,
                    transaction.policy_type,
                    transaction.amendment_no,
                    transaction.effective_dt.ToString("yyyy-MM-dd"),
                    transaction.expiry_dt.ToString("yyyy-MM-dd"),
                    transaction.net_premium,
                    p,
                    partner,
                    client,
                    transaction.invoice_status,
                    "",
                    transaction.renewable,
                    lang,
                    mob,
                    transaction.total_paid,
                    transaction.relationship_manager,
                    transaction.interest_transfer,
                    transaction.BankOptionalName,
                    transaction.renewed,
                    transaction.renewal_policy_id);
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

        public static MemoryStream GlBalanceToExcel(IEnumerable<VwLedgerBalance> gls)
        {
            var dt = new DataTable("Grid");
            dt.Columns.AddRange(new[]
            {
                new DataColumn("Legder No"),
                new DataColumn("Legder Desc"),
                new DataColumn("Currency"),
                new DataColumn("Balance")
            });


            foreach (var gl in gls)
            {
                dt.Rows.Add(
                    gl.LedgerNo,
                    gl.LedgerDescription,
                    gl.IsoCode,
                    gl.Balance
                );
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