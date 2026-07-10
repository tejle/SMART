using System;
using System.Collections.Generic;
using System.Linq;
using SMART.Core.Metadata;
using SMART.Core.Interfaces;
using Microsoft.Practices.Unity;

using SMART.Core.Events;

namespace SMART.Core.DomainModel {
    using System.Collections;

    public class Testcase : SmartEntityCollectionBase, ITestcase
    {
        private Guid id;
        private string name;
        private List<IModel> models;
        private List<IAlgorithm> algorithms;
        private List<IGenerationStopCriteria> generationStopCriterias;
        private List<IExecutionStopCriteria> executionStopCriteriases;
        private List<IAdapter> adapters;

        
        public Guid Id { get { return id; } set { id = value; OnPropertyChanged("Id"); } }
        [Config(Description = "The name for the testcase")]
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        public IEnumerable<IModel> Models
        {
            get { return from m in models select m; }

            set { models = new List<IModel>(value); }
        }

        // Used for generation
        public IEnumerable<IAlgorithm> Algorithms { get { return from a in algorithms select a; } set{ algorithms = new List<IAlgorithm>(value);} }
        public IEnumerable<IGenerationStopCriteria> GenerationStopCriterias { get { return from g in generationStopCriterias select g; }  set{generationStopCriterias = new List<IGenerationStopCriteria>(value);}}

        // Used for execution
        public IEnumerable<IExecutionStopCriteria> ExecutionStopCriteriasas { get { return from e in executionStopCriteriases select e;} set{executionStopCriteriases = new List<IExecutionStopCriteria>(value);} }
        public IEnumerable<IAdapter> Adapters { get { return from a in adapters select a;} set{adapters= new List<IAdapter>(value);}}

        [InjectionConstructor]
        public Testcase() : this("New TestCase") { }
        public Testcase(string name) : this(Guid.NewGuid(), name) { }
        public Testcase(Guid id, string name) {
            Id = id;
            Name = name;

            models = new List<IModel>();
            algorithms = new List<IAlgorithm>();
            generationStopCriterias = new List<IGenerationStopCriteria>();
            executionStopCriteriases = new List<IExecutionStopCriteria>();
            adapters = new List<IAdapter>();
        }


        public Testcase Add(IModel model) {
            models.Add(model);
            SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "Models", new List<IModel>(new[] {model}));
            return this;
        }

        public Testcase Add(List<IModel> elements)
        {
            models.AddRange(elements);
            SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "Models", new List<IModel>(elements));
            return this;
        }

        public Testcase Add(IAlgorithm algorithm) {
            algorithms.Add(algorithm);
            this.SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "Algorithms", new List<IAlgorithm>(new[] { algorithm }));
            return this;
        }

        public Testcase Add(IGenerationStopCriteria criteria) {
            generationStopCriterias.Add(criteria);
            this.SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "GenerationStopCriterias", new List<IGenerationStopCriteria>(new[] { criteria }));
            return this;
        }

        public Testcase Add(IExecutionStopCriteria criteria) {
            executionStopCriteriases.Add(criteria);
            this.SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "ExecutionStopCriterias", new List<IExecutionStopCriteria>(new[] { criteria }));
            return this;
        }

        public Testcase Add(IAdapter adapter) {
            adapters.Add(adapter);
            this.SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "Adapters", new List<IAdapter>(new [] {adapter} ));
            return this;
        }

        public Testcase Remove(IModel model) {
            if (models.Contains(model))
            {
                models.Remove(model);
                SendCollectionChanged(SmartNotifyCollectionChangedAction.Remove, "Models", new List<IModel>(new[] { model }));
            }
            return this;
        }

        public Testcase Remove(IAlgorithm algorithm) {
            if (algorithms.Contains(algorithm))
            {
                algorithms.Remove(algorithm);
                this.SendCollectionChanged(SmartNotifyCollectionChangedAction.Remove, "Algorithms", new List<IAlgorithm>(new[] { algorithm }));
            }
            return this;
        }

        public Testcase Remove(IGenerationStopCriteria criteria) {
            if (generationStopCriterias.Contains(criteria))
            {
                generationStopCriterias.Remove(criteria);
                this.SendCollectionChanged(SmartNotifyCollectionChangedAction.Remove, "GenerationStopCriterias", new List<IGenerationStopCriteria>(new[] { criteria }));
            }
            return this;
        }

        public Testcase Remove(IExecutionStopCriteria criteria) {
            if (executionStopCriteriases.Contains(criteria))
            {
                executionStopCriteriases.Remove(criteria);
                this.SendCollectionChanged(SmartNotifyCollectionChangedAction.Remove, "ExecutionStopCriterias", new List<IExecutionStopCriteria>(new[] { criteria }));
            }
            return this;
        }

        public Testcase Remove(IAdapter adapter) {
            if (adapters.Contains(adapter))
            {
                adapters.Remove(adapter);
                this.SendCollectionChanged(SmartNotifyCollectionChangedAction.Remove, "Adapters", new List<IAdapter>(new[] { adapter }));
            }
            return this;
        }

    }
}
