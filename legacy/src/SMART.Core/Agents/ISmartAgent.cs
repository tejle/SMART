namespace SMART.Core.Agents
{
    public interface ISmartAgent
    {
        void Run();
        void Stop();
        SmartAgentStatus Status { get; }
    }
}