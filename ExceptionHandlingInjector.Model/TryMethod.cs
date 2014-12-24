using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExceptionHandlingInjector.Model
{
    public static class TryMethod
    {
        public static TInput Try<TInput>(this TInput input, Expression<Action<TInput>> func)
        {

            try
            {
                func.Compile().Invoke(input);
            }
            catch (Exception e)
            {   
            }

            return input;
        }


        public static TInput Try<TInput, TOutput>(this TInput input, Expression<Func<TInput, TOutput>> func, out TOutput result)
        {

            try
            {
                var methodInfo = ((MethodCallExpression)func.Body).Method;
                result = func.Compile().Invoke(input);
            }
            catch (Exception e)
            {
            
                result = default(TOutput);
            }

            return input;
        }
    }
}
