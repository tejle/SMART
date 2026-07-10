using System;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Factories;
using SMART.IOC;

namespace SMART.Core.Services
{
    public class StatisticsFactory : IStatisticsFactory
    {
        public IStatistic Create(Type type)
        {
            return Resolver.ResolveType(type) as IStatistic;
        }
    }
}