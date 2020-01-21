using System;
using System.ComponentModel.DataAnnotations;

namespace eBroker.DAL
{
    public class Reconciliation
    {
        public int Id { get; set; }
        public int BankAccountId { get; set; }
        public virtual BankAccount BankAccount { get; set; }

        [Display(Name = "Bank")]
        public int BankId { get; set; }

        public virtual Bank Bank { get; set; }

        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Ledger No.")]
        public int? LedgerNo { get; set; }

        [Display(Name = "Last Reconciliation Date")]
        public DateTime? LastReconDate { get; set; }

        [Display(Name = "Bank Statement Date")]
        public DateTime BankStatementDate { get; set; }

        [Display(Name = "Statement Amount")]
        public decimal StatementAmount { get; set; }
    }
}