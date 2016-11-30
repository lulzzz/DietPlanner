using System;
using System.Web.Mvc;
using Ninject;

namespace DietPlanning.Web.Controllers
{
  public class NinjectControllerFactory : DefaultControllerFactory
  {
    public IKernel Kernel { get; }

    public NinjectControllerFactory(IKernel kernel)
    {
      Kernel = kernel;
    }

    protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext,
      Type controllerType)
    {
      IController controller = null;

      if (controllerType != null)
        controller = (IController)Kernel.Get(controllerType);

      return controller;
    }
  }
}