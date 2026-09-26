using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.Interface;
using api.Models;

namespace api.Service
{
    public class MonteCarloService : IMonteCarloService
    {
        private readonly ICalculateService _calculate;

        public MonteCarloService(ICalculateService calculate)
        {
            _calculate = calculate;
        }

        public double CalculateWinChance(
            List<Card> myHand,
            List<Card> tableCards,
            int numberOfSimulations = 100000)
        {
            var baseDeck = GenerateDeck();

            RemoveUsedCards(
                baseDeck,
                myHand,
                tableCards
            );

            int wins = 0;
            int ties = 0;

            var threadLocalRandom =
                new ThreadLocal<Random>(
                    () => new Random(Guid.NewGuid().GetHashCode())
                );

            Parallel.For(
                0,
                numberOfSimulations,

                () => (wins: 0, ties: 0),

                (i, state, local) =>
                {
                    var rng = threadLocalRandom.Value!;

                    var deck = baseDeck.ToArray();

                    Shuffle(deck, rng);

                    int index = 0;

                    var simulatedTable =
                        new List<Card>(tableCards);

                    while (simulatedTable.Count < 5)
                    {
                        simulatedTable.Add(deck[index++]);
                    }

                    var opponent1 = new List<Card>
                    {
                        deck[index++],
                        deck[index++]
                    };

                    var opponent2 = new List<Card>
                    {
                        deck[index++],
                        deck[index++]
                    };

                    var myBest =
                        _calculate.EvaluateBestHand(
                            myHand
                                .Concat(simulatedTable)
                                .ToList()
                        );

                    var opp1Best =
                        _calculate.EvaluateBestHand(
                            opponent1
                                .Concat(simulatedTable)
                                .ToList()
                        );

                    var opp2Best =
                        _calculate.EvaluateBestHand(
                            opponent2
                                .Concat(simulatedTable)
                                .ToList()
                        );

                    int myVsOpp1 =
                        _calculate.CompareHands(
                            myBest,
                            opp1Best
                        );

                    int myVsOpp2 =
                        _calculate.CompareHands(
                            myBest,
                            opp2Best
                        );

                    bool beatsOpp1 = myVsOpp1 > 0;
                    bool beatsOpp2 = myVsOpp2 > 0;

                    bool tieWithOpp1 = myVsOpp1 == 0;
                    bool tieWithOpp2 = myVsOpp2 == 0;

                    if (beatsOpp1 && beatsOpp2)
                    {
                        local.wins++;
                    }
                    else if (
                        (beatsOpp1 && tieWithOpp2) ||
                        (beatsOpp2 && tieWithOpp1) ||
                        (tieWithOpp1 && tieWithOpp2))
                    {
                        local.ties++;
                    }

                    return local;
                },

                localTotals =>
                {
                    Interlocked.Add(
                        ref wins,
                        localTotals.wins
                    );

                    Interlocked.Add(
                        ref ties,
                        localTotals.ties
                    );
                }
            );

            return (wins + ties / 2.0)
                   / numberOfSimulations;
        }

        private List<Card> GenerateDeck()
        {
            var deck = new List<Card>();

            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    deck.Add(new Card(rank, suit));
                }
            }

            return deck;
        }

        private void RemoveUsedCards(
            List<Card> deck,
            List<Card> myHand,
            List<Card> tableCards)
        {
            deck.RemoveAll(card =>
                myHand.Any(handCard =>
                    handCard.Rank == card.Rank &&
                    handCard.Suit == card.Suit)

                ||

                tableCards.Any(tableCard =>
                    tableCard.Rank == card.Rank &&
                    tableCard.Suit == card.Suit)
            );
        }

        private void Shuffle(
            Card[] cards,
            Random rng)
        {
            for (int i = cards.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);

                (cards[i], cards[j]) =
                    (cards[j], cards[i]);
            }
        }
    }
}