using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using static System.Environment;

namespace Aoc2023.ActiveDay
{
    using Coord = (int x, int y);


    internal class SolutionDay11
    {
        class ExpandedUniverseModel(HashSet<int> expandedX, HashSet<int> expandedY, long expansion)
        {
            public HashSet<int> ExpandedX { get; } = expandedX;
            public HashSet<int> ExpandedY { get; } = expandedY;
            public long Expansion { get; } = expansion;


            public long CalcDist(Coord galA, Coord galB)
            {
                var (xa, ya) = galA;
                var (xb, yb) = galB;

                long dist = 0;

                for (int x = Math.Min(xa, xb); x < Math.Max(xa, xb); x++)
                {
                    dist++;
                    if (ExpandedX.Contains(x)) { dist += Expansion; }
                }
                for (int y = Math.Min(ya, yb); y < Math.Max(ya, yb); y++)
                {
                    dist++;
                    if (ExpandedY.Contains(y)) { dist += Expansion; }
                }

                return dist;
            }
        }


        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();
        private List<string> Tokenize(string line, IEnumerable<char> splitChars) => line.Split(splitChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

        private HashSet<Coord> ParseGalaxyCoords(string input)
        {
            var lns = SplitToLines(input);

            var mapUniverse = lns.Index().SelectMany(lny =>
            {
                int y = lny.Key;
                return lny.Value.ToCharArray().Index().Select(partx => ((partx.Key, y), partx.Value));
            }).ToList();
            var mapGalaxy = mapUniverse.Where(tpl => tpl.Value == '#').ToList();

            HashSet<Coord> galCoords = mapGalaxy.Select(tpl => tpl.Item1).ToHashSet();

            return galCoords;
        }


        private HashSet<int> GetEmptyDimension<T>(HashSet<T> galCoords, Func<T, int> getDim)
        {
            int maxDim = galCoords.Max(getDim);
            return Enumerable.Range(0, maxDim).Where(idim => !galCoords.Any(gc => getDim(gc) == idim)).ToHashSet();
        }

        public long Solve1(string input)
        {
            HashSet<Coord> galCoords = ParseGalaxyCoords(input);
            HashSet<int> emptyX = GetEmptyDimension(galCoords, tpl => tpl.x);
            HashSet<int> emptyY = GetEmptyDimension(galCoords, tpl => tpl.y);

            var expandedModel = new ExpandedUniverseModel(emptyX, emptyY, 1);

            List<long> allDists =
                galCoords
                .SelectMany(galA =>
                    galCoords
                    .Where(galB => galB.y > galA.y || galB.y == galA.y && galB.x > galA.x)
                    .Select(galB => expandedModel.CalcDist(galA, galB)))
                .ToList();

            long res = allDists.Sum();
            return res;
        }


        public long Solve2(string input)
        {
            HashSet<Coord> galCoords = ParseGalaxyCoords(input);
            HashSet<int> emptyX = GetEmptyDimension(galCoords, tpl => tpl.x);
            HashSet<int> emptyY = GetEmptyDimension(galCoords, tpl => tpl.y);

            var expandedModel = new ExpandedUniverseModel(emptyX, emptyY, 1000000 - 1);

            List<long> allDists =
                galCoords
                .SelectMany(galA =>
                    galCoords
                    .Where(galB => galB.y > galA.y || galB.y == galA.y && galB.x > galA.x)
                    .Select(galB => expandedModel.CalcDist(galA, galB)))
                .ToList();

            long res = allDists.Sum();
            return res;
        }
    }
}
