namespace TransportManager.Logger.Abstract
{
    public interface ILogger
    {
        void Trace(string log);
        void Error(string log);
    }
}