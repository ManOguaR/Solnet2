using Solnet.Core.Contracts;
using System.Diagnostics;
using System.Reflection;

namespace Solnet.Core.Metadata
{
    [Serializable]
    public class SolnetHostInfo :
        HostInfo
    {
        public SolnetHostInfo()
        {
        }

        public SolnetHostInfo(bool initialize)
        {
            FrameworkVersion = Environment.Version.ToString();
            OperatingSystemVersion = Environment.OSVersion.ToString();
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetCallingAssembly();
            MachineName = Environment.MachineName;
            SolnetCoreVersion = typeof(HostInfo).Assembly.GetName().Version?.ToString();

            try
            {
                using var currentProcess = Process.GetCurrentProcess();
                ProcessId = currentProcess.Id;
                ProcessName = currentProcess.ProcessName;
                if ("dotnet".Equals(ProcessName, StringComparison.OrdinalIgnoreCase))
                    ProcessName = GetUsefulProcessName(ProcessName);
            }
            catch (NotSupportedException)
            {
                ProcessId = 0;
                ProcessName = GetUsefulProcessName("UWP");
            }

            var assemblyName = entryAssembly.GetName();
            Assembly = assemblyName.Name;
            AssemblyVersion = assemblyName.Version?.ToString() ?? "Unknown";
        }

        public string? MachineName { get; set; }
        public string? ProcessName { get; set; }
        public int ProcessId { get; set; }
        public string? Assembly { get; set; }
        public string? AssemblyVersion { get; set; }
        public string? FrameworkVersion { get; set; }
        public string? SolnetCoreVersion { get; set; }
        public string? OperatingSystemVersion { get; set; }

        static string GetAssemblyFileVersion(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (attribute != null)
                return attribute.Version;

            return FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion ?? "Unknown";
        }

        static string GetAssemblyInformationalVersion(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute != null)
                return attribute.InformationalVersion;

            return GetAssemblyFileVersion(assembly);
        }

        static string GetUsefulProcessName(string defaultProcessName)
        {
            var entryAssemblyLocation = System.Reflection.Assembly.GetEntryAssembly()?.Location;

            return string.IsNullOrWhiteSpace(entryAssemblyLocation)
                ? defaultProcessName
                : Path.GetFileNameWithoutExtension(entryAssemblyLocation);
        }
    }
}