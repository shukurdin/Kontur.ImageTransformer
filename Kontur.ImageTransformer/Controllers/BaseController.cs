using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using Kontur.ImageTransformer.Configuration;
using Kontur.ImageTransformer.Results;
using Kontur.ImageTransformer.Route;

namespace Kontur.ImageTransformer.Controllers
{
    public abstract class BaseController
    {
        private readonly RequestRestriction requestRestriction;

        protected BaseController(RequestRestriction requestRestriction)
        {
            this.requestRestriction = requestRestriction;
        }

        public BitmapResult ProcessImage(RouteData routeData, CancellationToken cancellationToken)
        {
            var request = routeData.Request;

            if (!IsValidRequest(routeData)) return new BitmapResult(HttpStatusCode.BadRequest);

            var image = СreateBitmap(request.InputStream);
            if (image == null || !IsValidImage(image))
            {
                image?.Dispose();
                return new BitmapResult(HttpStatusCode.BadRequest);
            }

            if (cancellationToken.IsCancellationRequested)
                return new BitmapResult(HttpStatusCode.RequestTimeout);

            image = Transform(image, routeData);

            if (image == null) return new BitmapResult(HttpStatusCode.NoContent);

            return new BitmapResult(HttpStatusCode.OK, image);
        }

        protected abstract Bitmap Transform(Bitmap bitmap, RouteData routeData);

        protected static Bitmap Crop(Bitmap image, Rectangle rectangle)
        {
            var bitmapRegtangle = new Rectangle(0, 0, image.Width, image.Height);
            var intersect = Rectangle.Intersect(bitmapRegtangle, rectangle);

            if (intersect.Width == 0 || intersect.Height == 0) return null;
            if (intersect == bitmapRegtangle) return image;

            var clone = image.Clone(intersect, image.PixelFormat);
            image.Dispose();
            image = clone;

            return image;
        }

        private Bitmap СreateBitmap(Stream body)
        {
            try
            {
                return new Bitmap(body);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private bool IsValidRequest(RouteData routeData)
        {
            var request = routeData.Request;

            return request.ContentLength64 > 0 &&
                   request.ContentLength64 <= requestRestriction.ContentLength &&
                   request.InputStream != null;
        }

        private bool IsValidImage(Bitmap image)
        {
            return image.Size.Width <= requestRestriction.ImageSize.Width &&
                   image.Size.Height <= requestRestriction.ImageSize.Height &&
                   image.PixelFormat == PixelFormat.Format32bppArgb;
        }
    }
}