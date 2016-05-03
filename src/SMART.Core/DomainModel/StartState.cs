using System;

namespace SMART.Core.DomainModel
{
    public class StartState : State
    {
        public StartState() :base("Start")
        {
         Size = new SmartSize(50,50);   
        }
    }
}