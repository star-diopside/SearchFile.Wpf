using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SearchFile.Wpf.Module.Views;

namespace SearchFile.Wpf.Module
{
    public class SearchFileModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("MainRegion", typeof(SearchFileView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
