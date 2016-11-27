using System.Web.Mvc;
using DietPlanning.Web.Helpers;

namespace DietPlanning.Web.Controllers
{
    public class TestsController : Controller
    {
        // GET: Tests
        public ActionResult NsgaLog()
        {
            return View(TempData.GetLog());
        }
    }
}