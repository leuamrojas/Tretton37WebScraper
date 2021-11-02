namespace Tretton37WebScraper.Configuration
{
    public class ConfigurationSettings : BaseConfiguration
    {
        public static string ApplicationName
        {
            get { return (string)GetAppSetting(typeof(string), "ApplicationName"); }
        }

        public static string BaseUrl
        {
            get { return (string)GetAppSetting(typeof(string), "BaseUrl"); }
        }

        public static string BasePath
        {
            get { return (string)GetAppSetting(typeof(string), "BasePath"); }
        }
    }
}
