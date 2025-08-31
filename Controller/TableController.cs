using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interface;
using Microsoft.AspNetCore.Mvc;

namespace api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TableController : ControllerBase
    {
        private readonly ITableRepository _tableRepo;
        public TableController(ITableRepository tableRepo)
        {
            _tableRepo = tableRepo;
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
            var result = _tableRepo.GetHand();

            return Ok(result);
        }
    }
}