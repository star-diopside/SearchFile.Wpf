namespace SearchFile.Wpf.Module.Services
{
    public interface IChooseFolderService
    {
        string? ShowDialog(string? initialDirectory);
    }
}
