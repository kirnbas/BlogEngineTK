using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogEngineTK.WebUI.Infrastructure.ModelBinders
{
    public class SessionStorageModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return SessionStorage.Current;
        }
    }
}