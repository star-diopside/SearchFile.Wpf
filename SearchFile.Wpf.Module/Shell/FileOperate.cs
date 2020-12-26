using SearchFile.Wpf.Module.Interop;
using SearchFile.Wpf.Module.Interop.ShellObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SearchFile.Wpf.Module.Shell
{
    /// <summary>
    /// ファイル操作を行うシェル API を呼び出す
    /// </summary>
    static class FileOperate
    {
        /// <summary>
        /// 複数のファイルを削除する
        /// </summary>
        /// <param name="files">削除するファイルのリスト</param>
        /// <param name="recycle">ファイルをごみ箱に移動する場合はtrue、完全に削除する場合はfalse</param>
        /// <returns>処理がすべて完了した場合はtrue、処理がキャンセルされた場合はfalse</returns>
        public static bool DeleteFiles(IEnumerable<string> files, bool recycle)
        {
            return DeleteFiles(null, files, recycle);
        }

        /// <summary>
        /// 複数のファイルを削除する
        /// </summary>
        /// <param name="owner">ダイアログボックスを所有するウィンドウ</param>
        /// <param name="files">削除するファイルのリスト</param>
        /// <param name="recycle">ファイルをごみ箱に移動する場合はtrue、完全に削除する場合はfalse</param>
        /// <returns>処理がすべて完了した場合はtrue、処理がキャンセルされた場合はfalse</returns>
        public static bool DeleteFiles(Window? owner, IEnumerable<string> files, bool recycle)
        {
            var fo = (IFileOperation)new FileOperation();

            fo.SetOwnerWindow(owner is null ? IntPtr.Zero : new WindowInteropHelper(owner).Handle);
            fo.SetOperationFlags(recycle ? FileOperationFlags.FOF_ALLOWUNDO : 0);

            foreach (var file in files)
            {
                fo.DeleteItem(InteropHelpers.CreateShellItem(file), null);
            }

            fo.PerformOperations();

            return !fo.GetAnyOperationsAborted();
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHELLEXECUTEINFO
        {
            public uint cbSize;
            public ShellExecuteMaskFlag fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string? lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string? lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string? lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string? lpDirectory;
            public int nShow;
            public IntPtr hInstApp;

            // Optional fields
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string? lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        [Flags]
        private enum ShellExecuteMaskFlag : uint
        {
            SEE_MASK_CLASSNAME = 0x00000001,
            SEE_MASK_CLASSKEY = 0x00000003,
            SEE_MASK_IDLIST = 0x00000004,
            SEE_MASK_INVOKEIDLIST = 0x0000000c,
            SEE_MASK_ICON = 0x00000010,
            SEE_MASK_HOTKEY = 0x00000020,
            SEE_MASK_NOCLOSEPROCESS = 0x00000040,
            SEE_MASK_CONNECTNETDRV = 0x00000080,
            SEE_MASK_NOASYNC = 0x00000100,
            SEE_MASK_FLAG_DDEWAIT = SEE_MASK_NOASYNC,
            SEE_MASK_DOENVSUBST = 0x00000200,
            SEE_MASK_FLAG_NO_UI = 0x00000400,
            SEE_MASK_UNICODE = 0x00004000,
            SEE_MASK_NO_CONSOLE = 0x00008000,
            SEE_MASK_ASYNCOK = 0x00100000,
            SEE_MASK_HMONITOR = 0x00200000,
            SEE_MASK_NOZONECHECKS = 0x00800000,
            SEE_MASK_NOQUERYCLASSSTORE = 0x01000000,
            SEE_MASK_WAITFORINPUTIDLE = 0x02000000,
            SEE_MASK_FLAG_LOG_USAGE = 0x04000000
        }

        private enum ShellExecuteErrors : int
        {
            /// <summary>
            /// ファイルが見つかりません。
            /// </summary>
            SE_ERR_FNF = 2,
            /// <summary>
            /// パスが見つかりません。
            /// </summary>
            SE_ERR_PNF = 3,
            /// <summary>
            /// ファイルアクセスが拒否されました。
            /// </summary>
            SE_ERR_ACCESSDENIED = 5,
            /// <summary>
            /// メモリ不足です。
            /// </summary>
            SE_ERR_OOM = 8,
            /// <summary>
            /// DLL が見つかりません。
            /// </summary>
            SE_ERR_DLLNOTFOUND = 32,
            /// <summary>
            /// 共有違反が発生しました。
            /// </summary>
            SE_ERR_SHARE = 26,
            /// <summary>
            /// ファイル関連付けが完全ではないか無効です。
            /// </summary>
            SE_ERR_ASSOCINCOMPLETE = 27,
            /// <summary>
            /// DDE トランザクションがタイムアウトにより中断されました。
            /// </summary>
            SE_ERR_DDETIMEOUT = 28,
            /// <summary>
            /// DDE トランザクションが失敗しました。
            /// </summary>
            SE_ERR_DDEFAIL = 29,
            /// <summary>
            /// 他の DDE トランザクションが処理されていたため DDE トランザクションが終了できませんでした。
            /// </summary>
            SE_ERR_DDEBUSY = 30,
            /// <summary>
            /// ファイル関連付けが不明です。
            /// </summary>
            SE_ERR_NOASSOC = 31
        }

        /// <summary>
        /// プロパティダイアログを表示する
        /// </summary>
        /// <param name="fileName">プロパティを表示するファイル名</param>
        public static void ShowPropertyDialog(string fileName)
        {
            ShowPropertyDialog(null, fileName);
        }

        /// <summary>
        /// プロパティダイアログを表示する
        /// </summary>
        /// <param name="owner">ダイアログボックスを所有するウィンドウ</param>
        /// <param name="fileName">プロパティを表示するファイル名</param>
        public static void ShowPropertyDialog(Window? owner, string fileName)
        {
            var info = new SHELLEXECUTEINFO();

            info.cbSize = (uint)Marshal.SizeOf(info);
            info.fMask = ShellExecuteMaskFlag.SEE_MASK_INVOKEIDLIST | ShellExecuteMaskFlag.SEE_MASK_FLAG_NO_UI;
            info.hwnd = (owner is null ? IntPtr.Zero : new WindowInteropHelper(owner).Handle);
            info.lpFile = fileName;
            info.lpVerb = "properties";
            info.lpParameters = null;
            info.lpDirectory = null;
            info.nShow = 0;
            info.lpIDList = IntPtr.Zero;

            if (!ShellExecuteEx(ref info))
            {
                // エラーコードに応じた例外をスローする
                throw (ShellExecuteErrors)Marshal.GetLastWin32Error() switch
                {
                    ShellExecuteErrors.SE_ERR_FNF => new FileNotFoundException(),
                    ShellExecuteErrors.SE_ERR_PNF => new DirectoryNotFoundException(),
                    ShellExecuteErrors.SE_ERR_ACCESSDENIED => new UnauthorizedAccessException(),
                    ShellExecuteErrors.SE_ERR_OOM => new OutOfMemoryException(),
                    ShellExecuteErrors.SE_ERR_DLLNOTFOUND => new DllNotFoundException(),
                    _ => new Win32Exception(),
                };
            }
        }
    }
}
