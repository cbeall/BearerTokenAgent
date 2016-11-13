using System.Net.Http;

namespace BearerTokenAgent
{
    public interface IHttpMessageInvokerFactory
    {
        HttpMessageInvoker CreateHttpMessageInvoker();
    }
}
