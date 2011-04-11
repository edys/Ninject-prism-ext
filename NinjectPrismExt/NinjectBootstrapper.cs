using System;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.NinjectExtensions;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using Prism.NinjectExtension.Properties;

namespace Prism.NinjectExtension
{
    public abstract class NinjectBootstrapper : Bootstrapper   
    {
      
        private bool useDefaultConfiguration = true;

        /// <summary>
        /// Gets the default <see cref="Ninject.IKernel"/> for the application.
        /// </summary>
        /// <value>The default <see cref="Ninject.IKernel"/> instance.</value>
        [CLSCompliant(false)]
        public IKernel Kernel { get; protected set; }


        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        /// <param name="runWithDefaultConfiguration">If <see langword="true"/>, registers default Composite Application Library services in the container. This is the default behavior.</param>
        public override void Run(bool runWithDefaultConfiguration)
        {
            this.useDefaultConfiguration = runWithDefaultConfiguration;

            this.Logger = this.CreateLogger();
            if (this.Logger == null)
            {
                throw new InvalidOperationException(Resources.NullLoggerFacadeException);
            }

            this.Logger.Log(Resources.LoggerCreatedSuccessfully, Category.Debug, Priority.Low);

            this.Logger.Log(Resources.CreatingModuleCatalog, Category.Debug, Priority.Low);
            this.ModuleCatalog = this.CreateModuleCatalog();
            if (this.ModuleCatalog == null)
            {
                throw new InvalidOperationException(Resources.NullModuleCatalogException);
            }

            this.Logger.Log(Resources.ConfiguringModuleCatalog, Category.Debug, Priority.Low);
            this.ConfigureModuleCatalog();

            this.Logger.Log(Resources.CreatingKernel, Category.Debug, Priority.Low);
            this.Kernel = this.CreateKernel();
            if (this.Kernel == null)
            {
                throw new InvalidOperationException(Resources.NullKernelException);
            }

            this.Logger.Log(Resources.ConfiguringKernel, Category.Debug, Priority.Low);
            this.ConfigureContainer();

            this.Logger.Log(Resources.ConfiguringServiceLocatorSingleton, Category.Debug, Priority.Low);
            this.ConfigureServiceLocator();

            this.Logger.Log(Resources.ConfiguringRegionAdapters, Category.Debug, Priority.Low);
            this.ConfigureRegionAdapterMappings();

            this.Logger.Log(Resources.ConfiguringDefaultRegionBehaviors, Category.Debug, Priority.Low);
            this.ConfigureDefaultRegionBehaviors();

            this.Logger.Log(Resources.RegisteringFrameworkExceptionTypes, Category.Debug, Priority.Low);
            this.RegisterFrameworkExceptionTypes();

            this.Logger.Log(Resources.CreatingShell, Category.Debug, Priority.Low);
            this.Shell = this.CreateShell();
            if (this.Shell != null)
            {
                this.Logger.Log(Resources.SettingTheRegionManager, Category.Debug, Priority.Low);
                RegionManager.SetRegionManager(this.Shell, this.Kernel.Get<IRegionManager>());

                this.Logger.Log(Resources.UpdatingRegions, Category.Debug, Priority.Low);
                RegionManager.UpdateRegions();

                this.Logger.Log(Resources.InitializingShell, Category.Debug, Priority.Low);
                this.InitializeShell();
            }

            if (this.Kernel.IsRegistered<IModuleManager>())
            {
                this.Logger.Log(Resources.InitializingModules, Category.Debug, Priority.Low);
                this.InitializeModules();
            }

            this.Logger.Log(Resources.BootstrapperSequenceCompleted, Category.Debug, Priority.Low);
        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="ServiceLocator" />.
        /// </summary>
        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => this.Kernel.Get<IServiceLocator>());
        }

        /// <summary>
        /// Registers in the <see cref="Ninject.IKernel"/> the <see cref="Type"/> of the Exceptions
        /// that are not considered root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();

            ExceptionExtensions.RegisterFrameworkExceptionType(
                typeof(Ninject.ActivationException));

            
        }

        /// <summary>
        /// Configures the <see cref="Ninject.IKernel"/>. May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            Kernel.Bind <ILoggerFacade>().ToConstant(Logger).InSingletonScope();
            Kernel.Bind<IModuleCatalog>().ToConstant(ModuleCatalog).InSingletonScope();

            if (useDefaultConfiguration)
            {
                this.Kernel.RegisterTypeIfMissing<IServiceLocator, NinjectServiceLocatorAdapter>(true);
                this.Kernel.RegisterTypeIfMissing<IModuleInitializer, ModuleInitializer>(true);
                this.Kernel.RegisterTypeIfMissing<IModuleManager, ModuleManager>(true);
                this.Kernel.RegisterTypeIfMissing<RegionAdapterMappings, RegionAdapterMappings>(true);
                this.Kernel.RegisterTypeIfMissing<IRegionManager, RegionManager>(true);
                this.Kernel.RegisterTypeIfMissing<IEventAggregator, EventAggregator>(true);
                this.Kernel.RegisterTypeIfMissing<IRegionViewRegistry, RegionViewRegistry>(true);
                this.Kernel.RegisterTypeIfMissing<IRegionBehaviorFactory, RegionBehaviorFactory>(true);
                this.Kernel.RegisterTypeIfMissing<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>(false);
                this.Kernel.RegisterTypeIfMissing<IRegionNavigationJournal, RegionNavigationJournal>(false);
                this.Kernel.RegisterTypeIfMissing<IRegionNavigationService, RegionNavigationService>(false);
                this.Kernel.RegisterTypeIfMissing<IRegionNavigationContentLoader, RegionNavigationContentLoader>(true);
            }
        }

        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use a custom Modules Catalog
        /// </summary>
        protected override void InitializeModules()
        {
            IModuleManager manager;

            try
            {
                manager = this.Kernel.Get<IModuleManager>();
            }
            catch (Ninject.ActivationException ex)
            {
                if (ex.Message.Contains("IModuleCatalog"))
                {
                    throw new InvalidOperationException(Resources.NullModuleCatalogException);
                }

                throw;
            }

            manager.Run();
        }

        /// <summary>
        /// Creates the <see cref="Ninject.IKernel"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="Ninject.IKernel"/>.</returns>
        [CLSCompliant(false)]
        protected virtual IKernel CreateKernel()
        {
            return new StandardKernel();
        }
    }
}

