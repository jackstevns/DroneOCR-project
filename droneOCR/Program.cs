using System;
using Tesseract;


class Program
{


    public static string ExtractTextFromImage(string imagePath)
    {
        using (var engine = new TesseractEngine(@"../tessdata", "eng", EngineMode.Default))
        {
            using (var img = Pix.LoadFromFile(imagePath))
            {
                using (var page = engine.Process(img))
                {
                    return page.GetText();
                }
            }
        }
    }

        static void Main(string[] args)
    {
        // Specify the path to your image here
        string imagePath = @"../phototest.tif";
        string extractedText = ExtractTextFromImage(imagePath);
        Console.WriteLine("Extracted Text:");
        Console.WriteLine(extractedText);
    }


}


