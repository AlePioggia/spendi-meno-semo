using DotNetEnv;
using Expenses.Application;
using Expenses.Infrastructure;
using Expenses.Infrastructure.persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5116");

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

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://host.docker.internal:8080/realms/myapp";
        //options.Authority = "http://localhost:8080/realms/myapp";
        options.RequireHttpsMetadata = false;
        options.RefreshOnIssuerKeyNotFound = true;

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerValidator = (issuer, token, parameters) =>
            {
                if (issuer == "http://localhost:8080/realms/myapp" || issuer == "http://host.docker.internal:8080/realms/myapp")
                    return issuer;
                throw new SecurityTokenInvalidIssuerException($"Invalid issuer: {issuer}");
            }
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine("JWT validation failed!");
                Console.WriteLine("Token: " + ctx.Request.Headers["Authorization"]);
                Console.WriteLine("Exception: " + ctx.Exception);
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                var kid = ctx.SecurityToken;
                Console.WriteLine("Token kid: " + kid.SigningKey);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<TransactionsDbContext>();
//    db.Database.Migrate();
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowAngularDev");
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();


app.Run();