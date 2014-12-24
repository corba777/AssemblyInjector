using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExceptionHandlingInjector.Model
{
    internal interface IHasIetms
    {
        IList<IAssemblyItem> GetItems();
    }
}
