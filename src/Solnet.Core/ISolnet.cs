namespace Solnet.Core
{
    public interface ISolnet
    {
        /// <summary>
        /// The InputAddress of the default bus endpoint
        /// </summary>
        Uri Address { get; }
    }
}
