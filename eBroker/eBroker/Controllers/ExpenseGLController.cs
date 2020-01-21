using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace eBroker.Controllers
{
    public class ExpenseGLController : BaseController
    {
        //
        // GET: /ExpenseCategory/
        private readonly eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetExpenseGLs(int Id)
        {
            var expenseCat = _dc.GL_Account.FirstOrDefault(x => x.LedgerNo== Id);
            var expenseGLs = (from r in _dc.GL_Account.Where(x => x.ParentLedgerNo == expenseCat.LedgerNo).ToList() select new SelectListItem { Text = r.LedgerDescription, Value = r.LedgerNo.ToString() }).ToList();
            return Json(new SelectList(expenseGLs, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        
       [AcceptVerbs(HttpVerbs.Get)]
       public ActionResult GetLedger(int ledgerNo)
       {
           var debitLedger = (from e in _dc.GL_Account 
                               where e.ParentLedgerNo == ledgerNo
                              select new SelectListItem { Text = e.LedgerDescription, Value = e.LedgerNo.ToString() });
          
           return Json(new SelectList(debitLedger, "Value", "Text"), JsonRequestBehavior.AllowGet);

                   
       }

       //[AcceptVerbs(HttpVerbs.Get)]
       //public ActionResult GetCreditLedger(int Id)
       //{
       //    var creditLedger = (from e in dc.ManualEntry
       //                       join c in dc.GL_Account on e.CreditLedgerNo equals c.LedgerNo
       //                       where e.Id == c.LedgerNo
       //                       select new SelectListItem { Text = c.LedgerDescription, Value = e.CreditLedgerNo.ToString() });

       //    return Json(new SelectList(creditLedger, "Value", "Text"), JsonRequestBehavior.AllowGet);


       //}

    }
}
