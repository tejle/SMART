using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CloningExtension;

namespace System
{
	public static class DeepCloning
	{
		public static T DeepClone<T>(this T obj)
		{
			IFormatter formatter = new BinaryFormatter();
			formatter.SurrogateSelector = new SurrogateSelector();
			formatter.SurrogateSelector.ChainSelector(
				new NonSerialiazableTypeSurrogateSelector());
			var ms = new MemoryStream();
			formatter.Serialize(ms, obj);
			ms.Position = 0;
			return (T)formatter.Deserialize(ms);
		}
	}
}
