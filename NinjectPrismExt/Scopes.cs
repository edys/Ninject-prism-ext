using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Activation;
using Ninject.Syntax;

namespace NinjectPrismExt
{
    public static class Scopes
    {
        public static IScope InSingletonScope
        {
            get { return new InSingletonScope(); }
        }

        public static IScope InRequestScope
        {
            get { return new InRequestScope(); }
        }

        public static IScope InThreadScope
        {
            get { return new InTransientScope(); }
        }

        public static IScope InTransientScope
        {
            get { return new InTransientScope(); }
        }

        public static IScope CreateInContextScope( Func<IContext, object> context)
        {
            return new InContextScope(context);
        }


    }


    public class InSingletonScope : IScope
    {
        public IBindingNamedWithOrOnSyntax<TFrom> InScope<TFrom>(IBindingWhenInNamedWithOrOnSyntax<TFrom> binding)
        {
            return binding.InSingletonScope();
        }
    }

    public class InRequestScope : IScope
    {
        public IBindingNamedWithOrOnSyntax<TFrom> InScope<TFrom>(IBindingWhenInNamedWithOrOnSyntax<TFrom> binding)
        {
            return binding.InRequestScope();
        }
    }

    public class InThreadScope : IScope
    {
        public IBindingNamedWithOrOnSyntax<TFrom> InScope<TFrom>(IBindingWhenInNamedWithOrOnSyntax<TFrom> binding)
        {
            return binding.InThreadScope();
        }
    }

    public class InContextScope : IScope
    {
        private readonly Func<IContext, object> _contextScope;

        public InContextScope(Func<IContext, object> contextScope)
        {
            _contextScope = contextScope;
        }

        public IBindingNamedWithOrOnSyntax<TFrom> InScope<TFrom>(IBindingWhenInNamedWithOrOnSyntax<TFrom> binding)
        {
            return binding.InScope(_contextScope);
        }
    }
    public class InTransientScope : IScope
    {
        public IBindingNamedWithOrOnSyntax<TFrom> InScope<TFrom>(IBindingWhenInNamedWithOrOnSyntax<TFrom> binding)
        {
            return binding.InTransientScope();
        }
    }

}
