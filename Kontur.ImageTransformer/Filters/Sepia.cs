namespace Kontur.ImageTransformer.Filters
{
    public static class Sepia
    {
        public static unsafe int GetColor(int argb)
        {
            var pointer = (byte*) &argb;

            // B component of the pixel is stored in byte 0
            // G component of the pixel is stored in byte 1
            // R component of the pixel is stored in byte 2
            // A component of the pixel is stored in byte 3

            var red = pointer[2] * .393f + pointer[1] * .769f + pointer[0] * .189f;
            var green = pointer[2] * .349f + pointer[1] * .686f + pointer[0] * .168f;
            var blue = pointer[2] * .272f + pointer[1] * .534f + pointer[0] * .131f;

            if (red > 255) red = 255;
            if (green > 255) green = 255;
            if (blue > 255) blue = 255;

            pointer[2] = (byte) red;
            pointer[1] = (byte) green;
            pointer[0] = (byte) blue;

            return argb;
        }
    }
}