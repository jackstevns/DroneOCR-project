using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tesseract;

class Program
{
private static readonly HttpClient httpClient = new HttpClient();

    static async Task Main(string[] args)
    {
        // Specify the path to your image here
        string imagePath = @"../phototest.tif";
        string extractedText = ExtractTextFromImage(imagePath);
        Console.WriteLine("Extracted Text:");
        Console.WriteLine(extractedText);

        // Sending extracted text to ChatGPT
        // string response = await GetResponseFromChatGPT(extractedText);
        // Console.WriteLine("Response from ChatGPT:");
        // Console.WriteLine(response);
    }

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

    public static async Task<string> GetResponseFromChatGPT(string inputText)
    {
        // Replace "your_api_key_here" with your actual OpenAI API key
        string apiKey = "your_api_key_here";
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var data = new
        {
            model = "text-davinci-003", // Or another model version
            prompt = inputText,
            temperature = 0.7,
            max_tokens = 150
        };

        string json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("https://api.openai.com/v1/completions", content);
        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<JsonElement>(responseString);
            return responseObject.GetProperty("choices")[0].GetProperty("text").GetString();
        }

        return "Failed to get a response from ChatGPT";
    }
}


