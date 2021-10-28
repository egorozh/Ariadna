using Serilog.Events;

namespace Ariadna;

public class LogObserver : IObserver<LogEvent>
{
    public event Action<LogEvent>? LogEventPushed;

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(LogEvent value)
    {
        LogEventPushed?.Invoke(value);
    }
}