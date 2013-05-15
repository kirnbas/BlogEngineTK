using BlogEngineTK.Domain;
using BlogEngineTK.WebUI.Infrastructure;
using BlogEngineTK.WebUI.Infrastructure.ModelBinders;
using BlogEngineTK.WebUI.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using elFinder.Connector;
using System.Collections;
using System.Web.Configuration;
using System.Configuration;
using elFinder.Connector.Config;

namespace BlogEngineTK.WebUI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Исп-ся для варьирования кэширования по авторизации (авторизован/нет)
        /// </summary>
        public const string VARYBYAUTH = "#isadmin";

        protected void Application_Start()
        {
            #region Standard

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            #endregion

            #region elFinder Connector

            // register IoC
            var builder = new ContainerBuilder();
            // add other registrations...
            // add elFinder connector registration
            builder.RegisterElFinderConnector();
            // create container
            var _container = builder.Build();
            // need also to set container in elFinder module
            _container.SetAsElFinderResolver();

            #endregion

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());
            BlogSettings.Current.ReadSettings();
            ModelBinders.Binders.Add(typeof(SessionStorage), new SessionStorageModelBinder());                        
        }

        /// <summary>
        /// Write to specified log information about unhandled exception in current app
        /// </summary>
        protected void Application_Error(object sender, EventArgs e)
        {
            Logger log = LogManager.GetLogger("UnhandledExceptions");
            Exception exception = Server.GetLastError();

            log.Error(string.Format("\tMessage: {0}{2}Exception type: {3}{2}StackTrace: {1}\n\n",
                exception.Message, exception.StackTrace, Environment.NewLine, exception.GetType()));

            Server.ClearError(); 
        }

        /// <summary>
        /// Для обработки кэширования при исп-нии [OutputCache(..., VaryByCustom = arg)]
        /// </summary>
        /// <returns>Возвращает ключ кэша в зависимости от условий</returns>
        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            string anon = "%.#ANONIMOUNS_=$%";

            if (arg == VARYBYAUTH && context.User.Identity.IsAuthenticated)
            {
                return context.User.Identity.Name; // для администратора возвращаем одни кэшированные страницы                
            }
            else
            {
                return anon; // для остальных пользователей возвращаем другие кэшированные страницы
            }
        }

        /// <summary>
        /// Очищает все кэшированные страницы (вызывается после создания, редактирования и удаления поста,
        /// изменения параметров блога)
        /// </summary>
        public static void ClearAppCache()
        {
            // Этот вариант не всегда работает в ASP.NET MVC
            //List<string> toRemove = new List<string>();
            //foreach (DictionaryEntry cache in HttpRuntime.Cache)
            //{
            //    toRemove.Add(cache.Key.ToString()); // Получаем все ключи в кэше
            //}

            //foreach (string key in toRemove)
            //{
            //    HttpRuntime.Cache.Remove(key); // Удаляем элементы кэша
            //}

            // Подходящий вариант для такого приложения, 
            // хотя выполняет больше, чем просто очистка кэша            
            HttpRuntime.Close();
        }
    }
}