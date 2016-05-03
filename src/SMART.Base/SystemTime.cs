using System;

namespace SMART.Base
{
    /// <summary>
    /// Wrapper for DateTime.Now so we can unit test methods using Now, by doing: SystemTime.Now = () => new DateTime(2009,1,1);
    /// </summary>
    public static class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.Now;
    }
}
