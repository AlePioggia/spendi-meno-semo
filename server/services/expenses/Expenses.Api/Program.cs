using DotNetEnv;
using Expenses.Application;
using Expenses.Api.middleware;
using Expenses.Infrastructure;
using Expenses.Infrastructure.scheduled;
using Expenses.Infrastructure.persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

bool isDevelopment = builder.Environment.IsDevelopment();

builder.Services.AddHybridCache();

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
    options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(120);
});

if (isDevelopment)
{
    builder.WebHost.UseUrls("http://localhost:5116");
}

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
    });
});


builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("per-ip", httpContext =>
    {
        var key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: key,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authorityFromConfig = builder.Configuration["Auth:Authority"]
            ?? Environment.GetEnvironmentVariable("AUTH_AUTHORITY");
        var defaultAuthority = isDevelopment
            ? "http://localhost:8080/realms/myapp"
            : "http://keycloak:8080/realms/myapp";

        options.Authority = string.IsNullOrWhiteSpace(authorityFromConfig) ? defaultAuthority : authorityFromConfig;
        options.RequireHttpsMetadata = options.Authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        options.RefreshOnIssuerKeyNotFound = true;

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(2),

            IssuerValidator = (issuer, token, parameters) =>
            {
                if (issuer.Contains("8080/realms/myapp"))
                    return issuer;
                throw new SecurityTokenInvalidIssuerException($"Invalid issuer: {issuer}");
            }
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine("JWT validation failed.");
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddHsts(options =>
{
    options.Preload = false;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(180);
});

if (isDevelopment)
{
    builder.Services.AddOpenApi();
}

builder.Services.AddApplication();

var connectionString = !isDevelopment
    ? Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
    : (Environment.GetEnvironmentVariable("LOCAL_CONNECTION_STRING")
        ?? Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING"));

builder.Services.AddInfrastructure(connectionString ?? "");

builder.Services.AddHostedService<RecurringTransactionsJob>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!isDevelopment)
{
    app.UseExceptionHandler(exceptionApp =>
    {
        exceptionApp.Run(context =>
            Results.Problem(title: "An unexpected error occurred.")
                .ExecuteAsync(context));
    });
}


app.UseForwardedHeaders();

var enableHttpsRedirection = builder.Configuration.GetValue("Security:EnableHttpsRedirection", false);
if (!isDevelopment && enableHttpsRedirection)
{
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

    await next();
});

if (!isDevelopment)
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<TransactionsDbContext>();
        db.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAngularDev");
app.UseSwagger();
app.UseSwaggerUI();

app.UseRateLimiter();
app.UseAuthentication();
app.UseMiddleware<ExecutionContextMiddleware>();
app.UseAuthorization();

if (enableHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.MapControllers().RequireRateLimiting("per-ip");


app.Run();