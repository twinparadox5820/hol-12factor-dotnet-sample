// using Azure.Identity;
// using Azure.Security.KeyVault.Secrets;
// using Azure.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// builder.Services.AddDbContext<TFAppContext>(options =>
//     options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("TFAppContext")));
builder.Services.AddDbContext<TFAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TFAppContext")));

// builder.Services.AddDistributedMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = builder.Environment.EnvironmentName.ToLower();
});

// セッションの設定
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpClientの設定
builder.Services.AddHttpClient("weather", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://func-hol-12factor-app-training.azurewebsites.net/");
});

//HttpContextAccessorの設定
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

// SecretClientOptions options = new SecretClientOptions()
//     {
//         Retry =
//         {
//             Delay= TimeSpan.FromSeconds(2),
//             MaxDelay = TimeSpan.FromSeconds(16),
//             MaxRetries = 5,
//             Mode = RetryMode.Exponential
//          }
//     };
// var client = new SecretClient(new Uri("https://kv-hol-12factor-app.vault.azure.net/"), new DefaultAzureCredential(),options);

// KeyVaultSecret secret = client.GetSecret("AzureSQLDatabaseConnectionString");

// string secretValue = secret.Value;

// app.MapGet("/", () => secretValue);

app.MapRazorPages();

app.Run();
