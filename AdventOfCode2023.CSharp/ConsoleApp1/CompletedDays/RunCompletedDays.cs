using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc2023.ActiveDay.CompletedDays
{
    internal class RunCompletedDays
    {
        public void RunDay01()
        {
            string input = File.ReadAllText($"CompletedDays/inputDay01.txt");

            var sln = new SolutionDay01();
            int res1 = sln.Solve1(input);
            Trace.Assert(res1 == 56397);
            int res2 = sln.Solve2ButPrettier(input);
            Trace.Assert(res2 == 55701);
        }

        public void RunDay02()
        {
            string input = File.ReadAllText($"CompletedDays/inputDay02.txt");

            var sln = new SolutionDay02();
            int res1 = sln.Solve1(input);
            Trace.Assert(res1 == 2528);
            int res2 = sln.Solve2(input);
            Trace.Assert(res2 == 67363);
        }

        public void RunDay03()
        {
            string input = File.ReadAllText($"CompletedDays/inputDay03.txt");

            var sln = new SolutionDay03();
            int res1 = sln.Solve1(input);
            Trace.Assert(res1 == 514969);
            int res2 = sln.Solve2(input);
            Trace.Assert(res2 == 78915902);
        }
    }
}
