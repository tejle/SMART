using System;
using System.Collections.Generic;
using System.Reflection;

namespace SMART.Core.DomainModel.Validation
{
    public static class ValidationManager
    {
        public static string Validate(string name, object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            var type = instance.GetType();
            var errorList = new List<string>();

            if (string.IsNullOrEmpty(name))
            {
                foreach (var pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    ValidateProperty(pi.Name, instance, errorList);
            }
            else
            {
                ValidateProperty(name, instance, errorList);
            }

            return string.Join(Environment.NewLine, errorList.ToArray());
        }

        static void ValidateProperty(string name, object instance, ICollection<string> errorList)
        {
            var type = instance.GetType();
            var pi = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi == null) return;
            
            var value = pi.GetValue(instance, null);
            foreach (var att in pi.GetCustomAttributes(true))
            {
                var iv = att as IValidator;
                if (iv == null) continue;
                
                var err = iv.Validate(name, value);
                if (!string.IsNullOrEmpty(err))
                    errorList.Add(err);
            }
        }
    }
}