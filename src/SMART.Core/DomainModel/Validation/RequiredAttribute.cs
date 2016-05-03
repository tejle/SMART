namespace SMART.Core.DomainModel.Validation
{
    public class RequiredAttribute : ValidatorBase
    {
        public override string Validate(string name, object value)
        {
            if (string.IsNullOrEmpty(value as string)) 
                return Message(name, name + " must be supplied.");
            
            return null;
        }
    }
}