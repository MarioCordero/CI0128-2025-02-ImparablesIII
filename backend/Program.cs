using backend.Services;
using backend.Repositories;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Configure Email Settings from external configuration file
builder.Configuration.AddJsonFile("emailconfig.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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

// ===================================
// REPOSITORIES REGISTRATION
// ===================================

// Core repositories (base entities)
builder.Services.AddScoped<IDirectionRepository, DirectionRepository>();
builder.Services.AddScoped<IPersonaRepository, PersonaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Business repositories
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IEmployerRepository, EmployerRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IPasswordRepository, PasswordRepository>();
builder.Services.AddScoped<IProfileEmployeeRepository, ProfileEmployeeRepository>();
builder.Services.AddScoped<IBenefitRepository, BenefitRepository>();
builder.Services.AddScoped<IEmployeeBenefitRepository, EmployeeBenefitRepository>();
builder.Services.AddScoped<IHoursRepository, HoursRepository>();

// Core services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<backend.Services.IEmployerService, backend.Services.EmployerService>();
builder.Services.AddScoped<IProfileEmployeeService, ProfileEmployeeService>();
builder.Services.AddScoped<IBenefitService, BenefitService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEmployeeDeletionService, EmployeeDeletionService>();
builder.Services.AddScoped<IEmployeeBenefitService, EmployeeBenefitService>();
builder.Services.AddScoped<IBenefitDeductionsService, BenefitDeductionsService>();
builder.Services.AddScoped<IHoursService, HoursService>();

// Authentication & Security services
builder.Services.AddScoped<IPasswordSetupService, PasswordSetupService>();

// Email services
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailVerificationService, EmailVerificationService>();

// External API services
builder.Services.Configure<ExternalApiSettings>(builder.Configuration.GetSection("ExternalApiSettings"));

// Benefit calculation services
builder.Services.AddHttpClient<backend.Services.PaymentsCalculate.Benefits.PrivateInsurance>();
builder.Services.AddHttpClient<backend.Services.PaymentsCalculate.Benefits.VoluntaryPension>();
builder.Services.AddHttpClient<backend.Services.PaymentsCalculate.Benefits.SolidarityAssociation>();

// Infrastructure services
builder.Services.AddMemoryCache(); // Memory cache for password tokens

// Payroll services
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
builder.Services.AddScoped<backend.Services.PaymentsCalculate.EmployeeDeductionCalculatorFactory>();
builder.Services.AddScoped<backend.Services.PaymentsCalculate.EmployerDeductionCalculatorFactory>();
builder.Services.AddScoped<backend.Services.IBenefitCodeParser, backend.Services.BenefitCodeParser>();
builder.Services.AddScoped<IPayrollService, PayrollService>();

// Report generation services
builder.Services.AddScoped<backend.Services.IPdfBuilder, backend.Services.PayrollReportPdfBuilder>();
builder.Services.AddScoped<backend.Services.IReportGenerationService, backend.Services.ReportGenerationService>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // ASK
builder.Services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString)); // ASK

var app = builder.Build();

// ===================================
// MIDDLEWARE CONFIGURATION
// ===================================

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlaniFy API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowVueFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();