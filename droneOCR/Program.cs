using System;
using Tesseract;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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

        static async Task Main(string[] args)
    {
        // Specify the path to your image here
        string imagePath = @"../phototest.tif";
        string extractedText = ExtractTextFromImage(imagePath);
        Console.WriteLine("Extracted Text:");
        Console.WriteLine(extractedText);
    
        string apiKey = "API KEY HERE";
        string apiUrl = "https://api.openai.com/v1/chat/completions";

        string prompt = $"You: what I am reading {extractedText}";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a helpful assistant."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                }
            };

            var response = await client.PostAsync(apiUrl, new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Response was successful: {response.IsSuccessStatusCode}");
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response was successful: {responseBody}");
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {   
                    JsonElement root = doc.RootElement;
                    JsonElement choices = root.GetProperty("choices");
                    string aiResponse = choices[0].GetProperty("message").GetProperty("content").GetString();

                    Console.WriteLine($"AI: {aiResponse}");
                }
        }
    }
    }
}
