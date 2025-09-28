using backend.Services;
using backend.Repositories;
using backend_lab_c28730.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load database configuration
builder.Configuration.AddJsonFile("dbconfig.json", optional: true);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();

// Add Entity Framework
builder.Services.AddDbContext<PlaniFyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PlaniFyDatabase")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            policy.WithOrigins("https://localhost:7030")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OrderRepository>(); // TEST

// Register the EmployerService
builder.Services.AddScoped<backend.Services.IEmployerService, backend.Services.EmployerService>();

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
