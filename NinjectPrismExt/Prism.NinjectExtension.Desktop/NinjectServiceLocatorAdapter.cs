using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Ninject;

namespace Prism.NinjectExtension
{
    /// <summary>
    /// The Service Locator Adapter for Ninject.
    /// </summary>
    public class NinjectServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly  IKernel _kernel;

        /// <summary>
        /// The default constructor that takes in a Ninject Kernel to be use for resolution of service types 
        /// </summary>
        /// <param name="kernel">the Ninject Kernel that is been adapter</param>
        public NinjectServiceLocatorAdapter(IKernel kernel)
        {
            _kernel = kernel;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return _kernel.Get(serviceType, key);
        }
    }
}
