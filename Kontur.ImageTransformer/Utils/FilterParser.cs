using System;
using Kontur.ImageTransformer.Filters;

namespace Kontur.ImageTransformer.Utils
{
    public static class FilterParser
    {
        private static readonly string grayscaleFilterName = "grayscale";
        private static readonly string thresholdFilterName = "threshold";
        private static readonly string sepiaFilterName = "sepia";

        public static Func<int, int> GetFilter(string filterName)
        {
            if (filterName.StartsWith(grayscaleFilterName, StringComparison.OrdinalIgnoreCase))
                return Grayscale.GetColor;

            if (filterName.StartsWith(sepiaFilterName, StringComparison.OrdinalIgnoreCase))
                return Sepia.GetColor;

            if (!filterName.StartsWith(thresholdFilterName, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(filterName);

            var x = ParseThresholdFilterParameter(filterName);
            return rgb => Threshold.GetColor(rgb, x);
        }

        private static uint ParseThresholdFilterParameter(string filter)
        {
            var start = filter.IndexOf('(') + 1;
            var length = filter.IndexOf(')') - start - 1;

            var s = filter.Substring(start, length);

            return uint.Parse(s);
        }
    }
}