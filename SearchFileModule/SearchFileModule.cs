using Prism.Modularity;
using Prism.Regions;
using SearchFile.Views;

namespace SearchFile
{
    public class SearchFileModule : IModule
    {
        private IRegionManager regionManager;

        public SearchFileModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion("MainRegion", typeof(SearchFileControl));
        }
    }
}
