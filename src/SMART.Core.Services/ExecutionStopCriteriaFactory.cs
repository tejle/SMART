using SMART.Core.Interfaces.Factories;

namespace SMART.Core.Services
{
    using System;

    using Interfaces;

    using IOC;

    public class ExecutionStopCriteriaFactory : IExecutionStopCriteriaFactory
    {
        public IExecutionStopCriteria Create(Type type)
        {
            return Resolver.ResolveType(type) as IExecutionStopCriteria;
        }
    }
}