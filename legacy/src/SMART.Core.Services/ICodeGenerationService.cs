using SMART.Core.Interfaces;

namespace SMART.Core.Services
{
    public interface ICodeGenerationService
    {
        string GetCodeAsString(IProject project, ITestcase testcase);

    }
}