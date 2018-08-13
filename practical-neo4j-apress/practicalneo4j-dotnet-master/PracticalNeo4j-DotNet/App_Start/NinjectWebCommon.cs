[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(PracticalNeo4j_DotNet.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(PracticalNeo4j_DotNet.App_Start.NinjectWebCommon), "Stop")]

namespace PracticalNeo4j_DotNet.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using PracticalNeo4j_DotNet.App_Start.Modules;
    using PracticalNeo4j_DotNet.Service;

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
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                kernel.Bind<GraphStoryInterface>().To<GraphStoryService>();
                kernel.Bind<GraphStory>().To<GraphStory>();
                kernel.Bind<ContentInterface>().To<ContentService>();
                kernel.Bind<LocationInterface>().To<LocationService>();
                kernel.Bind<ProductInterface>().To<ProductService>();
                kernel.Bind<PurchaseInterface>().To<PurchaseService>();
                kernel.Bind<TagInterface>().To<TagService>();
                kernel.Bind<UserInterface>().To<UserService>();
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load <Neo4jModule>();
        }        
    }
}
