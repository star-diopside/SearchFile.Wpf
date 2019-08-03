using Prism.Ioc;
using Prism.Modularity;
using SearchFile.Wpf.App.Views;
using SearchFile.Wpf.Module;
using System.Windows;

namespace SearchFile.Wpf.App
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override Window CreateShell()
        {
            return this.Container.Resolve<Shell>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<SearchFileModule>();
        }
    }
}
