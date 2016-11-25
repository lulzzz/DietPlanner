using System.Web.Mvc;
using DietPlanning.NSGA;
using DietPlanning.Web.Models;

namespace DietPlanning.Web.Controllers
{
  public class SettingsController : Controller
  {
    const string SettingsKey = "Settings";

    [HttpGet]
    public ActionResult Edit()
    {
      SettingsViewModel settingsViewModel;

      if (TempData.ContainsKey(SettingsKey))
      {
        settingsViewModel = TempData.Peek(SettingsKey) as SettingsViewModel;
      }
      else
      {
        var configurationProvider = new ConfigurationProvider();
        settingsViewModel = new SettingsViewModel { NsgaConfiguration = configurationProvider.GetConfiguration()};
        TempData.Add(SettingsKey, settingsViewModel);
      }

      return View("Settings", settingsViewModel);
    }

    [HttpPost]
    public ActionResult Edit(SettingsViewModel settings)
    {
      TempData[SettingsKey] = settings;

      return View("Settings");
    }
  }
}