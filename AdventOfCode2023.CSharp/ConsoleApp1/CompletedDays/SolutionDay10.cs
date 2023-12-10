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
using static System.Environment;

using Dir = (int dx, int dy);
using Point = (int x, int y);

namespace Aoc2023.ActiveDay
{
    internal class SolutionDay10
    {
        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();
        private List<string> Tokenize(string line, IEnumerable<char> splitChars) => line.Split(splitChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

        Dir East => (1, 0);
        Dir West => (-1, 0);
        Dir North => (0, -1);
        Dir South => (0, 1);

        List<Dir> GetPossibleDirs(char ch)
        {
            return ch switch
            {
                '|' => [North, South],
                '-' => [East, West],
                'L' => [North, East],
                'J' => [North, West],
                '7' => [South, West],
                'F' => [South, East],
                '.' => [],
                _ => throw new()
            };
        }

        List<Dir> AllDirs => [North, South, East, West];

        Point Move(Point p, Dir d) => (p.x + d.dx, p.y + d.dy);


        private char GetStartChar(Point startPoint, Dictionary<Point, char> map)
        {
            List<Dir> startDirs =
                new[] { East, North, West, South }
                .Where(dir =>
                    Move(startPoint, dir) is Point nextPos
                    && map.ContainsKey(nextPos)
                    && map[nextPos] is char nextCh
                    && GetPossibleDirs(nextCh).Select(nextDir =>
                        Move(nextPos, nextDir)).Contains(startPoint))
                .ToList();
            return
                startDirs.OrderBy(pt => pt.dx).ThenBy(pt => pt.dy).ToList() switch
                {
                    [Dir a, Dir b] when a == North && b == South => 'I',
                    [Dir a, Dir b] when a == West && b == East => '-',
                    [Dir a, Dir b] when a == North && b == East => 'L',
                    [Dir a, Dir b] when a == West && b == North => 'J',
                    [Dir a, Dir b] when a == West && b == South => '7',
                    [Dir a, Dir b] when a == South && b == East => 'F',
                    _ => throw new()
                };
        }

        public long Solve1(string input)
        {
            Dictionary<Point, char> map =
                SplitToLines(input)
                .Index()
                .SelectMany(pair => pair is not (int y, string ln) ? throw new() : ln.ToCharArray().Index().Select(pairX => ((pairX.Key, y),pairX.Value)))
                .Where(pair => pair.Value != '.')
                .ToDictionary(x => x.Item1, x => x.Value);

            Point startPoint = map.Single(m => m.Value == 'S').Key;
            char startChar = GetStartChar(startPoint, map);
            map[startPoint] = startChar;

            var (steps, _) = TraverseMainLoop(startPoint, map);

            return steps;
        }

        private (int steps, List<Point> mainLoopPoints) TraverseMainLoop(Point startPoint, Dictionary<Point, char> map)
        {
            int steps = 0;
            HashSet<Point> alreadyTaken = new([startPoint]);
            List<Point> nextEdges = GetPossibleDirs(map[startPoint]).Select(d => Move(startPoint, d)).ToList();
            while (true)
            {
                if (!nextEdges.Any()) { break; }
                foreach (var edge in nextEdges) { alreadyTaken.Add(edge); }

                List<Point> candidatesAfter = nextEdges.SelectMany(edge =>
                    map.ContainsKey(edge)
                    ? GetPossibleDirs(map[edge]).Select(dir => Move(edge, dir))
                    : []
                    ).ToList();
                List<Point> edgesAfter = candidatesAfter.Where(pt => !alreadyTaken.Contains(pt)).Distinct().ToList();
                nextEdges = edgesAfter;

                steps++;
            }

            return (steps, alreadyTaken.ToList());
        }

        public long Solve2(string input)
        {
            Dictionary<Point, char> map =
                SplitToLines(input)
                .Index()
                .SelectMany(pair => pair is not (int y, string ln) ? throw new() : ln.ToCharArray().Index().Select(pairX => ((pairX.Key, y), pairX.Value)))
                //.Where(pair => pair.Value != '.')
                .ToDictionary(x => x.Item1, x => x.Value);

            Point startPoint = map.Single(m => m.Value == 'S').Key;
            char startChar = GetStartChar(startPoint, map);
            map[startPoint] = startChar;

            Dictionary<(int x, int y), char> expandedMap = ExpandMap(map);

            var (_, mainLoopPts) = TraverseMainLoop(startPoint, map);
            var expandedMainLoopPts = mainLoopPts.Select(pt => (pt.x * 2, pt.y * 2)).ToList();

            var enclosedPoints= FindEnclosedPoints(startPoint, expandedMap, expandedMainLoopPts);


            int res = enclosedPoints.Count;
            return res;
        }

        private List<Point> FindEnclosedPoints(Point startPoint, Dictionary<Point, char> expandedMap, List<Point> expandedMainLoopPts)
        {
            foreach (Point pt in expandedMap.Keys.ToList())
            {
                if (!expandedMainLoopPts.Contains(pt) && expandedMap[pt] != '.') { expandedMap[pt] = '.'; }
            }

            HashSet<Point> escapablePoints = GetEscapablePoints(expandedMap);

            var walkablePoints = expandedMap
                .Where(kvp => kvp.Key.x % 2 == 0 && kvp.Key.y % 2 == 0) // original (not expanded) points only
                .Select(kvp => kvp.Key)
                .Except(expandedMainLoopPts) // exclude main loop
                .ToList();

            var enclosedPoints = walkablePoints.Except(escapablePoints).ToList();
            return enclosedPoints;
        }

        private HashSet<(int x, int y)> GetEscapablePoints(Dictionary<Point, char> expandedMap)
        {
            HashSet<Point> expandedWalls = MapToWalls(expandedMap).ToHashSet();

            //CommonPrint.PrintMap(expandedWalls);

            int maxX = expandedMap.Keys.Max(pt => pt.x);
            int maxY = expandedMap.Keys.Max(pt => pt.y);

            HashSet<Point> escapablePoints = new();

            for (int x = -1; x <= maxX + 1; x++) { escapablePoints.Add((x, -1)); escapablePoints.Add((x, maxY + 1)); }
            for (int y = -1; y <= maxY + 1; y++) { escapablePoints.Add((-1, y)); escapablePoints.Add((maxX + 1, y)); }
            List<Point> nextEdges = escapablePoints.SelectMany(pt => AllDirs.Select(d => Move(pt, d))).Distinct().ToList();
            while (true)
            {
                nextEdges = nextEdges
                    .Where(pt => !expandedWalls.Contains(pt))
                    .Where(pt => pt.x >= 0 && pt.x <= maxX && pt.y >= 0 && pt.y <= maxY)
                    .ToList();

                if (!nextEdges.Any()) { break; }
                foreach (var edge in nextEdges) { escapablePoints.Add(edge); }

                List<Point> candidatesAfter = nextEdges.SelectMany(edge =>
                    new[] { Move(edge, South), Move(edge, North), Move(edge, East), Move(edge, West) }
                    ).ToList();
                List<Point> edgesAfter = candidatesAfter.Where(pt => !escapablePoints.Contains(pt)).Distinct().ToList();
                nextEdges = edgesAfter.Distinct().ToList();
            }

            return escapablePoints;
        }

        private Dictionary<Point, char> ExpandMap(Dictionary<Point, char> map)
        {
            Dictionary<Point, char> expandedMap = map.ToDictionary(kvp => (kvp.Key.x * 2, kvp.Key.y * 2), kvp => kvp.Value);
            return expandedMap;
        }

        private List<Point> MapToWalls(Dictionary<Point, char> expandedMap)
        {
            HashSet<Point> existingExpanded = new(expandedMap.Keys);

            int maxX = existingExpanded.Max(pt => pt.x);
            int maxY = existingExpanded.Max(pt => pt.y);

            List<Point> walls = expandedMap.Where(kvp => kvp.Value != '.').Select(kvp => kvp.Key).ToList();

            for (int x = 0; x <= maxX; x++)
            {
                for (int y = 0; y <= maxY; y++)
                {
                    Point pt = (x, y);
                    if (existingExpanded.Contains(pt)) { continue; }

                    List<Dir> allDirs = [North, East, South, West];
                    List<Point> movablePointsFromNeighbours =
                        allDirs
                        .Select(dir => Move(pt, dir))
                        .Where(expandedMap.ContainsKey)
                        .SelectMany(newPt => GetPossibleDirs(expandedMap[newPt]).Select(expDir => Move(newPt, expDir)))
                        .ToList();

                    if (movablePointsFromNeighbours.Where(pti => pti == pt).Count() >= 2)
                    {
                        walls.Add(pt);
                    }
                }
            }
            return walls;
        }
    }
}
