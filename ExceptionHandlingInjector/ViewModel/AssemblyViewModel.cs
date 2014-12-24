using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceptionHandlingInjector.Infrastructure;
using ExceptionHandlingInjector.Model;

namespace ExceptionHandlingInjector.ViewModel
{
    public sealed class AssemblyViewModel:AssemblyItemViewModel
    {
        private readonly AssemblyModel _assemblyModel;

        public AssemblyViewModel(AssemblyModel assemblyItem) : base(assemblyItem)
        {
            _assemblyModel = assemblyItem;
        }

        public void Save(string path)
        {
            _assemblyModel.Save(path);
        }
    }
}
