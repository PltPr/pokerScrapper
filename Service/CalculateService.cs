using System;
using System.Collections.Generic;
using System.Linq;
using api.Interface;
using api.Models;

namespace api.Service
{
    public class CalculateService : ICalculateService
    {
        public HandValue EvaluateBestHand(List<Card> cards)
        {
            if (cards == null || cards.Count < 5)
                throw new ArgumentException(
                    "Do oceny ręki potrzebne jest minimum 5 kart."
                );

            if (cards.Count == 5)
                return EvaluateFiveCards(cards);

            HandValue? bestHand = null;

            foreach (var combination in GetFiveCardCombinations(cards))
            {
                var hand = EvaluateFiveCards(combination);

                if (bestHand == null || CompareHands(hand, bestHand) > 0)
                {
                    bestHand = hand;
                }
            }

            return bestHand!;
        }

        public HandValue EvaluateFiveCards(List<Card> cards)
        {
            if (cards == null || cards.Count != 5)
                throw new ArgumentException(
                    "EvaluateFiveCards wymaga dokładnie 5 kart."
                );

            var orderedCards = cards
                .OrderByDescending(c => (int)c.Rank)
                .ToList();

            var ranks = orderedCards
                .Select(c => c.Rank)
                .ToList();

            bool isFlush = cards.All(c => c.Suit == cards[0].Suit);

            var uniqueRanks = ranks
                .Distinct()
                .OrderByDescending(r => (int)r)
                .ToList();

            bool isStraight = false;
            Rank straightHighCard = Rank.Two;

            if (uniqueRanks.Count == 5)
            {
                int highest = (int)uniqueRanks[0];
                int lowest = (int)uniqueRanks[4];

                if (highest - lowest == 4)
                {
                    isStraight = true;
                    straightHighCard = uniqueRanks[0];
                }

                if (uniqueRanks.Contains(Rank.A) &&
                    uniqueRanks.Contains(Rank.Five) &&
                    uniqueRanks.Contains(Rank.Four) &&
                    uniqueRanks.Contains(Rank.Three) &&
                    uniqueRanks.Contains(Rank.Two))
                {
                    isStraight = true;
                    straightHighCard = Rank.Five;
                }
            }

            if (isFlush && isStraight)
            {
                if (straightHighCard == Rank.A)
                {
                    return new HandValue
                    {
                        Rank = HandRank.RoyalFlush,
                        Kickers = new List<Rank> { Rank.A }
                    };
                }

                return new HandValue
                {
                    Rank = HandRank.StraightFlush,
                    Kickers = new List<Rank> { straightHighCard }
                };
            }

            var groups = ranks
                .GroupBy(r => r)
                .Select(g => new
                {
                    Rank = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .ThenByDescending(g => (int)g.Rank)
                .ToList();

            var four = groups.FirstOrDefault(g => g.Count == 4);

            if (four != null)
            {
                var kicker = ranks
                    .Where(r => r != four.Rank)
                    .OrderByDescending(r => (int)r)
                    .First();

                return new HandValue
                {
                    Rank = HandRank.FourOfAKind,
                    Kickers = new List<Rank>
                    {
                        four.Rank,
                        kicker
                    }
                };
            }

            var trips = groups
                .Where(g => g.Count == 3)
                .OrderByDescending(g => (int)g.Rank)
                .ToList();

            var pairs = groups
                .Where(g => g.Count == 2)
                .OrderByDescending(g => (int)g.Rank)
                .ToList();

            if (trips.Count > 0 && (pairs.Count > 0 || trips.Count > 1))
            {
                var threeRank = trips[0].Rank;

                Rank pairRank;

                if (pairs.Count > 0)
                    pairRank = pairs[0].Rank;
                else
                    pairRank = trips[1].Rank;

                return new HandValue
                {
                    Rank = HandRank.FullHouse,
                    Kickers = new List<Rank>
                    {
                        threeRank,
                        pairRank
                    }
                };
            }

            if (isFlush)
            {
                return new HandValue
                {
                    Rank = HandRank.Flush,
                    Kickers = orderedCards
                        .Select(c => c.Rank)
                        .ToList()
                };
            }

            if (isStraight)
            {
                return new HandValue
                {
                    Rank = HandRank.Straight,
                    Kickers = new List<Rank>
                    {
                        straightHighCard
                    }
                };
            }

            if (trips.Count > 0)
            {
                var threeRank = trips[0].Rank;

                var kickers = ranks
                    .Where(r => r != threeRank)
                    .OrderByDescending(r => (int)r)
                    .Take(2)
                    .ToList();

                return new HandValue
                {
                    Rank = HandRank.ThreeOfAKind,
                    Kickers = new List<Rank> { threeRank }
                        .Concat(kickers)
                        .ToList()
                };
            }

            if (pairs.Count >= 2)
            {
                var highPair = pairs[0].Rank;
                var lowPair = pairs[1].Rank;

                var kicker = ranks
                    .Where(r => r != highPair && r != lowPair)
                    .OrderByDescending(r => (int)r)
                    .First();

                return new HandValue
                {
                    Rank = HandRank.TwoPair,
                    Kickers = new List<Rank>
                    {
                        highPair,
                        lowPair,
                        kicker
                    }
                };
            }

            if (pairs.Count == 1)
            {
                var pairRank = pairs[0].Rank;

                var kickers = ranks
                    .Where(r => r != pairRank)
                    .OrderByDescending(r => (int)r)
                    .Take(3)
                    .ToList();

                return new HandValue
                {
                    Rank = HandRank.OnePair,
                    Kickers = new List<Rank> { pairRank }
                        .Concat(kickers)
                        .ToList()
                };
            }

            return new HandValue
            {
                Rank = HandRank.HighCard,
                Kickers = ranks
                    .OrderByDescending(r => (int)r)
                    .Take(5)
                    .ToList()
            };
        }

        public int CompareHands(HandValue first, HandValue second)
        {
            int rankComparison = first.Rank.CompareTo(second.Rank);

            if (rankComparison != 0)
                return rankComparison;

            int count = Math.Min(
                first.Kickers.Count,
                second.Kickers.Count
            );

            for (int i = 0; i < count; i++)
            {
                int kickerComparison =
                    ((int)first.Kickers[i])
                    .CompareTo((int)second.Kickers[i]);

                if (kickerComparison != 0)
                    return kickerComparison;
            }

            return 0;
        }

        private IEnumerable<List<Card>> GetFiveCardCombinations(
            List<Card> cards)
        {
            for (int a = 0; a < cards.Count - 4; a++)
            {
                for (int b = a + 1; b < cards.Count - 3; b++)
                {
                    for (int c = b + 1; c < cards.Count - 2; c++)
                    {
                        for (int d = c + 1; d < cards.Count - 1; d++)
                        {
                            for (int e = d + 1; e < cards.Count; e++)
                            {
                                yield return new List<Card>
                                {
                                    cards[a],
                                    cards[b],
                                    cards[c],
                                    cards[d],
                                    cards[e]
                                };
                            }
                        }
                    }
                }
            }
        }
    }
}