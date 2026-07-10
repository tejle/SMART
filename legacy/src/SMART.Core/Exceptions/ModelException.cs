using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMART.Core.Exceptions
{
    public class ModelException : Exception
    {
        public ModelException(string message) : base(message) { }
    }
}
