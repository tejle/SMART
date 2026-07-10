
using SMART.Core.DataLayer.Interfaces;

namespace SMART.Core
{
    using System;
    using IOC;

    public static class ExtentionsOfIIdentifyable {
        public static T Clone<T>(this T f) where T : class, IIdentifyable {
            var t = f.DeepClone();
            //var t = Resolver.Resolve<T>();
            t.SetConfig(f.GetConfig());
            t.Id = f.Id;
            return t;
        }
    }
}
