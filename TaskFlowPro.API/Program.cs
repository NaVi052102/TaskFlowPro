using Microsoft.EntityFrameworkCore; // Import Entity Framework Core extension methods namespace
using TaskFlowPro.Infrastructure.Persistence; // Import AppDbContext namespace from the infrastructure layer
using FluentValidation; // Import FluentValidation namespace to enable assembly scanning extension methods
using FluentValidation.AspNetCore; // Import FluentValidation ASP.NET Core extension namespace
using Microsoft.AspNetCore.Authentication.JwtBearer; // Import JWT bearer authentication namespace
using Microsoft.IdentityModel.Tokens; // Import token validation parameters namespace
using TaskFlowPro.Infrastructure.Services;
using System.Text; // Import encoding namespace

var builder = WebApplication.CreateBuilder(args); // Initialize web application builder instance with command line arguments

// Register CORS policy to allow requests from any local React frontend development server port
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:5173", // Allow Vite dev server port 5173
                  "http://localhost:5174", // Allow Vite dev server port 5174
                  "http://localhost:5175", // Allow Vite dev server port 5175
                  "http://localhost:5177", // Allow Vite dev server port 5177 (Active Frontend Port)
                  "http://localhost:5178"  // Allow Vite dev server port 5178
                  
              ) // Allow common Vite dev server ports to prevent CORS blocks
              .AllowAnyHeader() // Allow incoming headers (like Authorization bearer tokens)
              .AllowAnyMethod(); // Allow all HTTP methods (GET, POST, PUT, DELETE, etc.)
    });
});

// Register MVC controllers, scan for API controllers, and configure JSON options to ignore circular reference loops
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // Ignore circular loops when serializing navigation properties to JSON
    });

// Retrieve JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings"); // Get JwtSettings configuration section
var secretKey = jwtSettings["Secret"] ?? "DefaultSuperSecretKeyForDevelopmentOnly123456789"; // Retrieve secret key or fallback
var keyBytes = Encoding.UTF8.GetBytes(secretKey); // Convert secret key string into UTF-8 byte array

// Configure JWT Bearer Authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Set default authentication scheme to JWT Bearer
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Set default challenge scheme to JWT Bearer
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters // Configure token validation validation rules
    {
        ValidateIssuer = true, // Ensure token issuer matches expected value
        ValidateAudience = true, // Ensure token audience matches expected value
        ValidateLifetime = true, // Ensure token expiration time is valid
        ValidateIssuerSigningKey = true, // Validate signature using the secret key
        ValidIssuer = jwtSettings["Issuer"], // Expected issuer string
        ValidAudience = jwtSettings["Audience"], // Expected audience string
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes) // Symmetric security key byte representation
    };
});

builder.Services.AddAuthorization(); // Register authorization services into dependency injection container

// Register AppDbContext with MySQL (Pomelo provider) and fetch connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // Retrieve MySQL connection string from configuration
builder.Services.AddDbContext<AppDbContext>(options => // Register AppDbContext database context service with lambda configuration options
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))); // Auto-detects your local MySQL version provider

builder.Services.AddScoped<TaskFlowPro.Application.Interfaces.IApplicationDbContext>(provider => provider.GetRequiredService<TaskFlowPro.Infrastructure.Persistence.AppDbContext>()); // Register IApplicationDbContext interface mapping to AppDbContext

// Register custom services for Dependency Injection (Scoped lifetime: created once per HTTP request)
builder.Services.AddScoped<TaskFlowPro.Application.Interfaces.IProjectService, TaskFlowPro.Application.Services.ProjectService>(); // Register project business logic service
builder.Services.AddScoped<TaskFlowPro.Application.Interfaces.ITaskService, TaskFlowPro.Application.Services.TaskService>(); // Register task business logic service
builder.Services.AddScoped<TaskFlowPro.Application.Interfaces.IUserService, TaskFlowPro.Application.Services.UserService>(); // Register user business logic service
builder.Services.AddScoped<TaskFlowPro.Application.Interfaces.ITokenService, TaskFlowPro.Infrastructure.Services.JwtTokenService>(); // Register JWT token generation service

// Register IEmailService and EmailService for dependency injection
builder.Services.AddScoped<IEmailService, EmailService>();

// Register all FluentValidation validators from the Application assembly container automatically
builder.Services.AddValidatorsFromAssemblyContaining<TaskFlowPro.Application.Validators.CreateTaskDtoValidator>(); // Scan validators assembly

// Enable automatic validation execution for incoming request payloads
builder.Services.AddFluentValidationAutoValidation(); // Register automatic model validation filter

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swag
builder.Services.AddEndpointsApiExplorer(); // Register API explorer endpoints
builder.Services.AddSwaggerGen(); // Register Swagger documentation generator

var app = builder.Build(); // Build the configured web application host instance

// Seed the database with initial records during application startup workflow
using (var scope = app.Services.CreateScope()) // Create an ephemeral DI scope to resolve scoped services during startup
{
    var services = scope.ServiceProvider; // Retrieve service provider from the created scope
    try
    {
        var dbContext = services.GetRequiredService<TaskFlowPro.Infrastructure.Persistence.AppDbContext>(); // Resolve AppDbContext instance
        await TaskFlowPro.Infrastructure.Persistence.DbInitializer.InitializeAsync(dbContext); // Invoke database initializer seeder method asynchronously
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>(); // Resolve logger instance for program startup
        logger.LogError(ex, "An error occurred while seeding the database."); // Log any unexpected seeding errors
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enables the Swagger JSON endpoint generator
    app.UseSwaggerUI(); // Enables the interactive web UI to test your API endpoints in the browser
}

// Enable CORS policy for the React frontend (Must be placed before routing/endpoints)
app.UseCors("AllowReactApp"); // Apply named CORS policy matching React frontend ports

// Register custom global exception handling middleware
app.UseMiddleware<TaskFlowPro.API.Middleware.ExceptionHandlingMiddleware>(); // Catch and format unhandled exceptions globally

app.UseHttpsRedirection(); // Forces HTTP requests to redirect to HTTPS for secure communication

app.UseAuthentication(); // Enables authentication middleware to validate JWT tokens on incoming requests
app.UseAuthorization(); // Enables authorization middleware (validates user roles and tokens if configured)

app.MapControllers(); // Maps incoming HTTP routes to your API controllers (e.g., ProjectsController, UsersController)

app.Run(); // Starts the web application server on port 5028