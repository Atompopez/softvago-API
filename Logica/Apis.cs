using System.Text;

namespace softvago_API.Logica
{
    public class JoobleBackgroundService : BackgroundService
    {
        //private readonly ILogger<JoobleBackgroundService> _logger;
        private readonly HttpClient _client;
        private const string ApiUrl = "https://jooble.org/api/";
        private const string ApiKey = "3f5453d5-ce7f-42c0-8bd1-ac74b5e9725b";

        public JoobleBackgroundService(ILogger<JoobleBackgroundService> logger)
        {
            //_logger = logger;
            _client = new HttpClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ConsultarJoobleAPIAsync();
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task ConsultarJoobleAPIAsync()
        {
            var jsonContent = "{ keywords: 'Developer', location: 'London'}";
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _client.PostAsync(ApiUrl + ApiKey, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                //_logger.LogInformation($"Respuesta de Jooble: {responseBody}");
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                //_logger.LogError($"Error al consultar la API de Jooble: {e.Message}");
                Console.WriteLine(e);
            }
        }
    }
}