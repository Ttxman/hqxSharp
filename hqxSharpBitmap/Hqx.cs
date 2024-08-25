using hqx;
using System.Drawing;
using System.Drawing.Imaging;

namespace hqxSharp.Bitmaps;

public static partial class HqxSharpBitmap
{
    public static Bitmap Scale(Bitmap bitmap, byte factor, HqxSharpParameters parameters)
    {
        if ((factor >= 2) && (factor <= 4))
        {
            return ScaleCore(bitmap, factor, parameters);
        }
        else
        {
            throw new ArgumentOutOfRangeException("factor", factor, "Hqx provides a scale factor of 2x, 3x, or 4x");
        }
    }

    private static Bitmap ScaleCore(Bitmap bitmap, byte factor, HqxSharpParameters parameters)
    {
        return Scale(bitmap, factor, HqxSharp.scalers[factor - 2], parameters);
    }

    private static unsafe Bitmap Scale(Bitmap bitmap, byte factor, Magnify magnify, HqxSharpParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        ArgumentNullException.ThrowIfNull(magnify);


        Bitmap? dest = null;
        BitmapData? bmpData = null, destData = null;
        var Xres = bitmap.Width;
        var Yres = bitmap.Height;
        try
        {
            dest = new Bitmap(checked((short)(Xres * factor)), checked((short)(Yres * factor)));
            bmpData = LockBits(bitmap, ImageLockMode.ReadOnly);
            destData = LockBits(dest, ImageLockMode.WriteOnly);
            magnify(StartPointer(bmpData), StartPointer(destData), Xres, Yres,
             parameters.LumaThreshold, parameters.BlueishThreshold, parameters.ReddishThreshold, parameters.AlphaThreshold,
             parameters.WrapX, parameters.WrapY);
        }
        finally
        {
            if (bmpData != null)
            {
                bitmap.UnlockBits(bmpData);
            }
            if ((destData != null) && (dest != null))
            {
                dest.UnlockBits(destData);
            }
        }
        return dest;
    }

    private static unsafe uint* StartPointer(BitmapData bmpData)
    {
        return (uint*)bmpData.Scan0.ToPointer();
    }

    private static BitmapData LockBits(Bitmap source, ImageLockMode lockMode)
    {
        return source.LockBits(new Rectangle(Point.Empty, source.Size), lockMode, PixelFormat.Format32bppArgb);
    }
}
