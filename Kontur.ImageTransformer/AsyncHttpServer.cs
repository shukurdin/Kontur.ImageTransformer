using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Configuration;
using Kontur.ImageTransformer.Extensions;
using Kontur.ImageTransformer.Requests;
using Kontur.ImageTransformer.Results;
using Kontur.ImageTransformer.Route;
using NLog;

namespace Kontur.ImageTransformer
{
    internal class AsyncHttpServer : IDisposable
    {
        private readonly HttpListener listener;
        private readonly Logger logger;
        private readonly int maxNumberTaskPending;
        private readonly List<IRouter> routers;
        private readonly int timeoutMinusAverageTime;

        private volatile uint currentRequestCount;
        private bool disposed;
        private volatile bool isRunning;
        private Thread listenerThread;

        public AsyncHttpServer(AsyncHttpServerOptions options,
            List<IRouter> routers)
        {
            listener = new HttpListener();
            this.routers = routers;

            maxNumberTaskPending = GetMaxNumberTask(options);
            timeoutMinusAverageTime = options.RequestTimeout -
                                      options.AverageRequestProcessingTime;

            if (timeoutMinusAverageTime < 0) throw new ArgumentException(nameof(options));

            logger = LogManager.GetCurrentClassLogger();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            Stop();

            listener.Close();
        }

        private static int GetMaxNumberTask(AsyncHttpServerOptions options)
        {
            var rpsForProcessor = 1000 / options.AverageRequestProcessingTime;
            var requestProcessingProcessorCount = Environment.ProcessorCount - 1;
            return (int) (rpsForProcessor * requestProcessingProcessorCount
                          * options.ServerLoadInPercent);
        }

        public void Start(string prefix)
        {
            lock (listener)
            {
                if (!isRunning)
                {
                    listener.Prefixes.Clear();
                    listener.Prefixes.Add(prefix);
                    listener.Start();

                    listenerThread = new Thread(Listen)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest
                    };
                    listenerThread.Start();

                    isRunning = true;
                }
            }
        }

        public void Stop()
        {
            lock (listener)
            {
                if (!isRunning)
                    return;

                listener.Stop();

                listenerThread.Abort();
                listenerThread.Join();

                isRunning = false;
            }
        }

        private void Listen()
        {
            while (true)
                try
                {
                    if (listener.IsListening)
                    {
                        var context = listener.GetContext();
                        if (currentRequestCount <= maxNumberTaskPending)
                        {
                            currentRequestCount++;

                            var cts = new CancellationTokenSource(timeoutMinusAverageTime);
                            Task.Run(() => HandleContextAsync(context, cts.Token),
                                    cts.Token)
                                .ContinueWith(t => Canceled(context),
                                    TaskContinuationOptions.OnlyOnCanceled);
                        }
                        else
                        {
                            context.SendResponse(429);
                        }
                    }
                    else
                    {
                        Thread.Sleep(0);
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception error)
                {
                    logger.Error(error);
                }
        }

        private void HandleContextAsync(HttpListenerContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                currentRequestCount--;
                var result = Handle(context.Request, cancellationToken);
                context.SendResponse(result);
            }
            catch (Exception error)
            {
                logger.Error(error);
            }
            finally
            {
                context.CloseStreams();
            }
        }

        private BitmapResult Handle(HttpListenerRequest request,
            CancellationToken cancellationToken)
        {
            var router = routers.FirstOrDefault(r => r.IsMatch(new Request(request)));

            if (router == null) return new BitmapResult(HttpStatusCode.BadRequest);

            var raouteData = router.GetRouteData(new Request(request));

            return router.Handler(raouteData, cancellationToken);
        }

        private void Canceled(HttpListenerContext context)
        {
            currentRequestCount--;
            context.SendResponse(HttpStatusCode.RequestTimeout);
            context.CloseStreams();
        }
    }
}