using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;


namespace SMART.Core.DataLayer
{
    public class ModelReader : ReaderBase, IModelReader
    {
        public IModel Load(Stream stream)
        {
            var settings = new XmlReaderSettings { CloseInput = false, IgnoreWhitespace = true, IgnoreComments = true };
            using (var xmlReader = XmlReader.Create(stream, settings))
            {
                var xdoc2 = XDocument.Load(xmlReader);

                var model = Configured<Model>(xdoc2.Descendants("model").First());

                var states = (from e in xdoc2.Descendants("state") select Configured<State>(e)).ToList();
                foreach (var state in states)
                {
                    if (state.Label == "Start" || state.Label == "Stop")
                    {
                        switch (state.Label)
                        {
                            case "Start":
                                model.StartState.Id = state.Id;
                                model.StartState.Location = state.Location;
                                break;
                            case "Stop":
                                model.StopState.Id = state.Id;
                                model.StopState.Location = state.Location;
                                break;
                        }
                    }
                    else
                    {
                        model.Add(state);    
                    }                    
                }
                //model.Add((from e in xdoc2.Descendants("state") select Configured<State>(e)).ToList());

                var transitions = (from e in xdoc2.Descendants("transition") select Configured<Transition>(e)).ToList();
                foreach (var transition in transitions)
                {
                    if (transition.Source != null && transition.Destination != null)
                    {
                        if (transition.Source.Id == model.StartState.Id)
                        {
                            transition.Source = null;
                        }
                        if (transition.Destination.Id == model.StopState.Id)
                        {
                            transition.Destination = null;
                        }
                    }
                    model.Add(transition);
                }
                //model.Add((from e in xdoc2.Descendants("transition") select Configured<Transition>(e)).ToList());

                return model;
            }
        }

    }
}