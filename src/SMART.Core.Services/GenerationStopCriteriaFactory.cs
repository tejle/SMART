using System;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Factories;
using SMART.IOC;

namespace SMART.Core.Services
{
    public class GenerationStopCriteriaFactory : IGenerationStopCriteriaFactory
    {
        public IGenerationStopCriteria Create(Type type)
        {
            return Resolver.ResolveType(type) as IGenerationStopCriteria;
        }
    }
}