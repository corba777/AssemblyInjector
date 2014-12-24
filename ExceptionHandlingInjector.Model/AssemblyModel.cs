using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace ExceptionHandlingInjector.Model
{
    public class AssemblyModel:IAssemblyItem,IHasIetms
    {

        private readonly AssemblyDefinition _assemblyDefinition;
        private readonly Lazy<List<TypeModel>> _types;

        public AssemblyModel(AssemblyDefinition assemblyDefinition)
        {
            _assemblyDefinition = assemblyDefinition;
            
            _types=new Lazy<List<TypeModel>>(() =>
                {
                    var typeModels=_assemblyDefinition.Modules
                        .SelectMany(m => m.Types).Where(t=>t.Name!="<Module>").Select(tm =>new TypeModel(tm)).ToList();

                    return typeModels;

                });
        }
       

        public string Name
        {
            get { return _assemblyDefinition.FullName; }
        }

        public string Body
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(Name);
                sb.Append("\r\n{\r\n");
                sb.Append(string.Join("\r\n",_types.Value.Select(t => t.Body)));
                sb.Append("}\r\n");

                return sb.ToString();

            }
        }

        public bool IsInjected 
        { 
            get { return _types.Value.All(t => t.IsInjected); }
        }
        
        public void Inject()
        {
            if (!IsInjected)
            {
                foreach (var type in _types.Value)
                {
                    type.Inject();
                }
            }
        }

        public void UnInject()
        {
            if (IsInjected)
            {
                foreach (var type in _types.Value)
                {
                    type.UnInject();
                }
            }
        }

        public IList<IAssemblyItem> GetItems()
        {
            return _types.Value.OfType<IAssemblyItem>().ToList();
        }

        public void Save(string path)
        {
            var fileInfo=new FileInfo(path);
            var location=new FileInfo(typeof (ExceptionLogger.ExceptionExtension).Assembly.Location);
            var destination = string.Format("{0}\\{1}", fileInfo.DirectoryName, location.Name);    
            
            var clientDll = string.Format("{0}\\ErrorReporting.ClientDLL.dll", location.DirectoryName);
            var destinationClientDll = string.Format("{0}\\ErrorReporting.ClientDLL.dll", fileInfo.DirectoryName);

            if(!File.Exists(destination))
                File.Copy(location.FullName,destination);

            if(File.Exists(clientDll) && !File.Exists(destinationClientDll))
                File.Copy(clientDll,destinationClientDll);

            _assemblyDefinition.MainModule.Import(typeof (ExceptionLogger.ExceptionExtension));
            _assemblyDefinition.Write(path);
        }
    }
}
