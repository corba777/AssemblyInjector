using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InjectorApi;

namespace ExceptionHandlingInjector.Model
{
    public interface IAssemblyItem
    {   
        string Name { get; }
        string Body { get; }
        bool IsInjected { get;}
        void Inject();
        void UnInject();
    }
}
