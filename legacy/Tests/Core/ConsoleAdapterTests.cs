using System.IO;
using NUnit.Framework;
using SMART.Base.Adapters;

namespace SMART.Test.Base
{
    [TestFixture]
    public class ConsoleAdapterTests
    {
        [Test]
        public void execute_should_write_to_std_out()
        {
            MemoryStream stream = new MemoryStream();
            TextWriter writer = new StreamWriter(stream);
            System.Console.SetOut(writer);

            var consoleAdapter = new ConsoleAdapter();
            consoleAdapter.Execute("test", "p1", "p2");
            writer.Flush();

            stream.Position = 0;
            TextReader reader = new StreamReader(stream);
            string s = reader.ReadLine();
            Assert.AreEqual("test p1, p2", s);
        }
    }
}