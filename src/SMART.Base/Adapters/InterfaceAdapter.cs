using System;
using System.ComponentModel.Composition;
using System.CodeDom;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;
using System.IO;

namespace SMART.Base.Adapters
{
    [Obsolete]
    [Export]
    [Export(typeof(IAdapter))]
    [Adapter(Name = "InterfaceAdapter", Description = "This adapter creates an interface")]
    public class InterfaceAdapter : IAdapter
    {
        [Config]
        public string InterfaceName { get; set; }

        CodeNamespace ns;

        CodeTypeDeclaration interfaceStub;

        CodeParameterDeclarationExpression arg;

        public void PreExecution()
        {
            ns = new CodeNamespace("SMART.Base");
            interfaceStub = new CodeTypeDeclaration(InterfaceName)
                                {
                                    Attributes = MemberAttributes.Public,
                                    IsInterface = false,
                                    IsClass = true,
                                    IsEnum = false,
                                    IsPartial = false,
                                    IsStruct = false

                                };
        }

        public bool Execute(string function, params string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    interfaceStub.Members.Add(new CodeMemberMethod
                                                  {
                                                      Name = function,
                                                      Parameters = { ParameterDeclaration() },
                                                      Attributes = MemberAttributes.Public,

                                                  });
                }
                else
                    interfaceStub.Members.Add(new CodeMemberMethod
                    {
                        Name = function,
                        Attributes = MemberAttributes.Public
                    });


            }
            catch (Exception e)
            {
                InvokeDefectDetected(null, e);
                return false;
            }
            return true;
        }

        public void PostExection()
        {
            try
            {
                AddResetMethod();

                ns.Types.Add(interfaceStub);
                var provider = new CSharpCodeProvider();
                var compileUnit = new CodeCompileUnit();
                compileUnit.Namespaces.Add(ns);

                var sb = new StringBuilder();

                using (var writer = new StringWriter(sb))
                {
                    provider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions()
                                                                                  {
                                                                                      BlankLinesBetweenMembers = true,
                                                                                      IndentString = "    "

                                                                                  });
                }

                Code = sb.ToString();
                //var cr = provider.CompileAssemblyFromDom(
                //    new CompilerParameters
                //        {
                //            OutputAssembly = string.Format("SMART.Base.{0}.dll", InterfaceName),
                //            GenerateInMemory = false,


                //        },
                //    compileUnit);

                //foreach (var r in cr.Errors)
                //    Console.WriteLine(r);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
        }

        private void AddResetMethod()
        {
            var method = new CodeMemberMethod
                             {
                                 Name = "Reset",
                                 Attributes = MemberAttributes.Public,

                             };
            method.Comments.Add(new CodeCommentStatement("Reset the adapter, used from smart when going back to the start state"));
            interfaceStub.Members.Add(method);
        }

        public string Code { get; private set; }
        private CodeParameterDeclarationExpression ParameterDeclaration()
        {
            if (arg == null)
            {
                var argAttr = new CodeAttributeDeclarationCollection
                                {
                                    new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute)))
                                };

                arg = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string[])), "parameters") { CustomAttributes = argAttr };
            }
            return arg;
        }

        private void InvokeDefectDetected(IStep step, Exception e)
        {
            var tmp = DefectDetected;
            if (tmp != null)
                tmp(this, new DefectEventArgs(step.ModelElement, e.Message));
        }

        public event EventHandler<DefectEventArgs> DefectDetected;

        public void Dispose()
        {

        }
    }
}
