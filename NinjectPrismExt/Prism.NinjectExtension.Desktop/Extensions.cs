using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Practices.Prism.Logging;
using Ninject;
using Ninject.Parameters;
using Prism.NinjectExtension.Properties;

namespace Prism.NinjectExtension
{
    public static  class  KernelExtensions
    {
        public static bool IsRegistered<TServiceType>(this IKernel currentKernel)
        {
            return currentKernel.CanResolve(currentKernel.CreateRequest(typeof (TServiceType), meta => true, new List<IParameter>(), false, false));             
        }


        public static void  RegisterTypeIfMissing<TFromType, TToType>(this IKernel currentKernel, bool registerAsSingleton) where TToType : TFromType
        {
           
            if (currentKernel.IsRegistered<TFromType>())
            {

                var logger= currentKernel.Get<ILoggerFacade>();
                logger.Log(String.Format(CultureInfo.CurrentCulture,Resources.TypeMappingAlreadyRegistered, typeof(TFromType).Name), Category.Debug, Priority.Low);
            }
            else
            {
                if (registerAsSingleton)
                {
                    currentKernel.Bind<TFromType>().To<TToType>().InSingletonScope();
                }
                else
                {
                    currentKernel.Bind<TFromType>().To<TToType>().InTransientScope();

                }
            }
        }

    }
}
