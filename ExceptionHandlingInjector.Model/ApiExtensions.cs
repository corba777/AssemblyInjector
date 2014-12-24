using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExceptionHandlingInjector.Model
{
    public static class ApiExtensions
    {
        public static TInput GetInstance<TInput>(this TInput input, params object[] args)
        {
            return Container.GetInstance<TInput>(args);
        }
    }
}
