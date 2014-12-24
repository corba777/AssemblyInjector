using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExceptionHandlingInjector.Target
{
    public class Entity
    {
        public float? GetResult(int div)
        {
            return div + 1;
        }

        public float? GetResult1(int div)
        {
            
            try
            {
                return div + 1;
            }
            catch (Exception)
            {
                return default(float);

            }
        }

        private string _name;
        public string Name
        {
            get
            {
                try
                {
                    return _name;
                }
                catch (Exception)
                {
                    return default(string);
                }
                
            }
            private
            set
            {
                _name = value;
            }
        }

        
        public string Name1{get; private set; }
    }
}
