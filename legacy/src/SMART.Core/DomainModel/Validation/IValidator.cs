
namespace SMART.Core.DomainModel.Validation
{
    public interface IValidator
    {
        string Validate(string name, object value);
    }
}