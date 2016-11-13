using System;
using System.Threading;

namespace BearerTokenAgent.Cancellation
{
    public interface ICancellationManager
    {
        CancellationToken GetCancellationToken();

        Exception HandleCancellation(OperationCanceledException exception);
    }
}
