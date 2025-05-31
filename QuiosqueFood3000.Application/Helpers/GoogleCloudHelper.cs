using System.Text;
using System.Text.Json;

namespace QuiosqueFood3000.Api.Helpers
{
    public static class GoogleCloudHelper
    {
        public static async Task<string> GetBearerToken(string url)
        {
            var httpClient = new HttpClient();
            var payload = new
            {
                email = "victordomingos91@gmail.com",
                password = "123456",
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(
                "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyBcWXnfmt7rN3JQ-b14brXypwg9Wmeu7Ow",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao obter token: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            var responseBodyDeserialized = JsonSerializer.Deserialize<JsonDocument>(responseBody);

            if (responseBodyDeserialized == null ||
                !responseBodyDeserialized.RootElement.TryGetProperty("idToken", out var idTokenElement))
            {
                throw new Exception("idToken não encontrado na resposta.");
            }

            var idToken = idTokenElement.GetString();
            if (idToken == null)
            {
                throw new Exception("idToken é nulo.");
            }

            return idToken;
        }
    }
}
