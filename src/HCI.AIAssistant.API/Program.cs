using Azure.Identity;
using HCI.AIAssistant.API.Managers;
using HCI.AIAssistant.API.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration
 [$"AppConfigurations{ConfigurationPath.KeyDelimiter}KeyVaultName"];
var secretsPrefix = builder.Configuration
 [$"AppConfigurations{ConfigurationPath.KeyDelimiter}SecretsPrefix"];
if (string.IsNullOrWhiteSpace(keyVaultName))
{
 throw new ArgumentNullException("KeyVaultName", "KeyVaultName is missing.");
}
if (string.IsNullOrWhiteSpace(secretsPrefix))
{
 throw new ArgumentNullException("SecretsPrefix", "SecretsPrefix is missing.");
}
var keyVaultUri = new Uri(
 $"https://{keyVaultName}.vault.azure.net/"
);
builder.Configuration.AddAzureKeyVault(
 keyVaultUri,
 new DefaultAzureCredential(),
 new CustomSecretManager(secretsPrefix)
);


// Configure values based on appsettings.json
builder.Services.Configure<SecretsService>(builder.Configuration.GetSection("Secrets"));
builder.Services.Configure<AppConfigurationsService>(builder.Configuration.GetSection("AppConfigurations"));
// Add services to the container.
builder.Services.AddSingleton<ISecretsService>(
provider => provider.GetRequiredService<IOptions<SecretsService>>().Value
);
builder.Services.AddSingleton<IAppConfigurationsService>(
provider => provider.GetRequiredService<IOptions<AppConfigurationsService>>().Value
);
builder.Services.AddSingleton<IParametricFunctions, ParametricFunctions>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Comment out the check so it runs in Azure (Production)
// if (app.Environment.IsDevelopment()) 
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Note: The lab manual implies adding Swagger to production or outside the IsDevelopment block 
// if you want to see it on Azure later, but follow the specific lines in the PDF carefully.
// The manual specifically shows app.UseSwagger() and UI usually inside Development check, 
// but implies we need it accessible.

app.UseAuthorization();

app.MapControllers();

app.Run();