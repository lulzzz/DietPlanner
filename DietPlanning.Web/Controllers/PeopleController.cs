using System.Linq;
using System.Web.Mvc;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.Web.Helpers;

namespace DietPlanning.Web.Controllers
{
  public class PeopleController : Controller
  {
    // GET: People
    public ActionResult People()
    {
      return View(TempData.GetPersonalDataList());
    }

    [HttpGet]
    public ActionResult Edit(int id)
    {
      return View("PersonalData", TempData.GetPersonalDataList().Single(p => p.Id == id));
    }

    [HttpPost]
    public ActionResult Edit(PersonalData personalData)
    {
      var people = TempData.GetPersonalDataList();
      people.Remove(people.Single(p => p.Id == personalData.Id));
      people.Add(personalData);
      TempData.SavePersonalDataList(people);

      return RedirectToAction("People");
    }
    
    public ActionResult Delete(int id)
    {
      var people = TempData.GetPersonalDataList();
      people.Remove(people.Single(p => p.Id == id));
      TempData.SavePersonalDataList(people);

      return RedirectToAction("People");
    }

    [HttpGet]
    public ActionResult Create()
    {
      return View("PersonalData", new PersonalData());
    }

    [HttpPost]
    public ActionResult Create(PersonalData personalData)
    {
      var people = TempData.GetPersonalDataList();
      personalData.Id = people.Select(p => p.Id).Max() + 1;
      people.Add(personalData);
      TempData.SavePersonalDataList(people);

      return RedirectToAction("People");
    }
  }
}