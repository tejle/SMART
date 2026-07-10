namespace SMART.Core.DomainModel
{
    public class StopState : State
    {
        public StopState() : base("Stop")
        {            
            Location = new SmartPoint(300,0);
            Size = new SmartSize(50, 50);   
        }
    }
}