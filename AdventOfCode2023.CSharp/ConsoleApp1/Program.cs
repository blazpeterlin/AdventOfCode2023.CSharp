using Aoc2023.ActiveDay;
using Aoc2023.ActiveDay.CompletedDays;
using System.Diagnostics;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new RunCompletedDays().RunDay05();

            string input = File.ReadAllText("input.txt");

            var sln = new Solution();
            long res1 = sln.Solve1(input);
            //Trace.Assert(res1 == 26218);
            long res2 = sln.Solve2(input);
            //Trace.Assert(res2 == 9997537);

        }
    }
}
