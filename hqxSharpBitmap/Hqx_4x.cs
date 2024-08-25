using hqx;
using System.Drawing;
using System.Drawing.Imaging;

namespace hqxSharp.Bitmaps;

public static partial class HqxSharpBitmap
{
    /// <summary>
    /// This is the extended C# port of the hq4x algorithm.
    /// <para>The image is scaled to four times its size.</para>
    /// </summary>
    /// <param name="bitmap">The source image.</param>
    /// <param name="parameters">Thresholds and wrapping options for scaling.</param>
    /// <returns>A new Bitmap instance that contains the source image scaled to four times its size.</returns>
    public static unsafe Bitmap Scale4(Bitmap bitmap, HqxSharpParameters parameters)
    {
        return Scale(bitmap, 4, HqxSharp.Scale4, parameters);
    }

    public static unsafe Bitmap Scale4(Bitmap bitmap)
    {
        return Scale(bitmap, 4, HqxSharp.Scale4, HqxSharpParameters.Default);
    }
}
