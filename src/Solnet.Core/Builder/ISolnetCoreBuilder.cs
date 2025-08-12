using Solnet.Core.Rpc;

namespace Solnet.Core.Builder
{
    public interface ISolnetCoreBuilder
    {
        void Configure(Action<SolnetOptions> configureOptions);

        void UseSigner(ISignerService impl);

        void UseSigner<TSigner>()
            where TSigner : ISignerService;

        void AddProgram<TProgramAbstraction>();

        void AddPolicy(Action<ResilienceOptions> configureOptions);

        void AddLogging();
    }
}
