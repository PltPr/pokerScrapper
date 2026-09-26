using api.Models;

namespace api.Interface
{
    public interface ICardService
    {
        Card ParseCard(string card);
        List<Card> ParseCards(string[] cards);
    }
}
