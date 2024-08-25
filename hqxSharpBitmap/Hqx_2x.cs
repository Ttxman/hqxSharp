using hqx;
using System.Drawing;
using System.Drawing.Imaging;

namespace hqxSharp.Bitmaps;

public static partial class HqxSharpBitmap
{
    /// <summary>
    /// This is the extended C# port of the hq2x algorithm.
    /// <para>The image is scaled to twice its size.</para>
    /// </summary>
    /// <param name="bitmap">The source image.</param>
    /// <param name="parameters">Thresholds and wrapping options for scaling.</param>
    /// <returns>A new Bitmap instance that contains the source image scaled to twice its size.</returns>
    public static unsafe Bitmap Scale2(Bitmap bitmap, HqxSharpParameters parameters)
    {
        return Scale(bitmap, 2, HqxSharp.Scale2, parameters);
    }

    public static unsafe Bitmap Scale2(Bitmap bitmap)
    {
        return Scale(bitmap, 2, HqxSharp.Scale2, HqxSharpParameters.Default);
    }
}
