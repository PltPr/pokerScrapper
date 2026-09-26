using api.Models;

namespace api.Interface
{
    public interface ICalculateService
    {
        HandValue EvaluateBestHand(List<Card> cards);
        HandValue EvaluateFiveCards(List<Card> cards);
        int CompareHands(HandValue first, HandValue second);
    }
}