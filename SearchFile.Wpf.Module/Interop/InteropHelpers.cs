using SearchFile.Wpf.Module.Interop.CommonControlObjects;
using SearchFile.Wpf.Module.Interop.ShellObjects;

namespace SearchFile.Wpf.Module.Interop
{
    internal static class InteropHelpers
    {
        internal static IShellItem CreateShellItem(string path)
        {
            var iid = typeof(IShellItem).GUID;
            return (IShellItem)InteropMethods.SHCreateItemFromParsingName(path, null, ref iid);
        }

        internal static IImageList GetImageList(SHIL imageList)
        {
            var iid = typeof(IImageList).GUID;
            return (IImageList)InteropMethods.SHGetImageList(imageList, ref iid);
        }
    }
}
