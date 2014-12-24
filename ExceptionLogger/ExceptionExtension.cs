using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionLogger
{
    public  class ExceptionExtension
    {
        public static void WriteToLog(Exception e, string applicationName,string logEndpoint,string logName,string environment,string ipaddress)
        {
           
            var clientdllPath = string.Format("{0}\\ErrorReporting.ClientDLL.dll", Directory.GetCurrentDirectory());

            if (File.Exists(clientdllPath))
            {
                TaskScheduler.UnobservedTaskException += (sender, args) => args.SetObserved();

                Task task = Task.Factory.StartNew(() =>
                {
                    Assembly assembly = Assembly.LoadFile(clientdllPath);
                    dynamic client = assembly.CreateInstance("ErrorReporting.ClientDLL.ClientErrorReportingService", false, BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance, null, new object[] { logEndpoint }, null, null);


                    if (client != null)
                    {
                        client.SendMessage(DateTime.Now, logName, applicationName, environment, 1, e, string.Empty, ipaddress);
                        
                    }
                });

                task.ContinueWith(t =>
                {
                    foreach (var ex in t.Exception.Flatten().InnerExceptions)
                        Console.WriteLine(ex.Message);
                }, TaskContinuationOptions.OnlyOnFaulted);

                Task.WaitAll(new[] { task });
                

            }   


        }
    }
}
