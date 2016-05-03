
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Metadata;

namespace SMART.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using System.Xml;
    using System.Xml.Linq;
    
    using DataLayer;
    using Metadata;
    
    using IOC;

    public static class ExtentionsOfEnumerableExportT_TMetadata
    {

        public static T Parse<T, TMetadata>(this IEnumerable<Export<T, TMetadata>> imports, string name)
        {
            var export = imports.FirstOrDefault(e => e.Metadata.ContainsKey("Name") && e.Metadata["Name"].Equals(name));
            return export != null ? export.GetExportedObject() : default(T);
        }

        public static TMetadata MetadataView<TMetadata>(this object instance)
        {
            return (TMetadata)instance.GetType().GetCustomAttributes(typeof(TMetadata), true).FirstOrDefault();
        }
    }

    public static class ExtentionsOfConfigAttributeObjects
    {
        public class Imports
        {
            [Import]
            public IEnumerable<Export<IStatistic, IStatisticMetadata>> Statistics { get; set; }
        }

        private static readonly Imports Available;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        static ExtentionsOfConfigAttributeObjects()
        {
            Available = Resolver.Resolve<Imports>();
        }

        public static IDictionary<string, IConfigSetting> GetConfig(this object instance)
        {
            return (from p in instance.GetType().GetProperties()
                    let customAttributes = p.GetCustomAttributes(typeof(ConfigAttribute), true)
                    where customAttributes.Length == 1
                    select new ConfigSetting
                               {
                                   Name = p.Name,
                                   Type = p.PropertyType,
                                   Config = customAttributes[0] as ConfigAttribute,
                                   Value = p.GetValue(instance, null)
                               }).Cast<IConfigSetting>().ToDictionary(c => c.Name);
        }

        public static void Update(this IDictionary<string, IConfigSetting> configSettings, string key, object value)
        {
            if (configSettings.ContainsKey(key))
                configSettings[key].Value = value;
        }

        public static object SetConfig(this object instance, IDictionary<string, IConfigSetting> config)
        {
            foreach (var setting in config.Values)
            {
                instance.SetConfig(setting);
            }
            return instance;
        }

        public static object SetConfig(this object instance, IConfigSetting configSetting)
        {
            var instanceType = instance.GetType();
            var propertyType = configSetting.Type;
            var propertyName = configSetting.Name;
            var value = configSetting.Value ?? configSetting.Config.Default;
            var property = instanceType.GetProperty(propertyName, propertyType);
            if (value != null && !propertyType.IsInstanceOfType(value))
            {
                var typeName = propertyType.FullName;
                if (ConvertToObject.ContainsKey(typeName))
                {
                    value = ConvertToObject[typeName](value.ToString());
                }
            }
            if (property != null && value != null)
                property.SetValue(instance, value, null);
            
            return instance;
        }

        public static object SetConfig(this object instance, XmlNodeList configNodes)
        {
            var config = instance.GetConfig();
            if (configNodes != null)
                foreach (XmlElement configNode in configNodes)
                    config.Update(configNode.Name, configNode.InnerXml);
            instance.SetConfig(config);
            return instance;
        }

        public static object SetConfig(this object instance, XElement configNode, IDictionary<Guid, object> resources)
        {
            var config = instance.GetConfig();
            if (configNode != null)
                foreach (var element in configNode.Elements())
                {
                    if (element.Value.Trim().Equals(""))
                        continue;

                    var value = element.Value;

                    var typeAttribute = element.Attribute("type");
                    var type = typeAttribute != null ? typeAttribute.Value : "System.String";

                    object valueObject = value;

                    if (type.Equals("System.Guid"))
                    {
                        var key = new Guid(value);
                        valueObject = resources.ContainsKey(key) ? resources[key] : null;
                    }

                    else if (ConvertToObject.ContainsKey(type))
                        valueObject = ConvertToObject[type](value);

                    config.Update(element.Name.LocalName, valueObject);
                }
            instance.SetConfig(config);
            return instance;
        }

        private static readonly IDictionary<string, Func<string, object>> ConvertToObject =
            new Dictionary<string, Func<string, object>>
                {
                    {"System.String", s => s},
                    {"System.Boolean", s => bool.Parse(s)},
                    //{"System.Windows.Point", s => Point.Parse(s.Replace(',','.').Replace(';', ','))},
                    {"SMART.Core.SmartPoint", s => SmartPoint.Parse(s.Replace(',','.').Replace(';', ','))},
                    {"SMART.Core.SmartSize", s => SmartSize.Parse(s.Replace(',','.').Replace(';', ','))},
                    {"SMART.Core.SmartImage", s => new SmartImage(s)},
                    {"System.Double", s => Double.Parse(s.Replace('.',','))},
                    {"System.Int32", s => Int32.Parse(s)},
                    {"System.TimeSpan", s => TimeSpan.Parse(s)},
                    {"System.DateTime", s => DateTime.Parse(s)},
                    {"SMART.Core.IStatistic", s => Available.Statistics.Parse(s)},
                    {"SMART.Core.DomainModel.StateType", s=> Enum.Parse(typeof(StateType),s)}                    
                };

        public static XElement GetXConfig(this object instance)
        {
            return new XElement("config", from c in instance.GetConfig().Values select new XElement(c.Name, ConfigValue(c)));
        }

        private static IEnumerable<XObject> ConfigValue(IConfigSetting c)
        {
            var configType = c.Type;
            var configValue = c.Value ?? c.Config.Default;
            if (configValue != null)
            {
                Guid? identity;
                if (IsIdentifyable(configValue, out identity))
                {
                    configType = typeof(Guid);
                    configValue = identity.Value;
                }
                else if (configType.Equals(typeof(SmartPoint)))
                {                    
                    configValue = string.Format("{0};{1}", (configValue as SmartPoint).X, (configValue as SmartPoint).Y);
                }
                else if (configType.Equals(typeof(SmartSize)))
                {
                    configValue = string.Format("{0};{1}", (configValue as SmartSize).Width, (configValue as SmartSize).Height);
                }
                else if (configType.Equals(typeof(SmartImage)))
                {
                  if (configValue is SmartImage)
                  {
                    if ((configValue as SmartImage).Image != null)
                    {
                      configValue = BitConverter.ToString((configValue as SmartImage).Image).Replace("-", "");
                    }
                  }
                }
                else if (configType.Equals(typeof(IStatistic)))
                {
                    configValue = configValue.MetadataView<IStatisticMetadata>().Name;
                }
                yield return new XText(configValue.ToString());
            }

            yield return new XAttribute("type", configType.FullName);

        }

        private static bool IsIdentifyable(object configValue, out Guid? identity)
        {
            identity = null;
            var identityProperty = configValue.GetType().GetProperty("Id", typeof(Guid));
            if (identityProperty == null) return false;
            identity = (Guid)identityProperty.GetValue(configValue, null);
            return identityProperty != null;
        }
    }
}
