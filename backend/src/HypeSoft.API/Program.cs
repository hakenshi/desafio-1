using AspNetCoreRateLimit;
using FluentValidation;
using HypeSoft.API.Middlewares;
using HypeSoft.Application.Behaviors;
using HypeSoft.Application.Interfaces;
using HypeSoft.Application.Mappings;
using HypeSoft.Application.Products.Validators;
using HypeSoft.Domain.Repositories;
using HypeSoft.Infraestructure.Caching;
using HypeSoft.Infraestructure.Data;
using HypeSoft.Infraestructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "HypeSoft:";
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

// Services
builder.Services.AddScoped<IKeycloakService, HypeSoft.Infraestructure.Services.KeycloakService>();

// MediatR with Validation, Caching and Cache Invalidation Behaviors
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(HypeSoft.Application.Products.Commands.CreateProductCommand).Assembly);
    cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
    cfg.AddOpenBehavior(typeof(CacheInvalidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

// HttpClient for Keycloak
builder.Services.AddHttpClient();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "HypeSoft API",
        Version = "v1",
        Description = @"API para gestão de produtos com Clean Architecture, CQRS e DDD.

**Funcionalidades:**
- Gestão completa de produtos (CRUD)
- Sistema de categorias
- Dashboard com métricas
- Controle de estoque
- Cache distribuído com Redis
- Rate limiting (100 req/min)

**Performance:**
- Tempo de resposta < 500ms
- Cache automático em queries
- Paginação eficiente",
        Contact = new()
        {
            Name = "HypeSoft",
            Email = "contato@hypesoft.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new()
    {
        Description = @"JWT Authorization header usando Bearer scheme.
                      
Entre com 'Bearer' [espaço] e então seu token.
                      
Exemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Enable XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Authentication with Keycloak
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = bool.Parse(builder.Configuration["Keycloak:RequireHttpsMetadata"] ?? "false");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                // Map Keycloak realm_access.roles to standard roles claim
                var claimsIdentity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                if (claimsIdentity == null) return Task.CompletedTask;

                // Get realm_access claim from the identity (already parsed by JWT handler)
                var realmAccessClaim = claimsIdentity.FindFirst("realm_access")?.Value;
                if (!string.IsNullOrEmpty(realmAccessClaim))
                {
                    try
                    {
                        var realmAccess = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(realmAccessClaim);
                        if (realmAccess.TryGetProperty("roles", out var rolesElement))
                        {
                            foreach (var role in rolesElement.EnumerateArray())
                            {
                                var roleValue = role.GetString();
                                if (!string.IsNullOrEmpty(roleValue))
                                {
                                    claimsIdentity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, roleValue));
                                }
                            }
                        }
                    }
                    catch { /* Ignore JSON parsing errors */ }
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<AspNetCoreRateLimit.IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.ClientIdHeader = "X-ClientId";
    options.GeneralRules = new List<AspNetCoreRateLimit.RateLimitRule>
    {
        new()
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 100
        },
        new()
        {
            Endpoint = "POST:*",
            Period = "1m",
            Limit = 30
        }
    };
});
// Rate limiting
builder.Services.AddSingleton<AspNetCoreRateLimit.IIpPolicyStore, AspNetCoreRateLimit.MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<AspNetCoreRateLimit.IRateLimitCounterStore, AspNetCoreRateLimit.MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<AspNetCoreRateLimit.IRateLimitConfiguration, AspNetCoreRateLimit.RateLimitConfiguration>();
builder.Services.AddSingleton<AspNetCoreRateLimit.IProcessingStrategy, AspNetCoreRateLimit.AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();

// Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

// Security headers
app.UseMiddleware<SecurityHeadersMiddleware>();

// Correlation ID for request tracking
app.UseMiddleware<CorrelationIdMiddleware>();

// Rate limiting
app.UseIpRateLimiting();

// Custom middleware for validation and error handling
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Initialize database (indexes and seed)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    
    // Create indexes for performance
    await MongoDbIndexes.CreateIndexesAsync(context);
    
    // Seed initial data
    await HypeSoft.API.Extensions.DatabaseSeeder.SeedAsync(context);
}

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
