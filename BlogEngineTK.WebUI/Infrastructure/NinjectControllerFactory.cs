using BlogEngineTK.Domain.Entities;
using BlogEngineTK.Domain.Repositories;
using BlogEngineTK.Domain.Services;
using BlogEngineTK.Domain.Services.Authorization;
using BlogEngineTK.WebUI.Controllers;
using Moq;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogEngineTK.WebUI.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory, IDisposable
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return (controllerType == null) ? null : (IController)ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {            
            ninjectKernel.Bind<IPostRepository>().To<EfPostRepository>();
            ninjectKernel.Bind<IPostService>().To<PostService>();
            ninjectKernel.Bind<IAuthProvider>().To<FormsAuthProvider>();            
        }

        public void Dispose()
        {
            DisposeManagedResources();
        }

        protected virtual void DisposeManagedResources()
        {
            ninjectKernel.Dispose();
            ninjectKernel = null;
        }
    }
}