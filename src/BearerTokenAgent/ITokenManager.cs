using System.Threading;
using System.Threading.Tasks;

namespace BearerTokenAgent
{
    public interface ITokenManager
    {
        Task<string> GetTokenAsync(CancellationToken cancellationToken);
    }
}