using System.Drawing;
using Kontur.ImageTransformer.Requests;

namespace Kontur.ImageTransformer.Route
{
    public struct RouteData
    {
        public RouteData(Request request, string transform, Rectangle rectangle)
        {
            Transform = transform;
            Rectangle = rectangle;
            Request = request;
        }

        public Request Request { get; }
        public string Transform { get; }
        public Rectangle Rectangle { get; }
    }
}