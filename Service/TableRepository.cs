using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using api.Dto;
using api.Interface;

namespace api.Service
{
    public class TableRepository : ITableRepository
    {
        private readonly ISnapshotRepository _snapshotRepo;
        public TableRepository(ISnapshotRepository snapshotRepo)
        {
            _snapshotRepo = snapshotRepo;
        }

        public async Task<CaptureDto> CaptureTable()
        {
            var bounds = new Rectangle(260, 0, 1400, 1030);

            await Task.Delay(2000);

            using var bmp = new Bitmap(bounds.Width, bounds.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bmp.Size);
            }

            var projectFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectFolder,"table","ggpoker.png");

            bmp.Save(filePath, ImageFormat.Png);

            return new CaptureDto
            {
                Status = "Captured",
                FilePath = filePath,
                Resolution = $"{bmp.Width}x{bmp.Height}",
                Timestamp = DateTime.UtcNow
            };
        }

        public string[] GetHand()
        {
            var currFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var screenshotPath = Path.Combine(currFolder,"table", "ggpoker.png");
            var projectFolder = Path.Combine(currFolder, "table", "myhand");

            if (!System.IO.File.Exists(screenshotPath))
                throw new Exception("Screenshot not found");

            using var bmp = new Bitmap(screenshotPath);

            var bothCardsRect = new Rectangle(600,745,140,100);
            using var bothCardBmp = bmp.Clone(bothCardsRect, bmp.PixelFormat);
            var bothCardPath = Path.Combine(projectFolder, "bothcard.png");
            bothCardBmp.Save(bothCardPath, ImageFormat.Png);

            

            var firstCardValue = new Rectangle(607, 753, 41, 52);
            var firstCardSymbol = new Rectangle(611, 801, 41, 40);

            using var card1valueBmp = bmp.Clone(firstCardValue, bmp.PixelFormat);
            var card1valuePath = Path.Combine(projectFolder, "card1value.png");
            card1valueBmp.Save(card1valuePath, ImageFormat.Bmp);
            string stringcard1val = _snapshotRepo.RecognizeValueWithTesseract(card1valuePath);

            using var card1symbolBmp = bmp.Clone(firstCardSymbol, bmp.PixelFormat);
            var card1symbolPath = Path.Combine(projectFolder, "card1symbol.png");
            card1symbolBmp.Save(card1symbolPath, ImageFormat.Png);
            string stringcard1symbol = _snapshotRepo.RecognizeSymbol(card1symbolPath, "First");

            var secoundCardValue = new Rectangle(687, 748, 39, 50);
            var secoundCardSymbol = new Rectangle(684, 798, 39, 40);

            using var card2valueBmp = bmp.Clone(secoundCardValue, bmp.PixelFormat);
            var card2valuePath = Path.Combine(projectFolder, "card2value.png");
            card2valueBmp.Save(card2valuePath, ImageFormat.Bmp);
            string stringcard2val = _snapshotRepo.RecognizeValueWithTesseract(card2valuePath);

            using var card2symbolBmp = bmp.Clone(secoundCardSymbol, bmp.PixelFormat);
            var card2symbolPath = Path.Combine(projectFolder, "card2symbol.png");
            card2symbolBmp.Save(card2symbolPath, ImageFormat.Png);
            string stringcard2symbol = _snapshotRepo.RecognizeSymbol(card2symbolPath, "Secound");

            string card1result = string.Concat(stringcard1val, stringcard1symbol);
            string card2result = string.Concat(stringcard2val, stringcard2symbol);

            return new string[] { card1result, card2result };
        }
    }
}