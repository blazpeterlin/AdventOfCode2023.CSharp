using MoreLinq;
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

        private (long targetStart, long sourceStart, long len) parseTuple(string ln)
        {
            if (ln.Split(" ").Select(long.Parse).ToList() is not [long a, long b, long c]) { throw new(); }
            return (a, b, c);
        }

        public long Solve1(string input)
        {
            var chunks = Regex.Split(input, NewLine + NewLine).Where(ln => ln != "").ToList();
            var seeds = chunks.First().Split(" ").Skip(1).Select(long.Parse);
            List<List<(long targetStart, long sourceStart, long len)>> mappings =
                chunks.Skip(1)
                .Select(chnk => SplitToLines(chnk).Skip(1).Select(parseTuple).ToList())
                .ToList();

            var eventLocations =
                seeds
                .Select(ev => {
                    long tempRes = ev;
                    foreach (var mapping in mappings)
                    {
                        foreach(var specificMap in mapping)
                        {
                            if (tempRes >= specificMap.sourceStart && tempRes < specificMap.sourceStart + specificMap.len)
                            {
                                tempRes = specificMap.targetStart + tempRes - specificMap.sourceStart;
                                break;
                            }
                        }

                    }
                    return tempRes;
                })
                .ToList();

            long res = eventLocations.Min();


            // not 3200469
            return res;
        }

        public long Solve2(string input)
        {
            var chunks = Regex.Split(input, NewLine + NewLine).Where(ln => ln != "").ToList();
            List<(long rangeStart, long rangeLen)> seedRanges = chunks.First().Split(" ").Skip(1).Select(long.Parse).Chunk(2).Select(chnk => (chnk[0], chnk[1])).ToList();
            List<List<(long targetStart, long sourceStart, long len)>> mappings =
                chunks.Skip(1)
                .Select(chnk => SplitToLines(chnk).Skip(1).Select(parseTuple).ToList())
                .ToList();
            //seedRanges = new() { (82, 1) };

            var finalRanges =
                seedRanges
                .SelectMany(rng => {
                    List<(long rangeStart, long rangeLen)> tempRanges = new() { rng };

                    foreach (var mapping in mappings)
                    {
                        foreach ((long targetStart, long sourceStart, long len) in mapping)
                        {
                            // split if a range crosses border
                            tempRanges = SplitRanges(tempRanges, sourceStart, len).ToList();
                        }

                        tempRanges = tempRanges.Select(rng => MapRange(rng, mapping)).ToList();
                    }
                    return tempRanges;
                }).ToList();
            
            long res = finalRanges.Min(fr => fr.rangeStart);


            // not 52380323
            return res;
        }

        private (long rangeStart, long rangeLen) MapRange((long rangeStart, long rangeLen) rng, List<(long targetStart, long sourceStart, long len)> mapping)
        {
            foreach (var map in mapping)
            {
                if (rng.rangeStart >= map.sourceStart && rng.rangeStart < map.sourceStart + map.len)
                {
                    return (map.targetStart + rng.rangeStart - map.sourceStart, rng.rangeLen);
                }
            }

            return rng;
        }

        private IEnumerable<(long rangeStart, long rangeLen)> SplitRanges(List<(long rangeStart, long rangeLen)> tempRanges, long sourceStart, long sourceLen)
        {
            foreach ((long inpRangeStart, long inpRangeLen) in tempRanges)
            {
                var (rangeStart, rangeLen) = (inpRangeStart, inpRangeLen);
                if (rangeStart < sourceStart && rangeStart + rangeLen > sourceStart)
                {
                    long newLen = sourceStart - rangeStart;
                    if (newLen <= 0) { }
                    yield return (rangeStart, newLen);
                    (rangeStart, rangeLen) = (sourceStart, rangeLen - newLen);
                }
                if (rangeStart >= sourceStart && rangeStart < sourceStart + sourceLen && rangeStart + rangeLen > sourceStart + sourceLen)
                {
                    long newLen = sourceStart + sourceLen - rangeStart;
                    if (newLen <= 0) { }
                    yield return (rangeStart, newLen);
                    (rangeStart, rangeLen) = (sourceStart + sourceLen, rangeLen - newLen);
                }
                
                if (rangeLen > 0) { yield return (rangeStart, rangeLen); }
            }
        }
    }
}
