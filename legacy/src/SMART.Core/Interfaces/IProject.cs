
using System;
using SMART.Core.Interfaces.Reporting;

namespace SMART.Core.Interfaces
{
    using System.Collections.Generic;
    using BusinessLayer;
    using DomainModel;

    public interface IProject : ISmartNotifyPropertyChanged, ISmartNotifyCollectionChanged
    {
        Guid Id { get; set; }
        string Name { get; set; }
        IEnumerable<ITestcase> Testcases { get; }
        IEnumerable<IModel> Models { get; }
        IEnumerable<IReport> Reports { get; }

        bool AddModel(IModel model, ITestcase testcase);
        bool AddModel(IModel model);
        bool RemoveModel(IModel model);
        bool AddTestCase(ITestcase testCase);
        bool RemoveTestCase(ITestcase testCase);
        bool AddReport(IReport report);
        
        //void StartExecution();
        //void StopExecution();
        //void SetExecutionPolicy(ExecutionPolicy policy);
        //ExecutionPolicy GetExecutionPolicy();
        //ITestcaseExecutor TestcaseExecutor { get; set; }        
    }
}