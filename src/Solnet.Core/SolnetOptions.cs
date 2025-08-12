namespace Solnet.Core
{
    public class SolnetOptions
    {
        public string? HttpEndpoint { get; set; }
        public string? WebSocketEndpoint { get; set; }
        public string? DefaultCommitment { get; set; }
        public int RequestTimeout { get; set; } = 300;
        public string? Cluster { get; set; }
        public string? FeePayer { get; set; }
        public int MaxRetries { get; set; } = 3;
        public string? Backoff { get; set; }
    }
}
