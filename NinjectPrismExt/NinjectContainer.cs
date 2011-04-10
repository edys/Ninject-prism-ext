using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Components;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace NinjectPrismExt
{
    public class NinjectContainer : INinjectContainer
    {
        private IKernel _ninjectKernel;

        public NinjectContainer()
        {
            _ninjectKernel = new StandardKernel();
            
            RegisterInstance<INinjectContainer>(this, new InContextScope((x) => this));
            
        }


        public IComponentContainer NinjectComponents
        {
            get { return _ninjectKernel.Components; }
        }

        public INinjectSettings NinjectSettings
        {
            get { return _ninjectKernel.Settings; }
        }


        public INinjectContainer Parent { get; protected set; }

        public IEnumerable<IBinding> Registrations
        {
            get { return _ninjectKernel.GetBindings(typeof(object)); }
        }



        private IBindingWhenInNamedWithOrOnSyntax<TFrom> SimpleBinding<TFrom, TTo>() where TTo : TFrom
        {
            return _ninjectKernel.Bind<TFrom>().To<TTo>();
        }

        private void AddName<TFrom>(IBindingNamedWithOrOnSyntax<TFrom> binding, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                binding.Named(name);
            }
        }

        public INinjectContainer RegisterType<TFrom, TTo>(IScope scope, string name = null, params IParameter[] injectionMembers) where TTo : TFrom
        {
            var binding = scope.InScope(SimpleBinding<TFrom, TTo>());
            AddName(binding, name);
            if (injectionMembers != null && injectionMembers.Length > 0)
            {
                foreach (var injectionMember in injectionMembers)
                {
                    binding.WithParameter(injectionMember);
                }
            }
            return this;
        }


        public INinjectContainer RegisterInstance<TType>(TType instance, IScope scope, string nameToCheck = null)
        {
            var binding = scope.InScope(_ninjectKernel.Bind<TType>().ToConstant(instance));
            AddName(binding, nameToCheck);
            return this;
        }

        public TType Resolve<TType>(string name = null, params IParameter[] resolverOverrides) where TType : class
        {
            return Resolve(typeof (TType), name, resolverOverrides) as TType;
        }

        public object Resolve(Type type, string name = null, params IParameter[] resolverOverrides )
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return _ninjectKernel.TryGet(type,resolverOverrides);
            }
            return _ninjectKernel.TryGet(type,name, resolverOverrides);
        }


        public IEnumerable<TType> ResolveAll<TType>(params IParameter[] resolverOverrides) where TType : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> ResolveAll(Type type,params IParameter[] resolverOverrides)
        {
            throw new NotImplementedException();
        }

        public object BuildUp<TType>(TType existing, string name, params IParameter[] resolverOverrides)
        {
            throw new NotImplementedException();
        }

        public void Teardown(object o)
        {
            throw new NotImplementedException();
        }

       

        public INinjectContainer CreateChildContainer()
        {
            var container = new NinjectContainer() { Parent = this};
            return container;
        }



        public bool IsRegistered<TType>(string nameToCheck = null)
        {
            return _ninjectKernel.CanResolve(_ninjectKernel.CreateRequest(typeof (TType), meta => string.IsNullOrWhiteSpace(nameToCheck) ? Equals(meta.Name, nameToCheck) : true, new List<IParameter>(), false, false)); 
        }
    }
}
