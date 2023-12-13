using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static System.Environment;

namespace Aoc2023.ActiveDay
{
    internal class SolutionDay13
    {
        private List<string> SplitToGroups(string input) => Regex.Split(input, NewLine + NewLine).Where(ln => ln != "").ToList();
        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();
        private List<string> Tokenize(string line, IEnumerable<char> splitChars) => line.Split(splitChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();



        int? FindSplit(List<List<char>> charSets, int? ignoreIdx)
        {
            for (int x = 1; x <= charSets[0].Count - 1; x++)
            {
                int len = Math.Min(x, charSets[0].Count - x);

                if(x == ignoreIdx) { continue; }

                var huh = charSets.Select(charSet =>
                    charSet[(x - len)..x] is var mirrLeft
                    && charSet[x..(x + len)] is var mirrRight
                    && new string(mirrLeft.ToArray()) == new string(mirrRight.Reverse<char>().ToArray())).ToList();

                if (charSets.All(charSet =>
                    charSet[(x - len)..x] is var mirrLeft
                    && charSet[x..(x + len)] is var mirrRight
                    && new string(mirrLeft.ToArray()) == new string(mirrRight.Reverse<char>().ToArray())))
                {
                    return x;
                }
            }

            return null;
        }
        int? FindMirrorX(string grp, int? ignoreIdx)
        {
            var lns = SplitToLines(grp);
            var charSets = lns.Select(ln => ln.ToCharArray().ToList()).ToList();

            return FindSplit(charSets, ignoreIdx);
        }

        int? FindMirrorY(string grp, int? ignoreIdx)
        {
            var lns = SplitToLines(grp);
            var charSetsTransposed = lns.Select(ln => ln.ToCharArray().ToList()).ToList()
                .Transpose()
                .Select(col => col.ToList()).ToList();

            return FindSplit(charSetsTransposed, ignoreIdx);
        }

        public long Solve1(string input)
        {
            var grps = SplitToGroups(input);

            var scores = grps.Select((grp, idx) =>
            {
                int? x = FindMirrorX(grp, null);
                int? y = FindMirrorY(grp, null);


                return (x, y) switch
                {
                    (int newXVal, _) => newXVal,
                    (_, int newYVal) => 100 * newYVal,
                    _ => throw new(),
                };
            }).ToList();

            long res = scores.Sum();

            return res;
        }

        public IEnumerable<string> MapVariations(string grp)
        {
            for (int i = 0; i < grp.Length; i++)
            {
                if (grp[i] is ('\r' or '\n')) { continue; }
                if (grp[i] is not ('.' or '#')) { throw new(); }

                yield return grp.Substring(0, i) + (grp[i] == '.' ? '#' : '.') + grp.Substring(i + 1);
            }
        }

        public long Solve2(string input)
        {
            var grps = SplitToGroups(input);

            var scores = grps.Select((grp, idx) =>
            {
                int? oldX = FindMirrorX(grp, null);
                int? oldY = FindMirrorY(grp, null);

                foreach(var vtn in MapVariations(grp))
                {
                    int? newX = FindMirrorX(vtn, oldX);
                    int? newY = FindMirrorY(vtn, oldY);

                    int? res = (newX, newY) switch
                    {
                        (int newXVal, _) => newXVal,
                        (_, int newYVal) => 100 * newYVal,
                        _ => null,
                    };

                    if (res != null) { return res.Value; }
                }

                throw new();
            }).ToList();

            long res = scores.Sum();

            return res;
        }
    }
}
