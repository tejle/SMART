using System;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.IO;

namespace SMART.Core.DomainModel.Validation
{
    public class RequiredCompilableAttribute : ValidatorBase
    {
        static private CSharpCodeProvider _provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.5" } });

        public override string Validate(string name, object value)
        {
            if (value == null)
                return null;

            try
            {
                _provider.Parse(new StringReader(value.ToString()));
            }
            catch (Exception)
            { 
                return Message(name, name + " does not compile.");
            }
            return null;
        }
    }
}