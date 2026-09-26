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
        public TableController(ITableRepository tableRepo,ICardService cardService,ICalculateService calculateService)
        {
            _tableRepo = tableRepo;
            _cardService = cardService;
            _calculateService = calculateService;
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
        "Qkaro",
        "9karo"
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
    }
}