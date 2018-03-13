namespace Kontur.ImageTransformer.Filters
{
    public static class Grayscale
    {
        public static unsafe int GetColor(int argb)
        {
            var pointer = (byte*) &argb;

            var intensity = (byte) ((pointer[0] + pointer[1] + pointer[2]) / 3);

            pointer[2] = pointer[1] = pointer[0] = intensity;

            return argb;
        }
    }
}