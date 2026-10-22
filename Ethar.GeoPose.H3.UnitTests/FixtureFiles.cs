using System.Globalization;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// Reads the Uber H3 fixture files copied under Fixtures/H3. See the readme in that folder for the formats.
    /// </summary>
    internal static class FixtureFiles
    {
        private static readonly Regex IndexToken = new Regex("^[0-9a-f]{15}$", RegexOptions.Compiled);

        private static readonly char[] Separators = { ' ', '\t' };

        public static string Directory => Path.Combine(TestContext.CurrentContext.TestDirectory, "Fixtures", "H3");

        /// <summary>
        /// Gets every fixture file name, sorted, for use as a test case source.
        /// </summary>
        public static IEnumerable<string> Names => System.IO.Directory.EnumerateFiles(Directory, "*.txt").Select(Path.GetFileName).OrderBy(name => name, StringComparer.Ordinal);

        /// <summary>
        /// Gets the fixture files whose lines are an index followed by its centre in degrees.
        /// </summary>
        public static IEnumerable<string> CenterFileNames => Names.Where(name => name.Contains("centers") || name.Contains("ic.txt"));

        /// <summary>
        /// Gets the fixture files that list every cell at a resolution with its boundary.
        /// </summary>
        public static IEnumerable<string> BoundaryFileNames => Names.Where(name => name.Contains("cells"));

        /// <summary>
        /// Returns the index token that starts each cell line, in file order. Boundary and brace lines are skipped.
        /// </summary>
        public static List<string> ReadIndexes(string name)
        {
            var indexes = new List<string>();
            foreach (var line in File.ReadLines(Path.Combine(Directory, name)))
            {
                var trimmed = line.Trim();
                if (trimmed.Length == 0)
                {
                    continue;
                }

                var end = trimmed.IndexOf(' ');
                var token = end < 0 ? trimmed : trimmed.Substring(0, end);
                if (IndexToken.IsMatch(token))
                {
                    indexes.Add(token);
                }
            }

            return indexes;
        }

        /// <summary>
        /// Returns each index with its centre in degrees from a centers or ic file.
        /// </summary>
        public static List<CenterLine> ReadCenters(string name)
        {
            var centers = new List<CenterLine>();
            foreach (var line in File.ReadLines(Path.Combine(Directory, name)))
            {
                var parts = line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3 || !IndexToken.IsMatch(parts[0]))
                {
                    continue;
                }

                centers.Add(new CenterLine(parts[0], ParseDegrees(parts[1]), ParseDegrees(parts[2])));
            }

            return centers;
        }

        /// <summary>
        /// Returns each index with its boundary vertices in degrees from a cells file.
        /// </summary>
        public static List<BoundaryBlock> ReadBoundaries(string name)
        {
            var blocks = new List<BoundaryBlock>();
            BoundaryBlock current = null;
            foreach (var line in File.ReadLines(Path.Combine(Directory, name)))
            {
                var trimmed = line.Trim();
                if (trimmed.Length == 0)
                {
                    continue;
                }

                if (IndexToken.IsMatch(trimmed))
                {
                    current = new BoundaryBlock(trimmed);
                    blocks.Add(current);
                    continue;
                }

                if (trimmed == "{" || trimmed == "}")
                {
                    continue;
                }

                var parts = trimmed.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 && current != null)
                {
                    current.Vertices.Add(new DegreesPoint(ParseDegrees(parts[0]), ParseDegrees(parts[1])));
                }
            }

            return blocks;
        }

        /// <summary>
        /// Difference between two longitudes in degrees, ignoring full turns, so 180 and -180 compare equal.
        /// </summary>
        public static double LongitudeDifference(double a, double b)
        {
            var diff = Math.Abs(a - b) % 360.0;
            return Math.Min(diff, 360.0 - diff);
        }

        private static double ParseDegrees(string text)
        {
            return double.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        public sealed class CenterLine
        {
            public CenterLine(string hex, double latitude, double longitude)
            {
                this.Hex = hex;
                this.Latitude = latitude;
                this.Longitude = longitude;
            }

            public string Hex { get; }

            public double Latitude { get; }

            public double Longitude { get; }
        }

        public sealed class DegreesPoint
        {
            public DegreesPoint(double latitude, double longitude)
            {
                this.Latitude = latitude;
                this.Longitude = longitude;
            }

            public double Latitude { get; }

            public double Longitude { get; }
        }

        public sealed class BoundaryBlock
        {
            public BoundaryBlock(string hex)
            {
                this.Hex = hex;
            }

            public string Hex { get; }

            public List<DegreesPoint> Vertices { get; } = new List<DegreesPoint>();
        }
    }
}
