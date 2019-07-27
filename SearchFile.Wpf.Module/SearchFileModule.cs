using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SearchFile.Wpf.Module.Views;
using Unity;

namespace SearchFile.Wpf.Module
{
    public class SearchFileModule : IModule
    {
        [Dependency]
        public IRegionManager RegionManager { private get; set; }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.RegionManager.RegisterViewWithRegion("MainRegion", typeof(SearchFileView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
