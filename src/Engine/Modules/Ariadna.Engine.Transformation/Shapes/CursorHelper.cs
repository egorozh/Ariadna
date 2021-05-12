using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32.SafeHandles;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Ariadna.Engine.Transformation
{
    internal static class CursorHelper
    {
        private struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        public static Cursor ConvertToCursor(FrameworkElement cursor, Point HotSpot)
        {
            cursor.Arrange(new Rect(new Size(cursor.Width, cursor.Height)));
            var bitmap = new RenderTargetBitmap((int) cursor.Width, (int) cursor.Height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(cursor);

            var info = new IconInfo();
            GetIconInfo(bitmap.ToBitmap().GetHicon(), ref info);
            info.fIcon = false;
            info.xHotspot = (byte) (HotSpot.X * cursor.Width);
            info.yHotspot = (byte) (HotSpot.Y * cursor.Height);

            return CursorInteropHelper.Create(new SafeFileHandle(CreateIconIndirect(ref info), true));
        }

        public static Bitmap ToBitmap(this BitmapSource bitmapSource)
        {
            var bitmap = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            var data = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bitmap.UnlockBits(data);
            return bitmap;
        }
    }
}