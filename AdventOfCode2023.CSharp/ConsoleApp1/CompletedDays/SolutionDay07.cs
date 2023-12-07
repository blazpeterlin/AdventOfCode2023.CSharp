using MoreLinq;
using System.Linq;

//using MoreLinq.Extensions;
using System.Text.RegularExpressions;
using static System.Environment;

namespace Aoc2023.ActiveDay
{
    internal class SolutionDay07
    {
        private List<string> SplitToLines(string input) => Regex.Split(input, NewLine).Where(ln => ln != "").ToList();
        private List<string> Tokenize(string line, IEnumerable<char> splitChars) => line.Split(splitChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

        record struct HandBid(List<char> Cards, int Bid)
        {

            static List<char> PossibleCards = ['A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2'];
            static Dictionary<char, int> IndexedCards => PossibleCards.Index().ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            public string Readable => new string(Cards.ToArray());
            public List<char> SortedCards => Cards.OrderBy(ch => IndexedCards[ch]).ToList();

            public List<KeyValuePair<char, int>> CountedCards => SortedCards.CountBy(x => x).OrderByDescending(x => x.Value).ToList();
            public bool Is_FiveOfKind => Cards.Distinct().Count() == 1;
            public bool Is_FourOfKind => CountedCards.First().Value == 4;
            public bool Is_FullHouse => CountedCards is var cc && cc.Count == 2 && cc[0].Value == 3;
            public bool Is_ThreeOfAKind => CountedCards is var cc && cc.Count == 3 && cc[0].Value == 3;
            public bool Is_TwoPair => CountedCards is var cc && cc.Count == 3 && cc[0].Value == 2 && cc[1].Value == 2;
            public bool Is_OnePair => CountedCards is var cc && cc.Count == 4 && cc[0].Value == 2;
            public bool Is_HighCard => CountedCards is var cc && cc.Count == 5;

            public List<bool> PrimarySorting => [Is_FiveOfKind, Is_FourOfKind, Is_FullHouse, Is_ThreeOfAKind, Is_TwoPair, Is_OnePair, Is_HighCard];
            public List<int> SecondarySorting => Cards.Select(c => IndexedCards[c]).ToList();

            public static HandBid Parse(string ln)
            {
                if (ln.Split(" ") is not [string handStr, string bidStr]) { throw new(); }

                List<char> cards = handStr.ToCharArray().ToList();
                int bid = int.Parse(bidStr);

                return new HandBid(cards, bid);
            }
        }

        class HandBidComparer : IComparer<HandBid>
        {
            public int Compare(HandBid x, HandBid y)
            {
                foreach (var (xDefSort, yDefSort) in x.PrimarySorting.Zip(y.PrimarySorting))
                {
                    if (xDefSort && !yDefSort) { return -1; }
                    if (yDefSort && !xDefSort) { return 1; }
                }

                foreach (var (xDefSort, yDefSort) in x.SecondarySorting.Zip(y.SecondarySorting))
                {
                    if (xDefSort < yDefSort) { return -1; }
                    if (xDefSort > yDefSort) { return 1; }
                }

                // never same hand ..?

                return 0;
            }
        }

        public long Solve1(string input)
        {
            var lns = SplitToLines(input);
            var handBids = lns.Select(HandBid.Parse).ToList();

            var huh = handBids.Select(hb => new string(hb.Cards.ToArray())).Where(x => x == "AQT54").ToList();

            handBids.Sort((x, y) => new HandBidComparer().Compare(x,y));
            handBids.Reverse();

            var huh2 = handBids.Select(hb => new string(hb.Cards.ToArray())).Where(x => x == "AQT54").ToList();

            List<long> winnings = handBids.Index().Select(handBidIdx =>
            {
                long rank = handBidIdx.Key + 1;
                var handBid = handBidIdx.Value;

                return rank * (long)handBid.Bid;
            }).ToList();
            
            long res = winnings.Sum();

            return res;
        }




        record struct HandBidJolly(List<char> Cards, int Bid, List<HandBid> MultiverseHandBids)
        {

            static List<char> PossibleCards = ['A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J'];
            static Dictionary<char, int> IndexedCards => PossibleCards.Index().ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            public string Readable => new string(Cards.ToArray());

            public bool Is_FiveOfKind => MultiverseHandBids.Any(mhb => mhb.Is_FiveOfKind);
            public bool Is_FourOfKind => MultiverseHandBids.Any(mhb => mhb.Is_FourOfKind);
            public bool Is_FullHouse => MultiverseHandBids.Any(mhb => mhb.Is_FullHouse);
            public bool Is_ThreeOfAKind => MultiverseHandBids.Any(mhb => mhb.Is_ThreeOfAKind);
            public bool Is_TwoPair => MultiverseHandBids.Any(mhb => mhb.Is_TwoPair);
            public bool Is_OnePair => MultiverseHandBids.Any(mhb => mhb.Is_OnePair);
            public bool Is_HighCard => MultiverseHandBids.Any(mhb => mhb.Is_HighCard);

            public List<bool> PrimarySorting => [Is_FiveOfKind, Is_FourOfKind, Is_FullHouse, Is_ThreeOfAKind, Is_TwoPair, Is_OnePair, Is_HighCard];
            public List<int> SecondarySorting => Cards.Select(c => IndexedCards[c]).ToList();

            public static HandBidJolly Parse(string ln)
            {
                if (ln.Split(" ") is not [string handStr, string bidStr]) { throw new(); }

                List<char> cards = handStr.ToCharArray().ToList();
                int bid = int.Parse(bidStr);

                return new HandBidJolly(cards, bid, CalculateAllPossibleJollyCombinations(cards));
            }

            private static List<HandBid> CalculateAllPossibleJollyCombinations(List<char> cards) =>
                GetAllVariations(ListVariationsByPlace(cards))
                .Select(cards => new HandBid(cards, 0))
                .ToList();

            private static List<List<char>> ListVariationsByPlace(List<char> cards) => cards.Select(c => c switch
            {
                'J' => PossibleCards.Except(['J']).ToList(),
                char ch => new List<char> { ch },
            }).ToList();


            private static IEnumerable<List<char>> GetAllVariations(IEnumerable<List<char>> elements)
            {
                if (elements.Any())
                {
                    foreach (char eltOfPlace in elements.ElementAt(0))
                    {
                        foreach (var innerEltOfPlace in GetAllVariations(elements.Skip(1)))
                        {
                            yield return [eltOfPlace, .. innerEltOfPlace];
                        }
                    }
                }
                else
                {
                    yield return [];
                }
            }
        }



        class HandBidJollyComparer : IComparer<HandBidJolly>
        {
            public int Compare(HandBidJolly x, HandBidJolly y)
            {
                foreach (var (xDefSort, yDefSort) in x.PrimarySorting.Zip(y.PrimarySorting))
                {
                    if (xDefSort && yDefSort) { break; }
                    if (xDefSort && !yDefSort) { return -1; }
                    if (yDefSort && !xDefSort) { return 1; }
                }

                foreach (var (xDefSort, yDefSort) in x.SecondarySorting.Zip(y.SecondarySorting))
                {
                    if (xDefSort < yDefSort) { return -1; }
                    if (xDefSort > yDefSort) { return 1; }
                }

                // never same hand ..?

                return 0;
            }
        }




        public long Solve2(string input)
        {
            var lns = SplitToLines(input);
            var handBids = lns.Select(HandBidJolly.Parse).ToList();

            handBids.Sort((x, y) => new HandBidJollyComparer().Compare(x, y));
            handBids.Reverse();

            var printable = handBids.Select(hb => hb.Readable).ToList();
            var printableStr = string.Join(NewLine, printable);

            List<long> winnings = handBids.Index().Select(handBidIdx =>
            {
                long rank = handBidIdx.Key + 1;
                var handBid = handBidIdx.Value;

                return rank * (long)handBid.Bid;
            }).ToList();

            long res = winnings.Sum();

            return res;
        }
    }
}
