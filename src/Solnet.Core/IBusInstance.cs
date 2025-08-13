namespace Solnet.Core
{
    public interface ISolnetInstance :
           IReceiveEndpointConnector
    {
        string Name { get; }
        Type InstanceType { get; }

        ISolnet Solnet { get; }
        ISolnetControl SolnetControl { get; }

        IHostConfiguration HostConfiguration { get; }

        void Connect<TRider>(IRiderControl riderControl)
            where TRider : IRider;

        TRider GetRider<TRider>()
            where TRider : IRider;
    }


    public interface ISolnetInstance<out TSolnet> :
        ISolnetInstance
        where TSolnet : ISolnet
    {
        new TSolnet Solnet { get; }

        /// <summary>
        /// The original solnet instance (since this is wrapped inside a multi-solnet instance
        /// </summary>
        ISolnetInstance SolnetInstance { get; }
    }
}
