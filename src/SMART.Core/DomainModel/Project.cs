using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SMART.Core.BusinessLayer;
using SMART.Core.Events;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Reporting;
using SMART.Core.Metadata;
using Microsoft.Practices.Unity;

namespace SMART.Core.DomainModel
{
    public class Project : SmartEntityCollectionBase, IProject
    {
        private string name;
        private IList<ITestcase> testcases;
        private IList<IModel> models;
        private IList<IReport> reports;

        public Guid Id
        {
            get;  set;
        }

        [Config]
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                OnPropertyChanged("Name");
            }
        }

        [InjectionConstructor]
        public Project() :this(Guid.NewGuid()){}
        
        public Project(Guid id)
        {
            Id = id;
            Models = new List<IModel>();
            Testcases = new List<ITestcase>();
            Reports = new List<IReport>();
        }

        public IEnumerable<ITestcase> Testcases
        {
            get { return testcases; }
            set { testcases = new List<ITestcase>(value); }
        }

        public IEnumerable<IModel> Models
        {
            get { return models; }
            set { models = new List<IModel>(value); }
        }

        public IEnumerable<IReport> Reports
        {
            get { return reports; }
            set { reports = new List<IReport>(value);}
        }

        public bool AddModel(IModel model, ITestcase testcase)
        {
            if (models.Contains(model)) return false;
            if (testcase != null)
                testcase.Add(model);

            models.Add(model);
            SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "Models", new[] {model});
            //OnPropertyChanged("Models", model, SmartPropertyChangedAction.Add);
            return true;
        }
        public bool AddModel(IModel model)
        {
            return AddModel(model, null);
        }

        public bool RemoveModel(IModel model)
        {
            bool success = false;

            if (models.Contains(model))
            {
                success = true;
                var testcases = Testcases.Where(t => t.Models.Contains(model));
                foreach(var t in testcases)
                    success &= t.Remove(model) != null;
                success &= models.Remove(model);
                if(success)
                    SendCollectionChanged(SmartNotifyCollectionChangedAction.Remove, "Models", new[] { model });
                    //OnPropertyChanged("Models", model, SmartPropertyChangedAction.Remove);
            }
            return success;
        }

        public bool AddTestCase(ITestcase testCase)
        {
            if(testcases.Contains(testCase)) return false;
            testcases.Add(testCase);

            SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "Testcases", new[] {testCase});
            //OnPropertyChanged("Testcases", testCase, SmartPropertyChangedAction.Add);

            return true;
        }

        public bool AddReport(IReport report)
        {
            if (reports.Contains(report)) return false;
            reports.Add(report);

            SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "Reports", new[] { report });
            //OnPropertyChanged("Testcases", testCase, SmartPropertyChangedAction.Add);

            return true;
        }

        public bool RemoveTestCase(ITestcase testCase)
        {
            bool success = false;
            if(testcases.Contains(testCase))
            {
                success = testcases.Remove(testCase);
                if(success)
                    SendCollectionChanged(SmartNotifyCollectionChangedAction.Remove, "Testcases", new []{testCase});
                    //      OnPropertyChanged("Testcases", testCase, SmartPropertyChangedAction.Remove);
            }
            return success;
        }

    }
}