using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Rotate
{
    public static class Rotator
    {
        public static unsafe Bitmap Rotate(Bitmap originalBitmap, RotateType rotateType)
        {
            if (originalBitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ArgumentException("Only Format32bppArgb is supported.");

            var originalWidth = originalBitmap.Width;
            var originalHeight = originalBitmap.Height;

            var newWidth = originalHeight;
            var newHeight = originalWidth;

            var rotatedBitmap = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);

            var originalData = originalBitmap
                .LockBits(new Rectangle(0, 0, originalWidth, originalHeight),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var rotatedData = rotatedBitmap
                .LockBits(new Rectangle(0, 0, newWidth, newHeight),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            var originalPointer = (int*)originalData.Scan0.ToPointer();
            var ratatedPointer = (int*)rotatedData.Scan0.ToPointer();

            try
            {
                switch (rotateType)
                {
                    case RotateType.Rotate90:

                        Rotate90(originalWidth, originalHeight,
                            originalPointer, ratatedPointer);

                        break;

                    case RotateType.Rotate270:

                        Rotate270(originalWidth, originalHeight, 
                            originalPointer, ratatedPointer);

                        break;
                }
            }
            finally
            {
                originalBitmap.UnlockBits(originalData);
                rotatedBitmap.UnlockBits(rotatedData);
            }

            return rotatedBitmap;
        }

        private static unsafe void Rotate90(int originalWidth, int originalHeight,
            int* originalPointer, int* tranformedPointer)
        {
            var newWidth = originalHeight;
            var newWidthMinusOne = originalHeight - 1;

            Parallel.For(0, originalHeight, y =>
            {
                var destinationX = newWidthMinusOne - y;
                for (var x = 0; x < originalWidth; x++)
                {
                    var sourcePosition = x + y * originalWidth;
                    var destinationY = x;
                    var destinationPosition = destinationX + destinationY * newWidth;
                    tranformedPointer[destinationPosition] = originalPointer[sourcePosition];
                }
            });
        }

        private static unsafe void Rotate270(int originalWidth, int originalHeight,
            int* originalPointer, int* tranformedPointer)
        {
            var newWidth = originalHeight;

            var newHeightMinusOne = originalWidth - 1;

            Parallel.For(0, originalHeight, y =>
            {
                var destinationX = y;
                for (var x = 0; x < originalWidth; x++)
                {
                    var sourcePosition = x + y * originalWidth;
                    var destinationY = newHeightMinusOne - x;
                    var destinationPosition = destinationX + destinationY * newWidth;
                    tranformedPointer[destinationPosition] = originalPointer[sourcePosition];
                }
            });
        }
    }
}