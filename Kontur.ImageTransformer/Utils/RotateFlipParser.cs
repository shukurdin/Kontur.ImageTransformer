using System;
using System.Drawing;

namespace Kontur.ImageTransformer.Utils
{
    public static class RotateFlipParser
    {
        public static RotateFlipType Parse(string transform)
        {
            if (transform.Equals("rotate-cw", StringComparison.OrdinalIgnoreCase))
                return RotateFlipType.Rotate90FlipNone;

            if (transform.Equals("rotate-ccw", StringComparison.OrdinalIgnoreCase))
                return RotateFlipType.Rotate270FlipNone;

            if (transform.Equals("flip-h", StringComparison.OrdinalIgnoreCase))
                return RotateFlipType.RotateNoneFlipX;

            if (transform.Equals("flip-v", StringComparison.OrdinalIgnoreCase))
                return RotateFlipType.RotateNoneFlipY;

            return RotateFlipType.RotateNoneFlipNone;
        }
    }
}