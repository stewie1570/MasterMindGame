[assembly: WebActivator.PreApplicationStartMethod(typeof(MasterMind.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(MasterMind.Web.App_Start.NinjectWebCommon), "Stop")]

namespace MasterMind.Web.App_Start
{
    using MasterMind.Core;
    using MasterMind.Core.ActualProviders;
    using MasterMind.Core.Models;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using System;
    using System.Web;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IGameProcess>().To<GameProcess>();

            kernel.Bind<Func<Context>>()
                .ToMethod(c => () => SessionSingleton("CurrentGameContext", () => new Context()));

            kernel.Bind<INumberGenerator>().To<RandomNumberGenerator>().InSingletonScope();

            kernel.Bind<IActualProvider>().To<RestrictedActualProvider>().InSingletonScope();
            
            kernel
                .Bind<Func<int, GuessColor[]>>()
                .ToMethod(c => width => kernel.Get<IActualProvider>().Create(pegCount: width, repeatLimit: 3));
        }


        #region Helpers

        private static T SessionSingleton<T>(string key, Func<T> newInstanceProvider) where T : class
        {
            if (HttpContext.Current.Session[key] as T == null)
                HttpContext.Current.Session[key] = newInstanceProvider();
            return HttpContext.Current.Session[key] as T;
        }

        #endregion
    }
}
