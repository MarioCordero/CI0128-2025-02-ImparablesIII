using backend.Services;
using backend.Repositories;
using backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure Email Settings from external configuration file
builder.Configuration.AddJsonFile("emailconfig.json", optional: false, reloadOnChange: true);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:8080", "http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<EmployeeRepository>(); // Employee registration repository
builder.Services.AddScoped<IPasswordRepository, PasswordRepository>(); // Password operations repository
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register services
builder.Services.AddScoped<IEmailService, EmailService>(); // Email service
builder.Services.AddScoped<IPasswordSetupService, PasswordSetupService>(); // Password setup service
builder.Services.AddMemoryCache(); // Memory cache for password tokens
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

// Use CORS
app.UseCors("AllowVueFrontend");

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
