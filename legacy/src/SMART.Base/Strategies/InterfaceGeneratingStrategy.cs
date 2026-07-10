using System;
using System.Collections.Generic;
using SMART.Base.Adapters;
using SMART.Base.Statistics;
using SMART.Base.StopCriterias;
using SMART.Core;
using SMART.Core.Interfaces;
using SMART.IOC;

namespace SMART.Base.Strategies
{
    public class InterfaceGeneratingStrategy //: IStrategy
    {
        public IEnumerable<Tuple<IAlgorithm, IStopCriteria>> Path { get; private set; }
        public IEnumerable<IAdapter> Adapters { get; private set; }

        public InterfaceGeneratingStrategy()
        {
            Adapters = new[] { GetAdapter<InterfaceAdapter>() };

            Path = new[]
                       {
                           new Tuple<IAlgorithm, IStopCriteria>(
                               new OrderedAlgortihm(), 
                               new AndStopCriteria
                                   {
                                       Criterias = new[]
                                                       {
                                                           GetStopCriteria<TransitionCoverageStatistic>(1d),
                                                           GetStopCriteria<StateCoverageStatistic>(1d)
                                                       }
                                   })
                       };
        }

        private static IAdapter GetAdapter<TAdapter>() where TAdapter : IAdapter
        {
            var adapter = Resolver.Resolve<TAdapter>();
            var adapterConfig = adapter.GetConfig();
            adapterConfig.Update("InterfaceName", "TestInterface");
            adapter.SetConfig(adapterConfig);
            return adapter;
        }

        private static IStopCriteria GetStopCriteria<TStatistic>(double stopLimit) where TStatistic : IStatistic
        {
            var stateCriteria = Resolver.Resolve<StatisticalStopCriteria>();
            var stateCriteriaConfig = stateCriteria.GetConfig();
            stateCriteriaConfig.Update("Statistic", Resolver.Resolve<TStatistic>());
            stateCriteriaConfig.Update("StopLimit", stopLimit);
            stateCriteria.SetConfig(stateCriteriaConfig);
            return stateCriteria;
        }
    }
}