using api.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;

namespace api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SnapshotController : ControllerBase
    {
        private readonly ISnapshotRepository _snapshotRepo;
        public SnapshotController(ISnapshotRepository snapshotRepo)
        {
            _snapshotRepo = snapshotRepo;
        }


        [HttpGet]
        public async Task<IActionResult> Capture()
        {
            //var bounds = Screen.PrimaryScreen.Bounds;
            var bounds = new Rectangle(260, 0, 1400, 1030);

            await Task.Delay(2000);

            using var bmp = new Bitmap(bounds.Width, bounds.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bmp.Size);
            }

            var projectFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectFolder, "ggpoker.png");

            bmp.Save(filePath, ImageFormat.Png);

            return Ok(new
            {
                Status = "Captured",
                FilePath = filePath,
                Resolution = $"{bmp.Width}x{bmp.Height}",
                Timestamp = DateTime.UtcNow
            });
        }
        [HttpGet("CheckCardValue")]
        public IActionResult CheckCardValue(string name)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var projectFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var cardPath = Path.Combine(projectFolder, $"{name}.png");
            var result = _snapshotRepo.RecognizeValueWithTesseract(cardPath);

            return Ok(result);
        }
        [HttpGet("CheckCardSymbol")]
        public IActionResult CheckCardSymbol(string name)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var projectFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var cardPath = Path.Combine(projectFolder, $"{name}.png");
            var result = _snapshotRepo.RecognizeSymbol(cardPath, "First");

            return Ok(result);
        }


        [HttpGet("GetMyCards")]
        public IActionResult GetMyCards()
        {
            var projectFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var screenshotPath = Path.Combine(projectFolder, "ggpoker.png");

            if (!System.IO.File.Exists(screenshotPath))
                return NotFound("Screenshot not found");

            using var bmp = new Bitmap(screenshotPath);

            var firstCardRegion = new Rectangle(607, 753, 44, 81);
            var firstCardValue = new Rectangle(607, 753, 41, 52);
            var firstCardSymbol = new Rectangle(611, 801, 41, 40);

            using var card1Bmp = bmp.Clone(firstCardRegion, bmp.PixelFormat);
            var card1Path = Path.Combine(projectFolder, "card1.png");
            card1Bmp.Save(card1Path, ImageFormat.Png);

            using var card1valueBmp = bmp.Clone(firstCardValue, bmp.PixelFormat);
            var card1valuePath = Path.Combine(projectFolder, "card1value.png");
            card1valueBmp.Save(card1valuePath, ImageFormat.Bmp);

            using var card1symbolBmp = bmp.Clone(firstCardSymbol, bmp.PixelFormat);
            var card1symbolPath = Path.Combine(projectFolder, "card1symbol.png");
            card1symbolBmp.Save(card1symbolPath, ImageFormat.Png);


            var secoundCardRegion = new Rectangle(687, 748, 39, 90);
            var secoundCardValue = new Rectangle(687, 748, 39, 50);
            var secoundCardSymbol = new Rectangle(684, 798, 39, 40);

            using var card2Bmp = bmp.Clone(secoundCardRegion, bmp.PixelFormat);
            var card2Path = Path.Combine(projectFolder, "card2.png");
            card2Bmp.Save(card2Path, ImageFormat.Png);

            using var card2valueBmp = bmp.Clone(secoundCardValue, bmp.PixelFormat);
            var card2valuePath = Path.Combine(projectFolder, "card2value.png");
            card2valueBmp.Save(card2valuePath, ImageFormat.Bmp);

            using var card2symbolBmp = bmp.Clone(secoundCardSymbol, bmp.PixelFormat);
            var card2symbolPath = Path.Combine(projectFolder, "card2symbol.png");
            card2symbolBmp.Save(card2symbolPath, ImageFormat.Png);

            return Ok();
        }

    }
}