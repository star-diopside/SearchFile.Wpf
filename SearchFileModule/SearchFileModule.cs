using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using SearchFile.Module.Views;

namespace SearchFile.Module
{
    public class SearchFileModule : IModule
    {
        [Dependency]
        public IRegionManager RegionManager { private get; set; }

        public void Initialize()
        {
            this.RegionManager.RegisterViewWithRegion("MainRegion", typeof(SearchFileView));
        }
    }
}
