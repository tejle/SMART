using System.IO;
using System.Linq;
using System.Xml;

using System.Xml.Linq;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

namespace SMART.Core.DataLayer
{
    public class ModelWriter : WriterBase, IModelWriter
    {
        public void Save(Stream stream, IModel model)
        {
            var settings = new XmlWriterSettings { Indent = true, CloseOutput = false };
            var xmlWriter = XmlWriter.Create(stream, settings);
            if (xmlWriter == null) return;

            XDocument doc = GetDoc(model);
            doc.Save(xmlWriter);
            xmlWriter.Close();
        }

        private static XDocument GetDoc(IModel model)
        {
            return new XDocument(new XDeclaration("1.0", "UTF-8", "no"),
                                 new XElement("smart",GetModel(model)));
        }

        private static XElement GetModel(IModel model)
        {
            return new XElement("model",
                                new XAttribute("id", model.Id.ToString()),
                                GetStates(model),
                                GetTransitions(model),
                                model.GetXConfig()
                );
        }

        private static XElement GetTransitions(IModel model)
        {
            return new XElement("transitions", from e in model.Transitions
                                               select new XElement("transition", new XAttribute("id", e.Id.ToString()), e.GetXConfig(),
                                                                   new XElement("parameters", from p in e.Parameters
                                                                                              select new XElement("parameter", p))));
        }

        private static XElement GetStates(IModel model)
        {
            return new XElement("states", 
                                from v in model.States
                                //from v in model.States.Except(new State[] { model.StartState, model.StopState })
                                select new XElement("state", new XAttribute("id", v.Id.ToString()), v.GetXConfig()));
        }
    }
}