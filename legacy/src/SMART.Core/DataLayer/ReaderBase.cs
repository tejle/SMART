using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SMART.IOC;

namespace SMART.Core.DataLayer
{
    public abstract class ReaderBase
    {
        protected static readonly IDictionary<Guid, object> Resources = new Dictionary<Guid, object>();

        protected static T Configured<T>(XElement xElement)
        {
            if (xElement == null)
                return default(T);
            var element = Resolver.Resolve<T>();
            return Configured(xElement, element);
        }

        protected static T Configured<T>(XElement xElement, T element)
        {
            var idAttribute = xElement.Attribute("id");
            var identifyerProperty = element.GetType().GetProperty("Id", typeof(Guid));
            if (idAttribute != null && identifyerProperty != null)
            {
                var guid = new Guid(idAttribute.Value);
                identifyerProperty.SetValue(element, guid, null);
                if (!Resources.ContainsKey(guid))
                {
                    Resources.Add(guid, element);
                }
            }
            return (T)element.SetConfig(xElement.Element("config"), Resources);
        }

        protected static IList<T> ConfiguredImport<T>(IEnumerable<XElement> itemNodes, IEnumerable<T> imports) {
            var list = new List<T>();
            if (itemNodes != null)
                foreach (var node in itemNodes)
                    list.Add(ConfiguredImport(node, imports));
            return list;
        }

        protected static T ConfiguredImport<T>(XElement node, IEnumerable<T> imports) {
            var typeAttribute = node.Attribute("type");
            if (typeAttribute == null)
                return default(T);
            var adapterName = typeAttribute.Value;
            var importItem = imports.FirstOrDefault(item =>
                                                        {
                                                            var name = item.GetType().FullName;
                                                            return name.Equals(adapterName);
                                                        });
      
            return importItem == null ? default(T) : Configured(node, importItem);
        }
    }
}