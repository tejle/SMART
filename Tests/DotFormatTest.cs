using NUnit.Framework;

using System.IO;
using SMART.Core.DomainModel;
using SMART.IOC;

namespace SMART.Test
{
    [TestFixture]
    public class DotFormatTest
    {
        
        [Test]
        public void make_dot_file_from_model()
        {
            var g = GetSimpleModel();

            DotBuilder.SaveModel(g, "test");
            Assert.IsTrue(File.Exists("test.plain"));
        }

        private static Model GetSimpleModel()
        {
            var v1 = new StartState { Label = "Start" };
            var v2 = new State { Label = "V2" };
            var v3 = new StopState { Label = "V3" };
            var e1 = new Transition { Source = v1, Destination = v2, Label = "E1" };
            var e2 = new Transition { Source = v2, Destination = v3, Label = "E2" };
            var model = new Model();
            model.Add(v1);
            model.Add(v2);
            model.Add(v3);
            model.Add(e1);
            model.Add(e2);
            return model;
        }
    }

    public class DotBuilder
    {
        public static void SaveModel(Model model, string name)
        {
            StreamWriter stream = new StreamWriter(name+".plain");
            stream.WriteLine("model 1.000 92.819 17.958");
            foreach (var v in model.States)
            {
                string id = "{" + v.Id + "}";
                stream.WriteLine(string.Format("node \"{0}\" 20.0 10.0 10.0 20.0 {1} filled ellipse black Plum",
                                               id, v.Label));
            }
            foreach (var transition in model.Transitions)
            {
                string id_s = "{" + transition.Source.Id +"}";
                string id_d = "{" + transition.Destination.Id +"}";
                stream.WriteLine(string.Format("transition \"{0}\" \"{1}\" 7 36.167 17.292 46.444 17.111 82.042 16.431 87.097 15.708 87.347 15.667 87.583 15.625 87.833 15.556 solid black", id_s, id_d));
            }
            stream.WriteLine("stop");
            stream.Flush();
            stream.Close();
        }
    }
}