using ExternalAPI.Services;
using ExternalAPI.Services.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Swagger documentation and configuration with Bearer token support
// Configure Swagger with Bearer Token Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PlaniFy API",
        Version = "v1",
        Description = "API for payroll management system"
    });

    // Configure Bearer Authentication for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// External API services registration
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAporteSolidaristaCalculatorService, AporteSolidaristaCalculatorService>();
builder.Services.AddHttpClient<IAsociacionSolidaristaApiService, AsociacionSolidaristaApiService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExternalAPI v1");
    c.RoutePrefix = ""; // Swagger on root
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();