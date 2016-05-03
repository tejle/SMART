using System;
using System.Collections.Generic;
using System.Windows.Controls;
using SMART.Core.DataLayer;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.DomainModel;
using SMART.Core.Events;

namespace SMART.Core.Interfaces
{
    public interface IModel : ISmartNotifyPropertyChanged, ISmartNotifyCollectionChanged, IIdentifyable 
    {
        string Name { get; set; }
        StartState StartState { get; set; }
        StopState StopState { get; set; }
        List<Transition> Transitions { get; }
        List<State> States { get; }
        IEnumerable<IModelElement> Elements { get; }
        SmartImage Thumbnail { get; set; }
        IModel Add(State state);
        IModel Add(IList<State> states);
        IModel Add(Transition transition);
        IModel Add(IList<Transition> transitions);
        IModel Remove(State state);
        IModel Remove(Transition transition);
        void ChangeTransitionSource(Transition transition, State newSource);
        void ChangeTransitionDestination(Transition transition, State newDestination);
    }
}