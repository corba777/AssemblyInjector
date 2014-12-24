using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using ExceptionLogger;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using Mono.Cecil;
using Mono.Cecil.Cil;
using OpCodes = Mono.Cecil.Cil.OpCodes;

namespace ExceptionHandlingInjector.Model
{
    public class MethodModel:IAssemblyItem
    {
        private readonly MethodDefinition _methodDefinition;

        public string Name
        {
            get { return _methodDefinition.Name; }
        }

        public string Body 
        {
            get
            {
                var astBuilder = new AstBuilder(new DecompilerContext(_methodDefinition.DeclaringType.Module) { CurrentType = _methodDefinition.DeclaringType });
                astBuilder.AddMethod(_methodDefinition);
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
            get { return _methodDefinition.CustomAttributes.Any(ca => ca.GetType().Name == "ExceptionHandlerAttribute"); }
        }


        public MethodModel(MethodDefinition methodDefinition)
        {
            _methodDefinition = methodDefinition;

        }

        public  void Inject()
        {
            if (!IsInjected)
            {
                string logEndpoint = ConfigurationManager.AppSettings.AllKeys.Contains("Endpoint") ? ConfigurationManager.AppSettings["Endpoint"].Trim() : string.Empty; 
                string logName = ConfigurationManager.AppSettings.AllKeys.Contains("LogName") ? ConfigurationManager.AppSettings["LogName"].Trim() : string.Empty; 
                string environment = ConfigurationManager.AppSettings.AllKeys.Contains("Environment") ? ConfigurationManager.AppSettings["Environment"].Trim() : string.Empty;
                string ipaddress = "127.0.0.1";
                //string applicationName,string logEndpoint,string logName,string environment,string ipaddress

                //DynamicMethod dynamicMethod=new DynamicMethod();
                

                var il = _methodDefinition.Body.GetILProcessor();
                var module=_methodDefinition.Module;

                var sourceInstructions=_methodDefinition.Body.Instructions.ToList();
                int sourceInstCount = sourceInstructions.Count;
                var copyInstructionCount = _methodDefinition.ReturnType.FullName==typeof(void).FullName?sourceInstCount-1:sourceInstCount-2;
                var destInstructions = new List<Instruction>();
               
                destInstructions.Add(il.Create(OpCodes.Nop));
                destInstructions.Add(il.Create(OpCodes.Nop));

                for (int i = 0; i < copyInstructionCount; i++)
                {
                    if (sourceInstructions[i].OpCode != OpCodes.Br_S)
                    {
                        destInstructions.Add(sourceInstructions[i]);
                    }
                }

                var startCatch = destInstructions.Count;
                //destInstructions.Add(il.Create(OpCodes.Leave_S, destInstructions[startCatch - 1]));
                destInstructions.Add(il.Create(OpCodes.Pop));
                destInstructions.Add(il.Create(OpCodes.Nop));

                if (_methodDefinition.ReturnType.FullName != typeof (void).FullName)
                {
                    var defval = il.Create(OpCodes.Ldnull);
                    var stloc = il.Create(OpCodes.Stloc_0);
                    //var leaves = il.Create(OpCodes.Leave_S, stloc);

                    destInstructions.Add(defval);
                    destInstructions.Add(stloc);
                }
                else
                {
                    destInstructions.Add(il.Create(OpCodes.Nop));
                }
                //destInstructions.Add(leaves);

                int endCatchPos = destInstructions.Count;

                destInstructions.Add(il.Create(OpCodes.Nop));
                if (_methodDefinition.ReturnType.FullName != typeof(void).FullName)
                    destInstructions.Add(sourceInstructions[sourceInstCount-2]);
               
                destInstructions.Add(sourceInstructions[sourceInstCount-1]);
                var leaveInst = destInstructions[endCatchPos];

                destInstructions.Insert(startCatch, il.Create(OpCodes.Leave_S, leaveInst));
                destInstructions.Insert(endCatchPos+1, il.Create(OpCodes.Leave_S, leaveInst));
                
                
                //var write = il.Create(OpCodes.Call, module.Import(typeof(ExceptionExtension).GetMethod("WriteToLog")));
                

                //il.InsertAfter(_methodDefinition.Body.Instructions.Last(), firstParam);

                //il.InsertAfter(firstParam, secondParam);
                //il.InsertAfter(secondParam, thirdParam);
                //il.InsertAfter(thirdParam, forthParam);
                //il.InsertAfter(forthParam, fifthParam);
                //il.InsertAfter(fifthParam, write);

                //il.InsertAfter(write, leave);
                

                var handler = new Mono.Cecil.Cil.ExceptionHandler(ExceptionHandlerType.Catch)
                {
                    TryStart = destInstructions[1],
                    TryEnd = destInstructions[startCatch+1],
                    HandlerStart = destInstructions[startCatch+1],
                    HandlerEnd = destInstructions[endCatchPos+2],
                    CatchType = module.Import(typeof(Exception)),
                };
                
                _methodDefinition.Body.Instructions.Clear();

                foreach (var destInstruction in destInstructions)
                {
                    _methodDefinition.Body.Instructions.Add(destInstruction);
                }
                

                _methodDefinition.Body.ExceptionHandlers.Add(handler);
                
            }
            
        }

        public void UnInject()
        {
            if (IsInjected)
            {
                
            }
            
        }
    }
}
