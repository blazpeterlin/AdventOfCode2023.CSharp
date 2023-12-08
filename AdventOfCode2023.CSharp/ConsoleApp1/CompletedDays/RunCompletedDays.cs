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

        public void RunDay04()
        {
            string input = File.ReadAllText("CompletedDays/inputDay04.txt");

            var sln = new SolutionDay04();
            int res1 = sln.Solve1(input);
            Trace.Assert(res1 == 26218);
            int res2 = sln.Solve2(input);
            Trace.Assert(res2 == 9997537);
        }

        public void RunDay05()
        {
            string input = File.ReadAllText("CompletedDays/inputDay05.txt");

            var sln = new SolutionDay05();
            long res1 = sln.Solve1(input);
            Trace.Assert(res1 == 340994526L);
            long res2 = sln.Solve2(input);
            Trace.Assert(res2 == 52210644L);
        }

        public void RunDay06()
        {
            string input = File.ReadAllText("CompletedDays/inputDay06.txt");

            var sln = new SolutionDay06();
            long res1 = sln.Solve1(input);
            Trace.Assert(res1 == 505494L);
            long res2 = sln.Solve2(input);
            Trace.Assert(res2 == 23632299L);
        }

        public void RunDay07()
        {
            string input = File.ReadAllText("CompletedDays/inputDay07.txt");

            var sln = new SolutionDay07();
            long res1 = sln.Solve1(input);
            Trace.Assert(res1 == 247823654L);
            long res2 = sln.Solve2(input);
            Trace.Assert(res2 == 245461700L);
        }

        public void RunDay08()
        {
            string input = File.ReadAllText("CompletedDays/inputDay08.txt");

            var sln = new SolutionDay08();
            long res1 = sln.Solve1(input);
            Trace.Assert(res1 == 21389);
            long res2 = sln.Solve2(input);
            Trace.Assert(res2 == 21083806112641L);
        }
    }
}
