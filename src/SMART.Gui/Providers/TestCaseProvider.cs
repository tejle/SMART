using System;

namespace SMART.Gui.Providers
{
    public class TestCaseProvider : ITestCaseProvider
    {
        //public ITestCase GetTestCase()
        //{
        //    return App.CurrentProject.TestCases.Find(tc => tc.ID == _id);
        //}

        private Guid _id;

        public TestCaseProvider(Guid id)
        {
            _id = id;
        }

    }
}
