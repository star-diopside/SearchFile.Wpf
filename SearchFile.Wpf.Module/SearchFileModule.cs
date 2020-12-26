using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SearchFile.Wpf.Module.Services;
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
            containerRegistry.RegisterSingleton<IExceptionService, ExceptionService>();
            containerRegistry.RegisterSingleton<IChooseFolderService, ChooseFolderService>();
            containerRegistry.RegisterSingleton<IDeleteFileService, DeleteFileService>();
            containerRegistry.RegisterSingleton<ISaveFileService, SaveFileService>();
        }
    }
}
