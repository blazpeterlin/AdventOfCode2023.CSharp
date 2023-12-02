﻿using System;
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
    }
}