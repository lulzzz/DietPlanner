using System.Web.Mvc;
using DietPlanning.Web.Helpers;
using DietPlanning.Web.Models;

namespace DietPlanning.Web.Controllers
{
  public class SettingsController : Controller
  {
    [HttpGet]
    public ActionResult Edit()
    {
      return View("Settings", TempData.GetSettings());
    }

    [HttpPost]
    public ActionResult Edit(SettingsViewModel settings)
    {
      TempData.SaveSettings(settings);

      return View("Settings");
    }
  }
}