using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using Mono.Cecil;

namespace ExceptionHandlingInjector.Model
{
    public class TypeModel:IAssemblyItem,IHasIetms
    {
        private readonly TypeDefinition _typeDefinition;

        public string Name
        {
            get
            {
                return _typeDefinition.Name;
            }
        }

        public string Body
        {
            get
            {
                var astBuilder = new AstBuilder(new DecompilerContext(_typeDefinition.Module) );
                astBuilder.AddType(_typeDefinition);
                string res = string.Empty;

                using (var sw = new StringWriter())
                {
                    astBuilder.GenerateCode(new PlainTextOutput(sw));
                    res = sw.ToString();
                }

                return res;
            }
        }

        public bool IsInjected
        {
            get { return _methods.Value.All(m => m.IsInjected); }
        }


        private readonly Lazy<List<MethodModel>> _methods;

        public TypeModel(TypeDefinition typeDefinition)
        {
            _typeDefinition = typeDefinition;
            _methods=new Lazy<List<MethodModel>>(() => _typeDefinition.Methods.Select(method => new MethodModel(method)).ToList());
            
        }

        public void Load()
        {
            foreach (var methodModel in _typeDefinition.Methods.Select(method => new MethodModel(method)))
            {
                _methods.Value.Add(methodModel);
            }
        }

        public void Inject()
        {
            foreach (var method in _methods.Value.Where(m=>!m.IsInjected))
            {
                method.Inject();
            }
        }


        public void UnInject()
        {
            foreach (var method in _methods.Value.Where(m => m.IsInjected))
            {
                method.UnInject();
            }
            
        }

        public IList<IAssemblyItem> GetItems()
        {
            return _methods.Value.OfType<IAssemblyItem>().ToList();
        }
    }
}
