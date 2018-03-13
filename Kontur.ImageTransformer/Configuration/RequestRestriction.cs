using System.Drawing;

namespace Kontur.ImageTransformer.Configuration
{
    public struct RequestRestriction
    {
        public RequestRestriction(long contentLength, Size imageSize)
        {
            ContentLength = contentLength;
            ImageSize = imageSize;
        }

        public long ContentLength { get; }
        public Size ImageSize { get; }
    }
}