using System.Collections.Generic;
using SMART.Core.Interfaces;

namespace SMART.Core.DomainModel
{
    public  interface IStatisticsService
    {
        List<IStatistic> Statistics { get; set; }
    }
}