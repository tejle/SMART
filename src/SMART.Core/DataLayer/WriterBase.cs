using System.Xml.Linq;
using SMART.Core.DataLayer.Interfaces;

namespace SMART.Core.DataLayer
{
    public abstract class WriterBase
    {
        protected static XElement Configured<T>(T instance) where T : IIdentifyable
        {
            return null;
        }
    }
}