using MoreLinq;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Environment;

namespace Aoc2023.ActiveDay
{
    internal class SolutionDay05
    {
        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();
        private List<string> Tokenize(string line, IEnumerable<char> splitChars) => line.Split(splitChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

        record struct Aoc05Range(long Start, long Len)
        {
            public long EndExcl => Start + Len;
            public bool Contains(long num) => num >= Start && num < EndExcl;

            public (Aoc05Range left, Aoc05Range right) SplitAt(long Num)
            {
                return (
                        new Aoc05Range(Start, Num - Start),
                        new Aoc05Range(Num, EndExcl - Num)
                    );
            }
        }

        record struct Aoc05Mapping(long TargetStart, long SourceStart, long Len)
        {
            public Aoc05Range SourceRange => new Aoc05Range(SourceStart, Len);
            public static Aoc05Mapping Parse(string ln)
            {
                if (ln.Split(" ").Select(long.Parse).ToList() is not [long a, long b, long c]) { throw new(); }
                return new Aoc05Mapping(a, b, c);
            }

            public long MapUnsafe(long val)
            {
                return TargetStart + val - SourceStart;
            }



            public Aoc05Range MapRangeUnsafe(Aoc05Range rng)
            {
                return new Aoc05Range(MapUnsafe(rng.Start), rng.Len);
            }
        }

        public long Solve1(string input)
        {
            var chunks = Regex.Split(input, NewLine + NewLine).Where(ln => ln != "").ToList();
            var seeds = chunks.First().Split(" ").Skip(1).Select(long.Parse);
            List<List<Aoc05Mapping>> mappingStages =
                chunks.Skip(1)
                .Select(chnk => SplitToLines(chnk).Skip(1).Select(Aoc05Mapping.Parse).ToList())
                .ToList();

            var seedLocations =
                seeds
                .Select(ev => {
                    long tempRes = ev;
                    foreach (var mappingStage in mappingStages)
                    {
                        tempRes = mappingStage.FirstOrDefault(mapping => mapping.SourceRange.Contains(tempRes)).MapUnsafe(tempRes);
                    }
                    return tempRes;
                })
                .ToList();

            long res = seedLocations.Min();

            return res;
        }

        public long Solve2(string input)
        {
            var chunks = Regex.Split(input, NewLine + NewLine).Where(ln => ln != "").ToList();
            List<Aoc05Range> seedRanges = chunks.First().Split(" ").Skip(1).Select(long.Parse).Chunk(2).Select(chnk => new Aoc05Range(chnk[0], chnk[1])).ToList();
            List<List<Aoc05Mapping>> mappingStages =
                chunks.Skip(1)
                .Select(chnk => SplitToLines(chnk).Skip(1).Select(Aoc05Mapping.Parse).ToList())
                .ToList();

            var finalRanges =
                seedRanges
                .SelectMany(rng => {
                    List<Aoc05Range> tempRanges = new() { rng };

                    foreach (var mappingStage in mappingStages)
                    {
                        foreach (Aoc05Mapping mapping in mappingStage)
                        {
                            // split if a range crosses border
                            tempRanges = SplitRanges(tempRanges, mapping).ToList();
                        }

                        tempRanges = tempRanges.Select(rng =>
                        {
                            return mappingStage
                                .FirstOrDefault(mapping => mapping.SourceRange.Contains(rng.Start))
                                .MapRangeUnsafe(rng);
                        }).ToList();
                    }
                    return tempRanges;
                }).ToList();
            
            long res = finalRanges.Min(fr => fr.Start);

            return res;
        }

        private IEnumerable<Aoc05Range> SplitRanges(List<Aoc05Range> ranges, Aoc05Mapping mapping)
        {
            foreach (Aoc05Range inputRng in ranges)
            {
                Aoc05Range rng = inputRng;
                var srcRng = mapping.SourceRange;

                if (rng.Contains(srcRng.Start))
                {
                    var (leftRng, rightRng) = rng.SplitAt(srcRng.Start);
                    yield return leftRng;
                    rng = rightRng;
                }
                if (rng.Contains(srcRng.EndExcl))
                {
                    var (leftRng, rightRng) = rng.SplitAt(srcRng.EndExcl);
                    yield return leftRng;
                    rng = rightRng;
                }
                
                if (srcRng.Len > 0) { yield return rng; }
            }
        }
    }
}
