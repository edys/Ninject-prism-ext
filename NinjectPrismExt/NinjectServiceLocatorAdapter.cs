using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Ninject;

namespace Prism.NinjectExtension
{
    public class NinjectServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly  IKernel _kernel;

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
            return _kernel.Get(serviceType);
        }
    }
}
