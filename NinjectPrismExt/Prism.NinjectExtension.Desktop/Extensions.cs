using System;
using System.Collections.Generic;
using System.Globalization;
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

    }
}
