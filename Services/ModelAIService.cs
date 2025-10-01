
namespace EmailAgentSWB.Services
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using EmailAgentSWB.Config;
    using Microsoft.Extensions.Options;

    public class ModelAIService
    {
        private readonly HttpClient _httpClient;
        private readonly AISettings _settings;

        public ModelAIService(HttpClient httpClient, IOptions<AISettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<string> GeneriereAntwort(string emailText)
        {
            
            var payload = new
            {
                inputs = $"Hier ist eine Kundenanfrage:\n\n{emailText}\n\nBitte generiere eine professionelle Antwort."
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://api-inference.huggingface.co/models/{_settings.Model}"
            );

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Apikey);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Errore API HuggingFace: {response.StatusCode}\n{responseJson}");
            }

            using var doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
            {
                var first = doc.RootElement[0];

                if (first.TryGetProperty("generated_text", out var generated))
                    return generated.GetString() ?? "(Keine Antwort generiert)";

                if (first.TryGetProperty("summary_text", out var summary))
                    return summary.GetString() ?? "(Keine Antwort generiert)";

                if (first.TryGetProperty("output_text", out var output))
                    return output.GetString() ?? "(Keine Antwort generiert)";
            }

            return "(Antwort konnte nicht gelesen werden)";
            /*

            var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.openai.com/v1/chat/completions"
            );

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Apikey);

            var payload = new
            {
                model = "gpt-4o-mini", // oppure "gpt-4o" o "gpt-5" quando disponibile
                messages = new object[]
                {
                new { role = "system", content = "Sei un assistente che scrive risposte email professionali." },
                new { role = "user", content = $"Ecco una email ricevuta:\n\n{emailText}\n\nScrivi una risposta cortese e professionale." }
                },
                max_tokens = 500
            };

            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Errore API OpenAI: {response.StatusCode}\n{responseJson}");
            }

            using var doc = JsonDocument.Parse(responseJson);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return content ?? "(Nessuna risposta generata)";*/
        }
    }
}


