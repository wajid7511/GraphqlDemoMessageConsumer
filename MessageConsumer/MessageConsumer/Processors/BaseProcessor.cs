using GraphQlDemo.Shared.Messaging;

namespace MessageConsumer.Processors;


public abstract class BaseProcessor
{
    public abstract bool CanProcess(MessageType messageType);

    public virtual ValueTask<bool> ProcessAsync(MessageDto messageDto)
    {
        return ValueTask.FromResult(true);
    }
}
