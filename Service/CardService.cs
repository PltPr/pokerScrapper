using api.Interface;
using api.Models;

namespace api.Service
{
    public class CardService : ICardService
    {
        private static readonly string[] suits =
        {
            "trefl",
            "pik",
            "kier",
            "karo"
        };

        public Card ParseCard(string card)
        {
            if (string.IsNullOrWhiteSpace(card))
                throw new ArgumentException("Brak karty.");

            string suitString = suits.FirstOrDefault(
                s => card.EndsWith(s, StringComparison.OrdinalIgnoreCase)
            );

            if (suitString == null)
                throw new ArgumentException("Brak koloru.");

            string rankString =
                card.Substring(0, card.Length - suitString.Length);

            Rank rank = rankString.ToUpper() switch
            {
                "2" => Rank.Two,
                "3" => Rank.Three,
                "4" => Rank.Four,
                "5" => Rank.Five,
                "6" => Rank.Six,
                "7" => Rank.Seven,
                "8" => Rank.Eight,
                "9" => Rank.Nine,
                "10" => Rank.Ten,
                "J" => Rank.J,
                "Q" => Rank.Q,
                "K" => Rank.K,
                "A" => Rank.A,

                _ => throw new ArgumentException(
                    $"Nieznana ranga: {rankString}"
                )
            };

            Suit suit = suitString.ToLower() switch
            {
                "trefl" => Suit.Trefl,
                "pik" => Suit.Pik,
                "kier" => Suit.Kier,
                "karo" => Suit.Karo,

                _ => throw new ArgumentException(
                    $"Nieznany kolor: {suitString}"
                )
            };

            return new Card(rank, suit);
        }

        public List<Card> ParseCards(string[] cards)
        {
            return cards.Select(ParseCard).ToList();
        }
    }
}