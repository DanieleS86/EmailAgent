namespace EmailAgentSWB.Services
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;

    public class OpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = ""; // meglio usare IConfiguration

        public OpenAiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GeneriereAntwortAsync(string emailText)
        {
            var requestBody = new
            {
                model = "gpt-4",
                messages = new[]
                {
                new { role = "system", content = "Du bist ein freundlicher Kundenservice-Agent." },
                new { role = "user", content = $"Hier ist eine Kundenanfrage:\n\n{emailText}\n\nBitte generiere eine professionelle Antwort." }
            }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var reply = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return reply ?? "(Keine Antwort generiert)";
        }
    }

}
