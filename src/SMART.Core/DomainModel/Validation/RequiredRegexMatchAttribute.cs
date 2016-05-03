using System.Text.RegularExpressions;

namespace SMART.Core.DomainModel.Validation
{
    public class RequiredRegexMatchAttribute : ValidatorBase
    {
        public string Expression { get; set; }

        public override string Validate(string name, object value)
        {
            if (value == null || Regex.IsMatch(value.ToString(), Expression))
                return null;

            return Message(name, name + " does not match required format.");
        }
    }
}