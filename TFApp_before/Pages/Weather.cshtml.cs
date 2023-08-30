namespace TFApp.Pages;

public class WeatherModel : PageModel
{
    private readonly TFAppContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RegisterModel> _logger;

    public WeatherModel(
        TFAppContext context,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<RegisterModel> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string? UserId { get; private set; }

    public Weather? Weather { get; set; }

    public static readonly string SessionKey = "UserId";

    public async Task OnGetAsync()
    {
        var session = _httpContextAccessor.HttpContext.Session;
        var key = session.GetString(SessionKey);
        UserId = key;

        if (_context.User != null)
        {
            // セッションと同じユーザーをDBから取得
            var user = await _context.User.FindAsync(key);

            // weather-apiをたたく
            if (user != null)
            {
                var client = _httpClientFactory.CreateClient("weather");
                client.DefaultRequestHeaders.Add("x-api-key", _configuration.GetValue<string>("ApiKey"));

                var response = await client.GetAsync($"api/weather/{user.City}");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    Weather = JsonSerializer.Deserialize<Weather>(responseBody);

                    _logger.LogInformation($"{DateTime.Now:F}: weather-apiのコールに成功しました\n");
                }
            }
            else
            {
                Weather = null;

                _logger.LogInformation($"{DateTime.Now:F}: ユーザーの登録処理が失敗しました\n");
            }
        }
    }
}
