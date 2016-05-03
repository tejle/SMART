using System;
using System.Collections.Generic;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Factories;
using SMART.Core.Interfaces.Repository;
using SMART.Core.Interfaces.Services;
using SMART.IOC;

namespace SMART.Core.Services
{
    public class SimpleStatisticsService : IStatisticsService {
        private List<IStatistic> statistics;

        public List<IStatistic> Statistics { get { return statistics; }set{ statistics = value;} }
        
        public SimpleStatisticsService(IEventService eventService)
        {
            this.statistics = new List<IStatistic>();
            eventService.GetEvent<TestGenerationEvent>().Subscribe(OnTestGenerationEvent);
        }

        private void OnTestGenerationEvent(IModelElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            NotifyStatisicsBefore(element);
            element.Visit();
            NotifyStatisicsAfter(element);
        }

        private void NotifyStatisicsBefore(IModelElement element)
        {
            foreach (var statistic in statistics)
            {
                statistic.BeforeVisit(element);
            }
        }

        private void NotifyStatisicsAfter(IModelElement element)
        {
            foreach (var statistic in statistics)
            {
                statistic.AfterVisit(element);
            }
        }

        public double GetStatisticValue(IStatistic statistic, IModel model)
        {
            return statistics.Find(s => s == statistic).Calculate(model);            
        }
    }
}