﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DietPlanning.Core.FoodPreferences;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.Web.Helpers;
using DietPlanning.Web.Models;
using MultiAttributeDecisionMaking;

namespace DietPlanning.Web.Controllers
{
  public class PeopleController : Controller
  {
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
      TempData.RemoveAhpModel(id);
      TempData.RemovePreferencePointModel(id);
      TempData.RemovePreferencesViewModel(id);
      TempData.ClearGroupDietsResultViewModel();

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
      personalData.Id = people.Any() ? people.Select(p => p.Id).Max() + 1 : 0;
      people.Add(personalData);
      TempData.SavePersonalDataList(people);
      TempData.ClearGroupDietsResultViewModel();

      return RedirectToAction("Preferences", new { id = personalData.Id});
    }

    [HttpGet]
    public ActionResult Preferences(int id)
    {
      var preferences = TempData.GetPreferencesViewModel(id);
      preferences.PersonId = id;
      preferences.PersonName = TempData.GetPersonalDataList().Single(p => p.Id == id).Name;

      return View(preferences);
    }

    [HttpPost]
    public ActionResult Preferences(PreferencesViewModel preferences)
    {
      UpdatePreferences(preferences);

      return new HttpStatusCodeResult(200);
    }


    [HttpGet]
    public ActionResult Pairwise(int id)
    {
      var ahpModel = TempData.GetAhpModel(id);
      ahpModel.PersonId = id;
      ahpModel.PersonName = TempData.GetPersonalDataList().Single(p => p.Id == id).Name;

      return View("Pairwise", ahpModel);
    }

    [HttpPost]
    public ActionResult Pairwise(AhpModel ahp)
    {
      TempData.SaveAhpModel(ahp, ahp.PersonId);

      return RedirectToAction("People");
    }

    [HttpGet]
    public ActionResult Point(int id)
    {
      var pointModel = TempData.GetPreferencePointModel(id);
      pointModel.PersonId = id;
      pointModel.PersonName = TempData.GetPersonalDataList().Single(p => p.Id == id).Name;

      return View("Point", pointModel);
    }

    [HttpPost]
    public ActionResult Point(WeightsModel weights)
    {
      TempData.SavePreferencePointModel(weights, weights.PersonId);

      return RedirectToAction("Pairwise", new { id = weights.PersonId } );
    }


    private void UpdatePreferences(PreferencesViewModel preferencesViewModel)
    {
      var personalData = TempData.GetPersonalDataList();
      var preferences = personalData.Single(p => p.Id == preferencesViewModel.PersonId).Preferences;
      preferences.CategoryPreferences.Clear();

      var oldPreferences = TempData.GetPreferencesViewModel(preferencesViewModel.PersonId);
      var oldMainPrefs = oldPreferences.MainCategoryPreferences;
      var oldSubPrefs = oldPreferences.MainCategoryPreferences.SelectMany(m => m.SubCategoryPreferences).ToList();

      foreach (var mainCategoryPreference in preferencesViewModel.MainCategoryPreferences)
      {
        List<SubCategoryPreferenceViewModel> subCategories;

        if (Math.Abs(mainCategoryPreference.Value - 1.0) > 0.01)
        {
          oldMainPrefs.Single(m => m.MainCategory == mainCategoryPreference.MainCategory).Value = mainCategoryPreference.Value;
          subCategories = mainCategoryPreference.SubCategoryPreferences;
        }
        else
        {
          subCategories = mainCategoryPreference.SubCategoryPreferences.Where(sub => Math.Abs(sub.Value - 1.0) > 0.01).ToList();
        }

        foreach (var subCategory in subCategories)
        {
          oldSubPrefs.Single(s => s.SubCategory == subCategory.SubCategory).Value = subCategory.Value;

          preferences.CategoryPreferences.Add(new CategoryPreference
          {
            Preference = subCategory.Value * mainCategoryPreference.Value,
            SubCategory = subCategory.SubCategory
          });
        }
      }

      TempData.SavePreferencesViewModel(oldPreferences, preferencesViewModel.PersonId);
      TempData.SavePersonalDataList(personalData);
    }
  }
}