using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Syntax;

namespace NinjectPrismExt
{

    public interface IScope
    {
        IBindingNamedWithOrOnSyntax<TFrom> InScope<TFrom>(IBindingWhenInNamedWithOrOnSyntax<TFrom> binding);
    }

}
