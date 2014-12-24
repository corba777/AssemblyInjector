using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExceptionHandlingInjector.Model
{
    public static class AssemblyItemExtension
    {
        public static IList<IAssemblyItem> GetItems(this IAssemblyItem parent)
        {
            var ietms = parent as IHasIetms;
            if (ietms != null)
            {
                return ietms.GetItems();
            }

            return new List<IAssemblyItem>();
        }
    }
}
