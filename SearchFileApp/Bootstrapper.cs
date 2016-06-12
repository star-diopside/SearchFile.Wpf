using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using SearchFile.Views;
using System.Windows;

namespace SearchFile
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog catalog = (ModuleCatalog)this.ModuleCatalog;
            catalog.AddModule(typeof(SearchFileModule));
        }
    }
}
