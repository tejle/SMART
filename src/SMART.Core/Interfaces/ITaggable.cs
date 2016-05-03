using System.Collections.Generic;

namespace SMART.Core.Interfaces
{
    public interface ITaggable
    {
        Dictionary<string,object> Tags{ get; set;}
    }
}