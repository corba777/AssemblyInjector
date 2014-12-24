using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExceptionHandlingInjector.Target
{
    class Program
    {
        static void Main(string[] args)
        {
            Entity entity=new Entity();
            entity.GetResult(5);
        }
    }
}
