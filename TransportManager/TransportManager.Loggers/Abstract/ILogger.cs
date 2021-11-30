namespace TransportManager.Loggers.Abstract
{
    public interface ILogger
    {
        void Trace(string log);
        void Error(string log);
    }
}