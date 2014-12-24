using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InjectorApi
{
    public interface IInjectorExtensionAttribute
    {
        IInjectorInterceptor Interceptor { get; }
    }
}
