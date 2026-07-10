using System.IO;

namespace SMART.Core.DomainModel.Validation
{
    public class FileMustExistAttribute : ValidatorBase
    {
        public override string Validate(string name, object value)
        {
          if (value == null) value = string.Empty;

            if (!File.Exists(value.ToString()))
                return string.Format("[{0}] {1} does not exist.", name, value);
            return null;
        }
    }
}