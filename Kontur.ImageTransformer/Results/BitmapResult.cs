using System.Drawing;
using System.Net;

namespace Kontur.ImageTransformer.Results
{
    public struct BitmapResult
    {
        public BitmapResult(HttpStatusCode statusCode) : this()
        {
            StatusCode = (int) statusCode;
        }

        public BitmapResult(HttpStatusCode statusCode, Bitmap bitmap)
        {
            StatusCode = (int) statusCode;
            Bitmap = bitmap;
        }

        public int StatusCode { get; }
        public Bitmap Bitmap { get; }
    }
}