using System;
using System.Drawing;
using Kontur.ImageTransformer.Configuration;
using Kontur.ImageTransformer.Rotate;
using Kontur.ImageTransformer.Route;

namespace Kontur.ImageTransformer.Controllers
{
    public class RotateFlipController : BaseController
    {
        private readonly Func<string, RotateFlipType> parse;
        private readonly Func<Bitmap, RotateType, Bitmap> rotate;

        public RotateFlipController(RequestRestriction requestRestriction,
            Func<Bitmap, RotateType, Bitmap> rotate,
            Func<string, RotateFlipType> parse) :
            base(requestRestriction)
        {
            this.rotate = rotate;
            this.parse = parse;
        }

        protected override Bitmap Transform(Bitmap bitmap, RouteData routeData)
        {
            var transformType = parse(routeData.Transform);
            var originalBitmap = bitmap;
            switch (transformType)
            {
                case RotateFlipType.Rotate90FlipNone:
                    bitmap = rotate(bitmap, RotateType.Rotate90);
                    break;
                case RotateFlipType.Rotate270FlipNone:
                    bitmap = rotate(bitmap, RotateType.Rotate270);
                    break;
                case RotateFlipType.RotateNoneFlipX:
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case RotateFlipType.RotateNoneFlipY:
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    break;
            }

            if (transformType == RotateFlipType.Rotate90FlipNone ||
                transformType == RotateFlipType.Rotate270FlipNone)
                originalBitmap.Dispose();

            return Crop(bitmap, routeData.Rectangle);
        }
    }
}