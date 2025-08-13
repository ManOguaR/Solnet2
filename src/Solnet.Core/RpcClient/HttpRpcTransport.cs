using Microsoft.Extensions.Options;

namespace Solnet.Core.RpcClient
{
    public sealed class HttpRpcTransport(HttpClient http, IOptions<RpcClientOptions> opts) : IRpcTransport
    {
        public async Task<HttpResponseMessage> PostAsync(HttpContent content, CancellationToken ct)
            => await http.PostAsync("", content, ct).ConfigureAwait(false);
    }
}
