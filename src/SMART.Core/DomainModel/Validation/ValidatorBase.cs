using System;

namespace SMART.Core.DomainModel.Validation
{
    public abstract class ValidatorBase : Attribute, IValidator
    {
        public string InvalidMessage { get; set; }
        protected string Message(string name, string defaultMessage)
        {
            return InvalidMessage == null ? defaultMessage : string.Format(InvalidMessage, name);
        }
        public abstract string Validate(string name, object value);

    }
}