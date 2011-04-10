using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.NinjectExtensions;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using NinjectPrismExt.Properties;

namespace NinjectPrismExt
{
    public abstract class NinjectBootstrapper : Bootstrapper   
    {
      
        private bool useDefaultConfiguration = true;

        /// <summary>
        /// Gets the default <see cref="IUnityContainer"/> for the application.
        /// </summary>
        /// <value>The default <see cref="IUnityContainer"/> instance.</value>
        [CLSCompliant(false)]
        public INinjectContainer Container { get; protected set; }


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

            this.Logger.Log(Resources.CreatingUnityContainer, Category.Debug, Priority.Low);
            this.Container = this.CreateContainer();
            if (this.Container == null)
            {
                throw new InvalidOperationException(Resources.NullUnityContainerException);
            }

            this.Logger.Log(Resources.ConfiguringUnityContainer, Category.Debug, Priority.Low);
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
                RegionManager.SetRegionManager(this.Shell, this.Container.Resolve<IRegionManager>());

                this.Logger.Log(Resources.UpdatingRegions, Category.Debug, Priority.Low);
                RegionManager.UpdateRegions();

                this.Logger.Log(Resources.InitializingShell, Category.Debug, Priority.Low);
                this.InitializeShell();
            }

            if (this.Container.IsRegistered<IModuleManager>())
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
            ServiceLocator.SetLocatorProvider(() => this.Container.Resolve<IServiceLocator>());
        }

        /// <summary>
        /// Registers in the <see cref="INinjectContainer"/> the <see cref="Type"/> of the Exceptions
        /// that are not considered root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();

            ExceptionExtensions.RegisterFrameworkExceptionType(
                typeof(Ninject.ActivationException));
        }

        /// <summary>
        /// Configures the <see cref="INinjectContainer"/>. May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            

            Container.RegisterInstance<ILoggerFacade>(Logger,Scopes.InSingletonScope);

            this.Container.RegisterInstance(this.ModuleCatalog, Scopes.InSingletonScope);

            if (useDefaultConfiguration)
            {
                RegisterTypeIfMissing<IServiceLocator,NinjectServiceLocatorAdapter>(true);
                RegisterTypeIfMissing<IModuleInitializer,ModuleInitializer>(true);
                RegisterTypeIfMissing<IModuleManager,ModuleManager>(true);
                RegisterTypeIfMissing<RegionAdapterMappings,RegionAdapterMappings>(true);
                RegisterTypeIfMissing<IRegionManager,RegionManager>(true);
                RegisterTypeIfMissing<IEventAggregator,EventAggregator>(true);
                RegisterTypeIfMissing<IRegionViewRegistry,RegionViewRegistry>(true);
                RegisterTypeIfMissing<IRegionBehaviorFactory,RegionBehaviorFactory>(true);
                RegisterTypeIfMissing<IRegionNavigationJournalEntry,RegionNavigationJournalEntry>(false);
                RegisterTypeIfMissing<IRegionNavigationJournal,RegionNavigationJournal>(false);
                RegisterTypeIfMissing<IRegionNavigationService,RegionNavigationService>(false);
                RegisterTypeIfMissing<IRegionNavigationContentLoader,RegionNavigationContentLoader>(true);
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
                manager = this.Container.Resolve<IModuleManager>();
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
        /// Creates the <see cref="INinjectContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="INinjectContainer"/>.</returns>
        [CLSCompliant(false)]
        protected virtual INinjectContainer CreateContainer()
        {
            return new NinjectContainer();
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <param name="fromType">The interface type to register.</param>
        /// <param name="toType">The type implementing the interface.</param>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        protected void RegisterTypeIfMissing<fromType, toType>(bool registerAsSingleton) where toType : fromType
        {
           
            if (Container.IsRegistered<fromType>())
            {
                Logger.Log(
                    String.Format(CultureInfo.CurrentCulture,
                                  Resources.TypeMappingAlreadyRegistered,
                                  typeof(fromType).Name), Category.Debug, Priority.Low);
            }
            else
            {
                if (registerAsSingleton)
                {
                    Container.RegisterType<fromType, toType>(Scopes.InSingletonScope);
                }
                else
                {
                    Container.RegisterType<fromType, toType>(Scopes.InTransientScope);
                }
            }
        }
    }
}

