using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bathroom_checker_client
{
    public class Settings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public static string UserName
        {
            get { return AppSettings.GetValueOrDefault(nameof(UserName), string.Empty); }
            set { AppSettings.AddOrUpdateValue(nameof(UserName), value); }
        }

        public static string ServerIP
        {
            get { return AppSettings.GetValueOrDefault(nameof(ServerIP), string.Empty); }
            set { AppSettings.AddOrUpdateValue(nameof(ServerIP), value); }
        }
    }
}
