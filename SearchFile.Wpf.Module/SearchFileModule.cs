using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SearchFile.Wpf.Module.Views;

namespace SearchFile.Wpf.Module
{
    public class SearchFileModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public SearchFileModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(SearchFileView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
