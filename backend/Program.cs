using backend.Services;
using backend.Repositories;
using backend_lab_c28730.Models;
using backend_lab_c28730.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Email Settings from external configuration file
builder.Configuration.AddJsonFile("emailconfig.json", optional: false, reloadOnChange: true);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure email settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register services
builder.Services.AddScoped<OrderRepository>(); // TEST
builder.Services.AddScoped<IEmailService, EmailService>(); // Email service
builder.Services.AddHttpClient<AsociacionSolidaristaApiService>(); // Adding the AsociacionSolidaristaApiService to the builder


var app = builder.Build();

// DEVELOP ENABLED
// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// PRODUCTION ENABLED
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
