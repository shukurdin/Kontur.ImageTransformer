namespace Kontur.ImageTransformer.Filters
{
    public static class Threshold
    {
        public static unsafe int GetColor(int argb, uint x)
        {
            var pointer = (byte*) &argb;

            var intensity = (pointer[2] + pointer[1] + pointer[0]) / 3;
            intensity = intensity >= 255 * x / 100 ? 255 : 0;

            pointer[2] = pointer[1] = pointer[0] = (byte) intensity;

            return argb;
        }
    }
}