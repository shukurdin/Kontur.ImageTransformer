using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Extensions
{
    public static class BitmapExtensions
    {
        public static unsafe void ApplyFilter(this Bitmap bitmap, Func<int, int> getRgb)
        {
            var width = bitmap.Width;
            var height = bitmap.Height;
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ArgumentException(nameof(bitmap));

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            var pointer = (int*) bitmapData.Scan0.ToPointer();

            try
            {
                ApplyFilter(width, height, pointer, getRgb);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        private static unsafe void ApplyFilter(int width, int height,
            int* pointer, Func<int, int> getRgb)
        {
            Parallel.For(0, height, y =>
            {
                for (var x = 0; x < width; x++)
                {
                    var position = x + y * width;
                    pointer[position] = getRgb(pointer[position]);
                }
            });
        }
    }
}