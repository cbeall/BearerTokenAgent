using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BearerTokenAgent.Cancellation
{
    public class CancellationManager : ICancellationManager
    {
        private readonly IEnumerable<IMessageCancellation> _messageCancellations;
        private IEnumerable<ActiveCancellationToken> _activeTokens;

        public CancellationManager(IEnumerable<IMessageCancellation> messageCancellations)
        {
            _messageCancellations = messageCancellations;
        }

        public CancellationToken GetCancellationToken()
        {
            _activeTokens = _messageCancellations
                .Select(a => new ActiveCancellationToken
                {
                    CancellationToken = a.GetCancellationToken(),
                    CancellationHandler = a
                })
                .ToList();

            var tokens = _activeTokens.Select(a => a.CancellationToken);

            if (!tokens.Any())
            {
                return CancellationToken.None;
            }

            CancellationTokenSource cancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(tokens.ToArray());

            return cancellationTokenSource.Token;
        }

        public Exception HandleCancellation(OperationCanceledException exception)
        {
            // TODO
            throw new NotImplementedException("need to implement how cancellation is handled");
        }

        private class ActiveCancellationToken
        {
            public CancellationToken CancellationToken { get; set; }
            public IMessageCancellation CancellationHandler { get; set; }
        }
    }
}
