using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExceptionHandlingInjector.Model
{
    public class Container
    {
        private static readonly Dictionary<Type, IList<Type>> Types = new Dictionary<Type, IList<Type>>();

        public static T GetInstance<T>(string typeName, params object[] args)
        {
            var type = GetTypes<T>().FirstOrDefault(t => t.Name == typeName || t.FullName == typeName);

            if (type == null)
                return GetInstance<T>(args);

            return (T)Activator.CreateInstance(type, args);
        }

        public static T GetInstance<T>(params object[] args)
        {
            var type = GetTypes<T>().FirstOrDefault();

            if (type != null)
                return (T)Activator.CreateInstance(type, args);

            return default(T);
        }

        private static IList<Type> GetTypes<T>()
        {


            if (Types.ContainsKey(typeof (T)))
                return Types[typeof (T)];                                    

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();//.Where(a => !a.FullName.Contains("System")).ToList();
            var resTypes=assemblies.SelectMany(a => a.GetTypes()).Where(t => t.GetInterfaces().Any(i => i.FullName == typeof(T).FullName)).ToList();
            Types.Add(typeof(T),resTypes);

            return resTypes;

        }

        private static bool IsNotObsolete(Type type)
        {
            var attributes = type.GetCustomAttributes(true).ToList();
            return attributes.Count == 0;
        }
    }
}
