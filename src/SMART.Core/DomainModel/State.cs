using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SMART.Core.Metadata;
using Microsoft.Practices.Unity;


namespace SMART.Core.DomainModel
{
    public class State : ModelElement
    {
        private SmartPoint location;
        private SmartSize size;

        [Config(Default = StateType.Normal)]
        public StateType Type { get; set; }

        [Config]
        public SmartPoint Location
        {
            get { if (location == null) location = new SmartPoint(0, 0); return this.location; }
            set { this.location = value; OnPropertyChanged("Location"); }
        }

        [Config]
        public SmartSize Size
        {
            get { if (size == null) size = new SmartSize(0, 0); return this.size; }
            set { this.size = value; OnPropertyChanged("Size"); }
        }

        protected readonly List<Transition> transitions;

        public ReadOnlyCollection<Transition> Transitions
        {
            get { return new ReadOnlyCollection<Transition>(transitions); }
        }

        public ReadOnlyCollection<Transition> OutTransitions
        {
            get
            {
                var out_t = from t in transitions where t.Source == this select t;
                return new ReadOnlyCollection<Transition>(out_t.ToList());
            }
        }

        public ReadOnlyCollection<Transition> InTransitions
        {
            get
            {
                var in_t = from t in transitions where t.Destination == this select t;
                return new ReadOnlyCollection<Transition>(in_t.ToList());
            }
        }

        [InjectionConstructor]
        public State() : this(string.Empty) { }
        public State(string label) : this(label, Guid.NewGuid()) { }
        public State(string label, Guid id)
            : base(label, id)
        {
            transitions = new List<Transition>();
        }

        public void Add(Transition transition)
        {
            if (!transitions.Contains(transition)) transitions.Add(transition);
        }

		public void Add(IEnumerable<Transition> trs)
		{
			transitions.AddRange(trs); 
		}

        public void Remove(Transition transition)
        {
            if (transitions.Contains(transition)) transitions.Remove(transition);
        }

		public void ClearTransitions()
		{
			transitions.Clear();
		}
        
    }
}