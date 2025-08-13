using Solnet.Core.Contracts;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Solnet.Core.Metadata
{
    public static class HostMetadataCache
    {
        static bool? _isRunningInContainer;
        static bool? _isRunningInKubernetes;

#if !NETFRAMEWORK
        static bool? _isNetFramework;
#endif
        public static HostInfo Host => Cached.HostInfo;
        public static HostInfo Empty => Cached.EmptyHostInfo;

        public static bool IsRunningInContainer =>
            _isRunningInContainer ??= bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var inDocker) && inDocker;

        public static bool IsKubernetes =>
            _isRunningInKubernetes ??= Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST") != null
                || Directory.Exists("/var/run/secrets/kubernetes.io");

#if NETFRAMEWORK
        public static bool IsNetFramework => true;
#else
        public static bool IsNetFramework => _isNetFramework ??= RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework");
#endif

        public static string? GetCommitHash()
        {
            var assembly = typeof(ISolnet).Assembly;

            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute == null)
                return null;

            var splitIndex = attribute.InformationalVersion.IndexOf('+');
            if (splitIndex > 0)
                return attribute.InformationalVersion.Substring(splitIndex + 1);

            return null;
        }
    }


    static class Cached
    {
        internal static readonly HostInfo HostInfo = new SolnetHostInfo(true);
        internal static readonly HostInfo EmptyHostInfo = new SolnetHostInfo();
    }
}
