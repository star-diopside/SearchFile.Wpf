using System;
using System.Runtime.InteropServices;

namespace SearchFile.Module.Interop.ShellObjects
{
    [ComImport]
    [Guid(ComIid.IFileOperation)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileOperation
    {
        uint Advise(IFileOperationProgressSink pfops);

        void Unadvise(uint dwCookie);

        void SetOperationFlags(FileOperationFlags dwOperationFlags);

        void SetProgressMessage([MarshalAs(UnmanagedType.LPWStr)] string pszMessage);

        void SetProgressDialog(
            [MarshalAs(UnmanagedType.Interface)] /* IOperationsProgressDialog */ object popd);

        void SetProperties(
            [MarshalAs(UnmanagedType.Interface)] /* IPropertyChangeArray */ object pproparray);

        void SetOwnerWindow(IntPtr hwndOwner);

        void ApplyPropertiesToItem(IShellItem psiItem);

        void ApplyPropertiesToItems(
            [MarshalAs(UnmanagedType.IUnknown)] object punkItems);

        void RenameItem(
            IShellItem psiItem,
            [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
            IFileOperationProgressSink pfopsItem);

        void RenameItems(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnkItems,
            [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);

        void MoveItem(
            IShellItem psiItem,
            IShellItem psiDestinationFolder,
            [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
            IFileOperationProgressSink pfopsItem);

        void MoveItems(
            [MarshalAs(UnmanagedType.IUnknown)] object punkItems,
            IShellItem psiDestinationFolder);

        void CopyItem(
            IShellItem psiItem,
            IShellItem psiDestinationFolder,
            [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName,
            IFileOperationProgressSink pfopsItem);

        void CopyItems(
            [MarshalAs(UnmanagedType.IUnknown)] object punkItems,
            IShellItem psiDestinationFolder);

        void DeleteItem(
            IShellItem psiItem,
            IFileOperationProgressSink pfopsItem);

        void DeleteItems(
            [MarshalAs(UnmanagedType.IUnknown)] object punkItems);

        void NewItem(
            IShellItem psiDestinationFolder,
            uint dwFileAttributes,
            [MarshalAs(UnmanagedType.LPWStr)] string pszName,
            [MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName,
            IFileOperationProgressSink pfopsItem);

        void PerformOperations();

        [return: MarshalAs(UnmanagedType.Bool)]
        bool GetAnyOperationsAborted();
    }
}
