using System.Configuration;
using System.Linq;
using System.Web.Http;
namespace eBroker.Controllers
{
    public class CustomerApiController : ApiController
    {
        eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
        [HttpGet]
        public IHttpActionResult CustomerInfoJson(int id = 0)
        {

            var data = _dc.Client.FirstOrDefault(x => x.Id == id);
            
            return Json(data);
        }
    }
}