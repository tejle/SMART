using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CSharp;
using SMART.Base.Adapters;
using SMART.Core.Interfaces;
using System.CodeDom;
using System.Text;
using SMART.IOC;
using SMART.Core.DomainModel;

namespace SMART.Core.Services
{
    public class CodeGenerationService: ICodeGenerationService {
        ManualResetEvent wait = new ManualResetEvent(false);
        private IProject project;
        private ITestcase testcase;
        List<IStep> items;
        IModelCompiler compiler;

        public CodeGenerationService()
        {
            items = new List<IStep>();
            compiler = Resolver.Resolve<IModelCompiler>();
        }

        public string GetCodeAsString(IProject project, ITestcase testcase)
        {
            this.project = project;
            this.testcase = testcase;

            var model = compiler.Compile(testcase.Models);
            if (model == null) return string.Empty;
            var env = compiler.CreateSandbox(model);

            foreach(var elem in model.Elements)
                items.Add(compiler.CreateStep(elem));

            return GenerateCode(testcase.Name, items);
        }


        private string GenerateCode(string name, IEnumerable<IStep> steps)
        {
            var ns = new CodeNamespace(string.Format("SMART.{0}", project.Name.Replace(' ', '_')));
            var typeDeclaration = new CodeTypeDeclaration(name.Replace(' ', '_'))
                                                      {
                                                          Attributes = MemberAttributes.Public,
                                                          IsClass = true
                                                          
                                                      };

            typeDeclaration.Members.Add(new CodeMemberEvent()
                                            {
                                                Name = "DefectDetected",
                                                Attributes = MemberAttributes.Public,
                                                Type = new CodeTypeReference("EventHandler")

                                            });

            var method = new CodeMemberMethod
            {
                Name = "Reset",
                Attributes = MemberAttributes.Public,

            };
            method.Comments.Add(new CodeCommentStatement("Reset the adapter, used from smart when going back to the start state"));
            typeDeclaration.Members.Add(method);

            var methods = new Dictionary<string, IStep>();
            var transitions = from t in steps
                              where t.ModelElement is Transition
                              select t;
            var states = from s in steps
                         where s.ModelElement is State
                         select s;

            foreach(var step in transitions)
            {
                if (!methods.ContainsKey(step.Function))
                {
                    typeDeclaration.Members.Add(new CodeMemberMethod()
                    {
                        Name = step.Function.Replace(' ', '_'),
                        Attributes = MemberAttributes.Public,
                        Parameters = { ParameterDeclaration() }
                    });
                    methods.Add(step.Function, step);
                }
            }

            foreach (var step in states)
            {
                if (!methods.ContainsKey(step.Function))
                {
                    typeDeclaration.Members.Add(new CodeMemberMethod()
                    {
                        Name = step.Function.Replace(' ', '_'),
                        Attributes = MemberAttributes.Public
                    });
                    methods.Add(step.Function, step);
                }
            }
            //foreach (var step in steps)
            //{
            //    if(step.Parameters == null || step.Parameters.Length == 0)
            //    {
            //        if (!methods.ContainsKey(step.Function))
            //        {
            //            typeDeclaration.Members.Add(new CodeMemberMethod()
            //                                            {
            //                                                Name = step.Function.Replace(' ', '_'),
            //                                                Attributes = MemberAttributes.Public
            //                                            });
            //            methods.Add(step.Function, step);
            //        }
            //    }
            //    else
            //    {
            //        if (!methods.ContainsKey(step.Function))
            //        {
            //            typeDeclaration.Members.Add(new CodeMemberMethod()
            //                                            {
            //                                                Name = step.Function.Replace(' ', '_'),
            //                                                Attributes = MemberAttributes.Public,
            //                                                Parameters = {ParameterDeclaration()}
            //                                            });
            //            methods.Add(step.Function, step);
            //        }
            //    }
            //}

            ns.Types.Add(typeDeclaration);
            var provide = new CSharpCodeProvider();
            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(ns);
            
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                provide.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions()
                                                                             {
                                                                                 BlankLinesBetweenMembers = true,
                                                                                 IndentString = "    "
                                                                             });
            }

            return sb.ToString();
        }

        private CodeParameterDeclarationExpression ParameterDeclaration() {
            CodeParameterDeclarationExpression declarationExpression;
                var argAttr = new CodeAttributeDeclarationCollection
                                {
                                    new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute)))
                                };

                declarationExpression = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string[])), "parameters") { CustomAttributes = argAttr };
            return declarationExpression;
        }
    }
}