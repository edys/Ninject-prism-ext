using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Practices.Prism.Logging;
using Ninject;
using Ninject.Parameters;
using Prism.NinjectExtension.Properties;

namespace Prism.NinjectExtension
{
    /// <summary>
    /// Extensions for the Ninject Kernel to provide similar methods as the IUnityContainer to help in 
    /// the transition to this Ninject Extension.  
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Basic method to find out if the service, <typeparamref name="TServiceType"/>, has been registered with the <param name="kernel"/> 
        /// </summary>
        /// <typeparam name="TServiceType"> the service to check for</typeparam>
        /// <param name="kernel"> the Ninject Kernel to check in</param>
        /// <returns> </returns>
        public static bool IsRegistered<TServiceType>(this IKernel kernel)
        {
            return kernel.IsRegistered(typeof(TServiceType));
        } 
        
        /// <summary>
        /// Basic method to find out if the service, <paramref name="serviceType"/>, has been registered with the <param name="kernel"/>  
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static bool IsRegistered(this IKernel kernel, Type serviceType)
        {
            return kernel.CanResolve(kernel.CreateRequest(serviceType, meta => true, new List<IParameter>(), false, false));             
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TFromType"></typeparam>
        /// <typeparam name="TToType"></typeparam>
        /// <param name="kernel"></param>
        /// <param name="registerAsSingleton"></param>
        public static void RegisterTypeIfMissing<TFromType, TToType>(this IKernel kernel, bool registerAsSingleton) where TToType : TFromType
        {
            kernel.RegisterTypeIfMissing(typeof(TFromType), typeof(TToType), registerAsSingleton);
        } 
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <param name="registerAsSingleton"></param>
        public static void RegisterTypeIfMissing(this IKernel kernel, Type fromType, Type toType,  bool registerAsSingleton)
        {

            if (kernel.IsRegistered(fromType))
            {
                var logger = kernel.Get<ILoggerFacade>();
                logger.Log(String.Format(CultureInfo.CurrentCulture, Resources.TypeMappingAlreadyRegistered, fromType.Name), Category.Debug, Priority.Low);
            }
            else
            {
                if (registerAsSingleton)
                {
                    kernel.Bind(fromType).To(toType).InSingletonScope();
                }
                else
                {
                    kernel.Bind(fromType).To(toType).InTransientScope();
                }
            }
        }


        /// <summary>
        /// Resolve an instance of the requested type with the given name from the kernel.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of object to get from the container.</typeparam>
        /// <param name="kernel">kernel to resolve from.</param>
        /// <param name="name">Name of the object to retrieve.</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        public static T Resolve<T>(this IKernel kernel, string name = null, params IParameter[] overrides)
        {
            return (T)kernel.Resolve(typeof(T), name, overrides);
        }

        /// <summary>
        /// Resolve an instance of the default requested type from the kernel.
        /// </summary>
        /// <param name="kernel">kernel to resolve from.</param>
        /// <param name="t"><see cref="Type"/> of object to get from the kernel.</param>
        /// <param name="name">Name of the object to retrieve.</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        public static object Resolve(this IKernel kernel, Type t, string name = null, params IParameter[] overrides)
        {
            return kernel.Get(t, name, overrides);
        }


        /// <summary>
        /// Return instances of all registered types requested.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful if you've registered multiple types with the same
        /// <see cref="Type"/> but different names.
        /// </para>
        /// <para>
        /// Be aware that this method does NOT return an instance for the default (unnamed) registration.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The type requested.</typeparam>
        /// <param name="container">Container to resolve from.</param>
        /// <param name="resolverOverrides">Any overrides for the resolve calls.</param>
        /// <returns>Set of objects of type <typeparamref name="T"/>.</returns>
        public static IEnumerable<T> ResolveAll<T>(this IKernel kernel, params IParameter[] resolverOverrides)
        {
            return kernel.GetAll(typeof (T), resolverOverrides).Cast<T>();
        }
    }
}
