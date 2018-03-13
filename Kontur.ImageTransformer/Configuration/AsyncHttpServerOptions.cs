using System;

namespace Kontur.ImageTransformer.Configuration
{
    public struct AsyncHttpServerOptions
    {
        public AsyncHttpServerOptions(
            int requestTimeout,
            int averageRequestProcessingTime,
            float serverLoadInPercent)
        {
            RequestTimeout = requestTimeout;
            AverageRequestProcessingTime = averageRequestProcessingTime;

            if (serverLoadInPercent > 100 || serverLoadInPercent < 0)
                throw new ArgumentException(nameof(serverLoadInPercent));

            ServerLoadInPercent = serverLoadInPercent;
        }

        public int RequestTimeout { get; }
        public int AverageRequestProcessingTime { get; }
        public float ServerLoadInPercent { get; }
    }
}