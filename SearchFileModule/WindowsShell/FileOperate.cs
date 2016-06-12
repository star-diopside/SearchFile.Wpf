using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace SearchFile.WindowsShell
{
    /// <summary>
    /// ファイル操作を行うシェル API を呼び出す
    /// </summary>
    static class FileOperate
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public SHFileOperationFunc wFunc;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pFrom;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pTo;
            public SHFileOperationFlags fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszProgressTitle;    // only used if FOF_SIMPLEPROGRESS
        }

        [Flags]
        private enum SHFileOperationFunc : uint
        {
            FO_MOVE = 0x0001,
            FO_COPY = 0x0002,
            FO_DELETE = 0x0003,
            FO_RENAME = 0x0004
        }

        [Flags]
        private enum SHFileOperationFlags : ushort
        {
            FOF_MULTIDESTFILES = 0x0001,
            FOF_CONFIRMMOUSE = 0x0002,
            FOF_SILENT = 0x0004,
            FOF_RENAMEONCOLLISION = 0x0008,
            FOF_NOCONFIRMATION = 0x0010,
            FOF_WANTMAPPINGHANDLE = 0x0020,
            FOF_ALLOWUNDO = 0x0040,
            FOF_FILESONLY = 0x0080,
            FOF_SIMPLEPROGRESS = 0x0100,
            FOF_NOCONFIRMMKDIR = 0x0200,
            FOF_NOERRORUI = 0x0400,

            // _WIN32_IE >= 0x0500
            FOF_NOCOPYSECURITYATTRIBS = 0x0800,
            FOF_NORECURSION = 0x1000,
            FOF_NO_CONNECTED_ELEMENTS = 0x2000,
            FOF_WANTNUKEWARNING = 0x4000,

            // _WIN32_WINNT >= 0x0501
            FOF_NORECURSEREPARSE = 0x8000,

            FOF_NO_UI = (FOF_SILENT | FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_NOCONFIRMMKDIR)  // don't display any UI at all
        }

        /// <summary>
        /// 複数のファイルを削除する
        /// </summary>
        /// <param name="files">削除するファイルのリスト</param>
        /// <param name="recycle">ファイルをごみ箱に移動する場合はtrue、完全に削除する場合はfalse</param>
        public static void DeleteFiles(IEnumerable<string> files, bool recycle)
        {
            DeleteFiles(null, files, recycle);
        }

        /// <summary>
        /// 複数のファイルを削除する
        /// </summary>
        /// <param name="owner">ダイアログボックスを所有するウィンドウ</param>
        /// <param name="files">削除するファイルのリスト</param>
        /// <param name="recycle">ファイルをごみ箱に移動する場合はtrue、完全に削除する場合はfalse</param>
        public static void DeleteFiles(Window owner, IEnumerable<string> files, bool recycle)
        {
            var sb = new StringBuilder();

            // 削除対処ファイルを指定する
            foreach (var file in files)
            {
                sb.Append(Path.GetFullPath(file)).Append('\0');
            }

            // 削除対象ファイルが指定されている場合
            if (sb.Length > 0)
            {
                sb.Append('\0');

                // ファイルを削除するシェル API を呼び出す
                var sh = new SHFILEOPSTRUCT();

                sh.hwnd = (owner == null ? IntPtr.Zero : new WindowInteropHelper(owner).Handle);
                sh.wFunc = SHFileOperationFunc.FO_DELETE;
                sh.pFrom = sb.ToString();
                sh.fFlags = 0;
                if (recycle)
                {
                    sh.fFlags |= SHFileOperationFlags.FOF_ALLOWUNDO;
                }

                if (SHFileOperation(ref sh) != 0)
                {
                    throw new Win32Exception();
                }

                // 処理がキャンセルされた場合
                if (sh.fAnyOperationsAborted)
                {
                    throw new OperationCanceledException();
                }
            }
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
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;

            // Optional fields
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
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
        public static void ShowPropertyDialog(Window owner, string fileName)
        {
            var info = new SHELLEXECUTEINFO();

            info.cbSize = (uint)Marshal.SizeOf(info);
            info.fMask = ShellExecuteMaskFlag.SEE_MASK_INVOKEIDLIST | ShellExecuteMaskFlag.SEE_MASK_FLAG_NO_UI;
            info.hwnd = (owner == null ? IntPtr.Zero : new WindowInteropHelper(owner).Handle);
            info.lpFile = fileName;
            info.lpVerb = "properties";
            info.lpParameters = null;
            info.lpDirectory = null;
            info.nShow = 0;
            info.lpIDList = IntPtr.Zero;

            if (!ShellExecuteEx(ref info))
            {
                // エラーコードに応じた例外をスローする
                switch ((ShellExecuteErrors)Marshal.GetLastWin32Error())
                {
                    case ShellExecuteErrors.SE_ERR_FNF:
                        throw new FileNotFoundException();
                    case ShellExecuteErrors.SE_ERR_PNF:
                        throw new DirectoryNotFoundException();
                    case ShellExecuteErrors.SE_ERR_ACCESSDENIED:
                        throw new UnauthorizedAccessException();
                    case ShellExecuteErrors.SE_ERR_OOM:
                        throw new OutOfMemoryException();
                    case ShellExecuteErrors.SE_ERR_DLLNOTFOUND:
                        throw new DllNotFoundException();
                    default:
                        throw new Win32Exception();
                }
            }
        }
    }
}
