using System;
using System.Collections.Generic;
using System.Drawing;
using Kontur.ImageTransformer.Configuration;
using Kontur.ImageTransformer.Controllers;
using Kontur.ImageTransformer.Rotate;
using Kontur.ImageTransformer.Route;
using Kontur.ImageTransformer.Utils;

namespace Kontur.ImageTransformer
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            var requestRestriction = new RequestRestriction(100 * 1024, new Size(1000, 1000));

            var filterController = new FilterController(requestRestriction,
                FilterParser.GetFilter);
            var rotateFlipController = new RotateFlipController(requestRestriction,
                Rotator.Rotate, RotateFlipParser.Parse);

            var filterPattern = @"(grayscale|threshold\((100|0?[0-9]{1,2}|0{0,2}[0-9])\)|sepia)";
            var rotateFlipPattern = @"(rotate-(cw|ccw)|flip-(h|v))";

            var routers = new List<IRouter>
            {
                new ImageTransformRouter(filterController.ProcessImage, filterPattern),
                new ImageTransformRouter(rotateFlipController.ProcessImage, rotateFlipPattern)
            };
            
            using (var server = new AsyncHttpServer(new AsyncHttpServerOptions(1000, 15, 80), 
                routers))
            {
                server.Start("http://+:8080/");

                Console.ReadKey(true);
            }
        }
    }
}