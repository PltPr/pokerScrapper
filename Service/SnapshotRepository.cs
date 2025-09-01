using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using api.Interface;
using Tesseract;
using System.Drawing.Imaging.Effects;
using Emgu.CV;
using Emgu.CV.Structure;


namespace api.Service
{
    public class SnapshotRepository : ISnapshotRepository
    {
        public string RecognizeSymbol(string filePath, string type)
        {
            using var bmp = new Bitmap(filePath);
            Image<Gray, byte> grayImage = bmp.ToImage<Gray, byte>();
            var templateFolder = "";

            if (type == "First")
            {
                templateFolder = Path.Combine("Templates", "FirstCardTemplates");
            }
            else if (type == "Secound")
            {
                templateFolder = Path.Combine("Templates", "SecoundCardTemplates");
            }
            else if (type == "Table")
            {
                templateFolder = Path.Combine("Templates", "TableTemplates");
            }

            var templates = new Dictionary<string, string>
            {
                {"pik",Path.Combine(templateFolder,"pik.png")},
                {"pik2",Path.Combine(templateFolder,"pik2.png")},
                {"kier",Path.Combine(templateFolder,"kier.png")},
                {"karo",Path.Combine(templateFolder,"karo.png")},
                {"trefl",Path.Combine(templateFolder,"trefl.png")}
            };

            string bestMatch = "";
            double maxScore = double.MinValue;

            foreach (var x in templates)
            {
                using var templateBmp = new Bitmap(x.Value);
                Image<Gray, byte> template = templateBmp.ToImage<Gray, byte>();

                using var result = grayImage.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
                double[] minValues, maxValues;
                Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                if (maxValues[0] > maxScore)
                {
                    maxScore = maxValues[0];
                    bestMatch = x.Key;
                }
            }
            if (maxScore < 0.7) return String.Empty;

            var validator = new List<string>
            {
                "pik","kier","karo","trefl"
            };

            var match = validator.FirstOrDefault(v => bestMatch.Contains(v));

            

            return match ?? string.Empty;
        }

        public string RecognizeValueWithTesseract(string filePath)
        {
            using var ocr = new TesseractEngine(
                @"./tessdata",
                "eng",
                EngineMode.Default
            );
            ocr.SetVariable("tessedit_char_whitelist", "0123456789QKAJ");
            using var pix = Pix.LoadFromFile(filePath);

            using var page = ocr.Process(pix, PageSegMode.SingleWord);
            string value = page.GetText().Trim();

            value = value.Replace(" ", "").Replace("\n", "");
            if (value == "0" || value == "1")
                return string.Empty;

            if (value.Length >= 2 && value != "10")
                return value[0].ToString();

            if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            return string.Empty;
            
        }
    }
}