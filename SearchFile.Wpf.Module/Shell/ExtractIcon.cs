using SearchFile.Wpf.Module.Interop;
using SearchFile.Wpf.Module.Interop.CommonControlObjects;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SearchFile.Wpf.Module.Shell
{
    /// <summary>
    /// 実行ファイルや拡張子に関連付けられたアイコンに関する機能を提供する。
    /// </summary>
    public static class ExtractIcon
    {
        /// <summary>
        /// 取得するアイコンを指定する列挙体
        /// </summary>
        public enum IconSize
        {
            Large = SHIL.SHIL_LARGE,
            Small = SHIL.SHIL_SMALL,
            ExtraLarge = SHIL.SHIL_EXTRALARGE,
            SysSmall = SHIL.SHIL_SYSSMALL,
            Jumbo = SHIL.SHIL_JUMBO
        }

        /// <summary>
        /// ファイルに関連付けられたアイコンを取得する
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="size">アイコンサイズ</param>
        /// <returns>ファイルに関連付けられたアイコンイメージ</returns>
        public static ImageSource ExtractFileIcon(string fileName, IconSize size)
        {
            var fileInfo = new SHFILEINFO();
            InteropMethods.SHGetFileInfo(fileName, 0, ref fileInfo, (uint)Marshal.SizeOf(fileInfo),
                SHGetFileInfoFlags.SHGFI_SYSICONINDEX);

            var imageList = InteropHelpers.GetImageList((SHIL)size);

            IntPtr? icon = null;

            try
            {
                icon = imageList.GetIcon(fileInfo.iIcon, ImageListDrawFlags.ILD_TRANSPARENT);
                return Imaging.CreateBitmapSourceFromHIcon(icon.Value, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                if (icon.HasValue)
                {
                    InteropMethods.DestroyIcon(icon.Value);
                }
            }
        }
    }
}
