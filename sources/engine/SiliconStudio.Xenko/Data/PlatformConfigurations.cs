using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SiliconStudio.Core;

namespace SiliconStudio.Xenko.Data
{
    [DataContract]
    public class PlatformConfigurations
    {
        public static string RendererName = string.Empty;

        public static string DeviceModel = string.Empty;

        [DataMember]
        public List<ConfigurationOverride> Configurations = new List<ConfigurationOverride>();

        [DataMember]
        public List<string> PlatformFilters = new List<string>(); 

        public T Get<T>() where T : Configuration, new()
        {
            //find default
            var config = Configurations.Where(x => x.Platforms == ConfigPlatforms.None).FirstOrDefault(x => x.Configuration is T);
            
            //perform logic by platform and if required even gpu/cpu/specs

            var platform = ConfigPlatforms.None;
            switch (Platform.Type)
            {
                case PlatformType.Shared:
                    break;
                case PlatformType.Windows:
                    platform = ConfigPlatforms.Windows;
                    break;
                case PlatformType.WindowsPhone:
                    platform = ConfigPlatforms.WindowsPhone;
                    break;
                case PlatformType.WindowsStore:
                    platform = ConfigPlatforms.WindowsStore;
                    break;
                case PlatformType.Android:
                    platform = ConfigPlatforms.Android;
                    break;
                case PlatformType.iOS:
                    platform = ConfigPlatforms.iOS;
                    break;
                case PlatformType.Windows10:
                    platform = ConfigPlatforms.Windows10;
                    break;
                case PlatformType.Linux:
                    platform = ConfigPlatforms.Linux;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //find per platform if available
            var platformConfig = Configurations.Where(x => x.Platforms.HasFlag(platform) && x.SpecificFilter == -1).FirstOrDefault(x => x.Configuration is T);
            if (platformConfig != null)
            {
                config = platformConfig;
            }

            //find per specific renderer settings
            platformConfig = Configurations.Where(x => x.Platforms.HasFlag(platform) && x.SpecificFilter != -1 && new Regex(PlatformFilters[x.SpecificFilter], RegexOptions.IgnoreCase).IsMatch(RendererName)).FirstOrDefault(x => x.Configuration is T);
            if (platformConfig != null)
            {
                config = platformConfig;
            }

            //find per specific device settings
            platformConfig = Configurations.Where(x => x.Platforms.HasFlag(platform) && x.SpecificFilter != -1 && new Regex(PlatformFilters[x.SpecificFilter], RegexOptions.IgnoreCase).IsMatch(DeviceModel)).FirstOrDefault(x => x.Configuration is T);
            if (platformConfig != null)
            {
                config = platformConfig;
            }

            if (config == null)
            {
                return new T();
            }

            return (T)config.Configuration;
        }
    }
}
