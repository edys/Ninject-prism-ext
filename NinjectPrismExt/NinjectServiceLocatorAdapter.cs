using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using NinjectPrismExt;

namespace Microsoft.Practices.Prism.NinjectExtensions
{
    public class NinjectServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly INinjectContainer _container;

        public NinjectServiceLocatorAdapter(INinjectContainer container)
        {
            _container = container;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _container.ResolveAll(serviceType);
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return _container.Resolve(serviceType, key);
        }
    }
}
