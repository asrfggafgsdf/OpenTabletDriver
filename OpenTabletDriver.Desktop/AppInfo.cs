using System;
using System.IO;
using OpenTabletDriver.Desktop.Interop;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.Plugin;

namespace OpenTabletDriver.Desktop
{
    public class AppInfo
    {
        public static AppInfo Current { set; get; } = new AppInfo();

        public static DesktopPluginManager PluginManager { get; } = new DesktopPluginManager();

        private string configDirectory, appDataDirectory;

        public string ConfigurationDirectory
        {
            set => this.configDirectory = value;
            get => this.configDirectory ??= DefaultConfigurationDirectory;
        }

        public string AppDataDirectory
        {
            set => this.appDataDirectory = value;
            get => this.appDataDirectory ??= DefaultAppDataDirectory;
        }

        public string SettingsFile => Path.Join(AppDataDirectory, "settings.json");
        public string PluginDirectory => Path.Join(AppDataDirectory, "Plugins");
        public string TemporaryDirectory => Path.Join(AppDataDirectory, "Temp");
        public string CacheDirectory => Path.Join(AppDataDirectory, "Cache");
        public string TrashDirectory => Path.Join(AppDataDirectory, "Trash");

        private static string ProgramDirectory => AppContext.BaseDirectory;

        private static string DefaultConfigurationDirectory
        {
            get
            {
                var path = Path.Join(ProgramDirectory, "Configurations");
                var fallbackPath = Path.Join(Environment.CurrentDirectory, "Configurations");
                return Directory.Exists(path) ? path : fallbackPath;
            }
        }

        private static string DefaultAppDataDirectory
        {
            get
            {
                var path = Path.Join(ProgramDirectory, "userdata");
                var fallbackPath = SystemInterop.CurrentPlatform switch
                {
                    PluginPlatform.Windows => Path.Join(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "OpenTabletDriver"),
                    PluginPlatform.Linux   => Path.Join(Environment.GetEnvironmentVariable("HOME"), ".config", "OpenTabletDriver"),
                    PluginPlatform.MacOS   => Path.Join(Environment.GetEnvironmentVariable("HOME"), "Library", "Application Support", "OpenTabletDriver"),
                    _                       => null
                };
                return Directory.Exists(path) ? path : fallbackPath;
            }
        }
    }
}
