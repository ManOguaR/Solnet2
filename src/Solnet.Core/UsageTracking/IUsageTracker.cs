namespace Solnet.Core.UsageTracking
{
    public interface IUsageTracker
    {
        public void PreConfigureSolnet<T>(T configurator, ISolnetRegistrationContext context)
            where T : ISolnetFactoryConfigurator;

        void PreConfigureRider<T>(T configurator)
            where T : IRiderFactoryConfigurator;
    }
}
