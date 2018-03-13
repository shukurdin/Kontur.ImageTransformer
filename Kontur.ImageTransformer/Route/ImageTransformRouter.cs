using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Kontur.ImageTransformer.Requests;
using Kontur.ImageTransformer.Results;

namespace Kontur.ImageTransformer.Route
{
    public class ImageTransformRouter : IRouter
    {
        private static readonly string routePrefix = @"/process/";
        private static readonly string coorsPattern = @"/(\-?[0-9]{1,11}(\,|/?$)){4}";

        private readonly string httpMethod;
        private readonly Regex regex;

        public ImageTransformRouter(Func<RouteData, CancellationToken, BitmapResult> action,
            string transformPattern, string httpMethod = "POST")
        {
            Handler = action ?? throw new ArgumentException();

            var stringBuilder = new StringBuilder(routePrefix);
            stringBuilder.Append(transformPattern);
            stringBuilder.Append(coorsPattern);
            regex = new Regex(stringBuilder.ToString(), RegexOptions.Compiled);

            this.httpMethod = httpMethod;
        }

        public Func<RouteData, CancellationToken, BitmapResult> Handler { get; }

        public RouteData GetRouteData(Request rquest)
        {
            if (!IsMatch(rquest)) throw new InvalidOperationException();

            var urlSegments = rquest.RawUrl.Split('/');

            return new RouteData(rquest, urlSegments[2], GetRectangle(urlSegments[3]));
        }

        public bool IsMatch(Request request)
        {
            return request.HttpMethod.Equals(httpMethod, StringComparison.OrdinalIgnoreCase) &&
                   regex.IsMatch(request.RawUrl);
        }

        private static Rectangle GetRectangle(string str)
        {
            var coors = str.Split(',');

            if (!int.TryParse(coors[0], out var x) ||
                !int.TryParse(coors[1], out var y) ||
                !int.TryParse(coors[2], out var width) ||
                !int.TryParse(coors[3], out var height)) return Rectangle.Empty;

            if (width < 0)
            {
                x += width;
                width *= -1;
            }

            if (height < 0)
            {
                y += height;
                height *= -1;
            }

            return new Rectangle(x, y, width, height);
        }
    }
}