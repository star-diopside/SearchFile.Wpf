using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using SearchFile.Wpf.App.Views;
using SearchFile.Wpf.Module;
using System.IO;
using System.Windows;
using Unity;
using Unity.Microsoft.Logging;

namespace SearchFile.Wpf.App
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            containerRegistry.GetContainer().AddExtension(new LoggingExtension(LoggerFactory.Create(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"))
                       .AddNLog(new NLogLoggingConfiguration(configuration.GetSection("NLog")));
            })));
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<SearchFileModule>();
        }
    }
}
