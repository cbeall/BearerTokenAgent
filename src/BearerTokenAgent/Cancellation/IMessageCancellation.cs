using System.Threading;

namespace BearerTokenAgent.Cancellation
{
    public interface IMessageCancellation
    {
        CancellationToken GetCancellationToken();
    }
}
