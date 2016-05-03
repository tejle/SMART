
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;
using Microsoft.Practices.Unity;

namespace SMART.Core.DomainModel
{
    public class Transition : ModelElement
    {
        private List<string> parameters;
        private string action;
        private State source;
        private State destination;
        private string guard;

        public ReadOnlyCollection<string> Parameters
        {
            get
            {
                return new ReadOnlyCollection<string>(parameters);
            }
        }

        [Config]
        public State Source
        {
            get { return source; }
            set { source = value; OnPropertyChanged("Source");}
        }

        [Config]
        public State Destination
        {
            get { return destination; }
            set { destination = value; OnPropertyChanged("Destination");}
        }

        [Config]
        public string Parameter
        {
            get
            {
                return string.Join(";", parameters.ToArray());
            }
            set { parameters = new List<string>(value.Split(';')); OnPropertyChanged("Parameters");}
        }

        [Config]
        public string Guard
        {
            get { return guard; }
            set { guard = value; OnPropertyChanged("Guard");}
        }

        [Config]
        public string Action
        {
            get { return action; }
            set { action = value; OnPropertyChanged("Action");}
        }

        [InjectionConstructor]
        public Transition() : this(string.Empty) { }
        public Transition(string label) : this(label, Guid.NewGuid()) { }
        public Transition(string label, Guid id)
            : base(label, id)
        {
            parameters = new List<string>(); 
        }

    }
}