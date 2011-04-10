using System;
using System.Collections.Generic;
using Ninject.Parameters;
using Ninject.Planning.Bindings;

namespace NinjectPrismExt
{
    public interface INinjectContainer
    {
        /// <summary>
        /// The parent of this container.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The parent container, or null if this container doesn't have one.
        /// </value>
        INinjectContainer Parent { get; }

        /// <summary>
        /// Get a sequence of <see cref="T:Ninject.Planning.Bindings.IBinding"/> that describe the current state
        ///             of the container.
        /// </summary>
        IEnumerable<IBinding> Registrations { get; }

        /// <summary>
        /// Register a type mapping with the container, where the created instances will use
        ///             the given <see cref="T:NinjectPrismExt.IScope"/>.
        /// 
        /// </summary>
        /// <param name="Tfrom"><see cref="T:System.Type"/> that will be requested.</param>
        /// <param name="Tto"><see cref="T:System.Type"/> that will actually be returned.</param>
        /// <param name="scope">The <see cref="T:NinjectPrismExt.IScope"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>
        /// The <see cref="T:NinjectPrismExt.NinjectContainer"/> object that this method was called on (this in C#, Me in Visual Basic).
        /// </returns>
        INinjectContainer RegisterType<TFrom, TTo>(IScope scope, string name = null, params IParameter[] injectionMembers) where TTo : TFrom;

        /// <summary>
        /// Register an instance with the container.
        ///
        /// </summary>
        /// 
        /// <remarks>
        /// 
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        ///             of the container creating the instance the first time it is requested, the user
        ///             creates the instance ahead of type and adds that instance to the container.
        /// 
        /// </para>
        /// 
        /// </remarks>
        /// <param name="TType">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="scope"><see cref="T:NinjectPrismExt.IScope"/> object that controls how this instance will be managed by the container.</param>
        /// <returns>
        /// The <see cref="T:NinjectPrismExt.NinjectContainer"/> object that this method was called on (this in C#, Me in Visual Basic).
        /// </returns>
        INinjectContainer RegisterInstance<TType>(TType instance, IScope scope, string name = null);

        /// <summary>
        /// Resolve an instance of the requested type with the given name from the container.
        /// 
        /// </summary>
        /// <param name="TType"><see cref="T:System.Type"/> of object to get from the container.</param>
        /// <param name="name">Name of the object to retrieve.</param>
        /// <param name="resolverOverrides">Any overrides for the resolve call.</param>
        /// <returns>
        /// The retrieved object.
        /// </returns>
        TType Resolve<TType>(string name = null, params IParameter[] resolverOverrides) where TType : class;

        object Resolve(Type type, string name = null, params IParameter[] resolverOverrides);


        /// <summary>
        /// Return instances of all registered types requested.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// 
        /// <para>
        /// This method is useful if you've registered multiple types with the same
        ///             <see cref="T:System.Type"/> but different names.
        /// 
        /// </para>
        /// 
        /// <para>
        /// Be aware that this method does NOT return an instance for the default (unnamed) registration.
        /// 
        /// </para>
        /// 
        /// </remarks>
        /// <param name="t">The type requested.</param>
        /// <param name="resolverOverrides">Any overrides for the resolve calls.</param>
        /// <returns>
        /// Set of objects of type <paramref name="TType"/>.
        /// </returns>
        IEnumerable<TType> ResolveAll<TType>(params IParameter[] resolverOverrides) where TType : class;


        IEnumerable<object> ResolveAll(Type type, params IParameter[] resolverOverrides);


        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// 
        /// <para>
        /// This method is useful when you don't control the construction of an
        ///             instance (ASP.NET pages or objects created via XAML, for instance)
        ///             but you still want properties and other injection performed.
        /// 
        /// </para>
        /// 
        /// </remarks>
        /// <param name="TType"><see cref="T:System.Type"/> of object to perform injection on.</param>
        /// <param name="existing">Instance to build up.</param>
        /// <param name="name">name to use when looking up the type mappings and other configurations.</param>
        /// <param name="resolverOverrides">Any overrides for the resolve calls.</param>
        /// <returns>
        /// The resulting object. By default, this will be <paramref name="existing"/>, but
        ///             container extensions may add things like automatic proxy creation which would
        ///             cause this to return a different object (but still type compatible with <paramref name="TType"/>).
        /// </returns>
        object BuildUp<TType>(TType existing, string name, params IParameter[] resolverOverrides);

        /// <summary>
        /// Run an existing object through the container, and clean it up.
        /// 
        /// </summary>
        /// <param name="o">The object to tear down.</param>
        void Teardown(object o);

        #region Un-implmented methods 

        ///// <summary>
        ///// Add an extension object to the container.
        ///// 
        ///// </summary>
        ///// <param name="extension"><see cref="T:Microsoft.Practices.Unity.UnityContainerExtension"/> to add.</param>
        ///// <returns>
        ///// The <see cref="T:Microsoft.Practices.Unity.UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).
        ///// </returns>
        //INinjectContainer AddExtension(INinjectContainer extension);

        ///// <summary>
        ///// Resolve access to a configuration interface exposed by an extension.
        ///// 
        ///// </summary>
        ///// 
        ///// <remarks>
        ///// Extensions can expose configuration interfaces as well as adding
        /////             strategies and policies to the container. This method walks the list of
        /////             added extensions and returns the first one that implements the requested type.
        ///// 
        ///// </remarks>
        ///// <param name="configurationInterface"><see cref="T:System.Type"/> of configuration interface required.</param>
        ///// <returns>
        ///// The requested extension's configuration interface, or null if not found.
        ///// </returns>
        //object Configure(Type configurationInterface);

        /// <summary>
        /// Remove all installed extensions from this container.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// 
        /// <para>
        /// This method removes all extensions from the container, including the default ones
        ///             that implement the out-of-the-box behaviour. After this method, if you want to use
        ///             the container again you will need to either re-add the default extensions or replace
        ///             them with your own.
        /// 
        /// </para>
        /// 
        /// <para>
        /// The registered instances and singletons that have already been set up in this container
        ///             do not get removed.
        /// 
        /// </para>
        /// 
        /// </remarks>
        /// 
        /// <returns>
        /// The <see cref="T:Microsoft.Practices.Unity.UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).
        /// </returns>
        //INinjectContainer RemoveAllExtensions();

        /// <summary>
        /// Create a child container.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// A child container shares the parent's configuration, but can be configured with different
        ///             settings or lifetime.
        /// </remarks>
        /// 
        /// <returns>
        /// The new child container.
        /// </returns>
        //INinjectContainer CreateChildContainer();
        
        #endregion

        /// <summary>
        /// Check if a particular type/name pair has been registered with the container.
        /// 
        /// </summary>
        /// <param name="TType">Type to check registration for.</param>
        /// <param name="nameToCheck">Name to check registration for.</param>
        /// <returns>
        /// True if this type/name pair has been registered, false if not.
        /// </returns>
        bool IsRegistered<TType>(string nameToCheck = null);






    }
}
