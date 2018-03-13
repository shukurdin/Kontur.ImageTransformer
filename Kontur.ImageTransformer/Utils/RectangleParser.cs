using System.Drawing;

namespace Kontur.ImageTransformer.Utils
{
    public static class RectangleParser
    {
        public static Rectangle Parse(string str)
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