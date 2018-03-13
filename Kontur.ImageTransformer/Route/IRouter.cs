using System;
using System.Threading;
using Kontur.ImageTransformer.Requests;
using Kontur.ImageTransformer.Results;

namespace Kontur.ImageTransformer.Route
{
    public interface IRouter
    {
        Func<RouteData, CancellationToken, BitmapResult> Handler { get; }
        RouteData GetRouteData(Request request);
        bool IsMatch(Request request);
    }
}