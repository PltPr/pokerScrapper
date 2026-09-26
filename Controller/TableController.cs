using api.Interface;
using api.Service;
using Microsoft.AspNetCore.Mvc;

namespace api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TableController : ControllerBase
    {
        private readonly ITableRepository _tableRepo;
        private readonly ICardService _cardService;
        private readonly ICalculateService _calculateService;
        private readonly IMonteCarloService _monteCarloService;
        public TableController(ITableRepository tableRepo,ICardService cardService,ICalculateService calculateService, IMonteCarloService monteCarloService)
        {
            _tableRepo = tableRepo;
            _cardService = cardService;
            _calculateService = calculateService;
            _monteCarloService = monteCarloService;
        }

        [HttpGet("CaptureTable")]
        public async Task<IActionResult> CaptureTable()
        {
            var result = await _tableRepo.CaptureTable();

            return Ok(result);
        }

        [HttpGet("GetMyHand")]
        public IActionResult GetMyHand()
        {
            var cards = _tableRepo.GetHand();
            var result = _cardService.ParseCards(cards);

            return Ok(result);
        }
        [HttpGet("GetTableCards")]
        public IActionResult GetTableCards()
        {
            var cards = _tableRepo.GetTableCards();
            var result = _cardService.ParseCards(cards);
            return Ok(result);
        }
        [HttpGet("TestCalculate")]
        public IActionResult TestCalculate()
        {
            var hand = _cardService.ParseCards(new[]
            {
        "Akaro",
        "APik"
    });

            var table = _cardService.ParseCards(new[]
            {
        "6karo",
        "Jpik",
        "7trefl",
        "6pik",
        "Kpik"
    });

            var allCards = hand.Concat(table).ToList();

            var result = _calculateService.EvaluateBestHand(allCards);

            return Ok(result);
        }
        [HttpGet("TestMonteCarlo")]
        public IActionResult TestMonteCarlo()
        {
            var hand = _cardService.ParseCards(new[]
            {
        "Akaro",
        "APik"
    });

            var table = _cardService.ParseCards(new[]
            {
        "6karo",
        "Jpik",
        "7trefl",
        "6pik",
        "Kpik"
    });

            var chance = _monteCarloService.CalculateWinChance(
                hand,
                table,
                100000
            );

            return Ok(new
            {
                ChanceOfWin = chance
            });
        }
        [HttpGet("GetDecision")]
        public IActionResult GetDecision(int pot, int toCall)
        {
            var tableCards = _tableRepo.GetTableCards();
            var formattedTableCards = _cardService.ParseCards(tableCards);

            var handCards = _tableRepo.GetHand();
            var formattedHandCards = _cardService.ParseCards(handCards);

            var chanceOfWin = _monteCarloService.CalculateWinChance(
                formattedHandCards,
                formattedTableCards,
                100000
            );

            double potOdds = (double)toCall / (pot + toCall);

            string action;

            if (chanceOfWin < potOdds)
                action = "Fold";
            else if (chanceOfWin == potOdds && chanceOfWin < 0.7)
                action = "Call";
            else
                action = "Raise";

            return Ok(new
            {
                action,
                ChanceOfWin = chanceOfWin,
                PotOdds = potOdds
            });
        }
    }
}