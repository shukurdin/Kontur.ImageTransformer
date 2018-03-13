using System.Drawing.Imaging;
using System.IO;
using System.Net;
using Kontur.ImageTransformer.Results;

namespace Kontur.ImageTransformer.Extensions
{
    public static class HttpListenerContextExtensions
    {
        public static void SendResponse(this HttpListenerContext context, BitmapResult result)
        {
            if (result.Bitmap != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    result.Bitmap.Save(memoryStream, ImageFormat.Png);
                    memoryStream.WriteTo(context.Response.OutputStream);
                }

                result.Bitmap.Dispose();
            }

            context.SendResponse(result.StatusCode);
        }

        public static void SendResponse(this HttpListenerContext context, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            SendResponse(context);
        }

        public static void SendResponse(this HttpListenerContext context, HttpStatusCode statusCode)
        {
            context.SendResponse((int) statusCode);
        }

        private static void SendResponse(HttpListenerContext context)
        {
            using (var writer = new StreamWriter(context.Response.OutputStream))
            {
                writer.WriteLine();
            }
        }

        public static void CloseStreams(this HttpListenerContext context)
        {
            context.Request.InputStream?.Close();
            context.Response.OutputStream?.Close();
        }
    }
}