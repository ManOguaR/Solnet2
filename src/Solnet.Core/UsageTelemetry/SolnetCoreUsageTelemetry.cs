namespace Solnet.Core.UsageTelemetry
{
    public class SolnetCoreUsageTelemetry
    {
        public Guid? Id { get; set; }
        public string? CustomerId { get; set; }
        public string? Created { get; set; }
        public HostUsageTelemetry? Host { get; set; }
        //public List<SolnetUsageTelemetry>? Solnet { get; set; }
        //public List<RiderUsageTelemetry>? Rider { get; set; }
    }
}
