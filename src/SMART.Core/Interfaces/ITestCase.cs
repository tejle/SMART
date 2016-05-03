using System.Collections.Generic;
using System.Collections.ObjectModel;
using SMART.Core.BusinessLayer;

using System;
using SMART.Core.DomainModel;

namespace SMART.Core.Interfaces
{

    public interface ITestcase : ISmartNotifyPropertyChanged, ISmartNotifyCollectionChanged {
       
        Guid Id { get; set; }
        string Name { get; set; }

        IEnumerable<IModel> Models { get; set; }
        IEnumerable<IAlgorithm> Algorithms { get; set; }
        IEnumerable<IGenerationStopCriteria> GenerationStopCriterias { get; set; }
        IEnumerable<IExecutionStopCriteria> ExecutionStopCriteriasas { get; set; }
        IEnumerable<IAdapter> Adapters { get; set; }
        Testcase Add(IModel model);
        Testcase Add(IAlgorithm algorithm);
        Testcase Add(IGenerationStopCriteria criteria);
        Testcase Add(IExecutionStopCriteria criteria);
        Testcase Add(IAdapter adapter);
        Testcase Remove(IModel model);
        Testcase Remove(IAlgorithm algorithm);
        Testcase Remove(IGenerationStopCriteria criteria);
        Testcase Remove(IExecutionStopCriteria criteria);
        Testcase Remove(IAdapter adapter);
    }


    //public interface ITestcase : ISmartNotifyPropertyChanged
    //{
    //    IEnumerable<IAdapter> Adapters { get; set; }
    //    Model ExecutableModel { get; }
    //    IStatisticsManager StatisticsManager { get; }
    //    ISandbox Sandbox { get; }

    //    IEnumerable<Model> Models { get; set; }
        
    //    string Name { get; set; }
    //    Guid Id { get; }
    //    IList<Tuple<IAlgorithm, IStopCriteria>> ExecutionPath { get; set; }
    //    bool Blocked { get; set; }

    //    bool AddModel(Model model);

    //    //void Execute();
    //    void PauseExecution();
    //    void StartExecution();
    //    void StopExecution();
    //    void RestartExecution();
    //    int DelayBetweenExecutionSteps { get; set; }
    //    Model StartUpModel { get; set; }
    //    event EventHandler TestCaseExecutionComplete;
    //    bool RemoveModel(Model model);
    //    void ResumeExecution();
    //}
}