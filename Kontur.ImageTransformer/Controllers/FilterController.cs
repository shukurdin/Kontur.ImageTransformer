using System;
using System.Drawing;
using Kontur.ImageTransformer.Configuration;
using Kontur.ImageTransformer.Extensions;
using Kontur.ImageTransformer.Route;

namespace Kontur.ImageTransformer.Controllers
{
    public class FilterController : BaseController
    {
        private readonly Func<string, Func<int, int>> filterParse;

        public FilterController(
            RequestRestriction requestRestriction,
            Func<string, Func<int, int>> filterParse) :
            base(requestRestriction)
        {
            this.filterParse = filterParse;
        }

        protected override Bitmap Transform(Bitmap bitmap, RouteData routeData)
        {
            bitmap = Crop(bitmap, routeData.Rectangle);

            if (bitmap != null)
            {
                var filter = filterParse(routeData.Transform);
                bitmap.ApplyFilter(filter);
            }

            return bitmap;
        }
    }
}