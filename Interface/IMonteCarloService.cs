using api.Models;

namespace api.Interface
{
    public interface IMonteCarloService
    {
        double CalculateWinChance(
            List<Card> myHand,
            List<Card> tableCards,
            int numberOfSimulations = 100000
        );
    }
}