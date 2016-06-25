﻿using System;
using System.Runtime.InteropServices;

namespace SearchFile.Module.Interop.ShellObjects
{
    [ComImport]
    [Guid(ComIid.IFileOperationProgressSink)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileOperationProgressSink
    {
        void StartOperations();

        void FinishOperations(int hrResult);

        void PreRenameItem(
            uint dwFlags,
            IShellItem psiItem,
            [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);

        void PostRenameItem(
            uint dwFlags,
            IShellItem psiItem,
            [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
            int hrRename,
            IShellItem psiNewlyCreated);

        void PreMoveItem(
            uint dwFlags,
            IShellItem psiItem,
            IShellItem psiDestinationFolder,
            [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);

        void PostMoveItem(
            uint dwFlags,
            IShellItem psiItem,
            IShellItem psiDestinationFolder,
            [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
            int hrMove,
            IShellItem psiNewlyCreated);

        void PreCopyItem(
            uint dwFlags,
            IShellItem psiItem,
            IShellItem psiDestinationFolder,
            [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);

        void PostCopyItem(
            uint dwFlags,
            IShellItem psiItem,
            IShellItem psiDestinationFolder,
            [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
            int hrCopy,
            IShellItem psiNewlyCreated);

        void PreDeleteItem(
            uint dwFlags,
            IShellItem psiItem);

        void PostDeleteItem(
            uint dwFlags,
            IShellItem psiItem,
            int hrDelete,
            IShellItem psiNewlyCreated);

        void PreNewItem(
            uint dwFlags,
            IShellItem psiDestinationFolder,
            [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);

        void PostNewItem(
            uint dwFlags,
            IShellItem psiDestinationFolder,
            [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
            [MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName,
            uint dwFileAttributes,
            int hrNew,
            IShellItem psiNewItem);

        void UpdateProgress(
            uint iWorkTotal,
            uint iWorkSoFar);

        void ResetTimer();

        void PauseTimer();

        void ResumeTimer();
    }
}
