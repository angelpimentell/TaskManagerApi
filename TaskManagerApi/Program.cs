using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskManagerApi;
using TaskManagerApi.Data;
using TaskManagerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// JWT
builder.Services.AddSingleton<JwtTokenService>();
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// Add Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    // Enable JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer [your JWT token]' to authenticate."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

// Add SignalR
builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApiDocument();

builder.Services.AddExceptionHandler<AppExceptionHandler>();

// Customize validation error response
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        List<string> errorMessages = new List<string>();
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .Select(e => new
            {
                Field = e.Key,
                Errors = e.Value.Errors.Select(x => x.ErrorMessage)
            });

        foreach (var errorM in errors)
        {
            string rawError = errorM.Errors.First();

            // Shorten specific known verbose error messages
            if (rawError.Contains("missing required properties including", StringComparison.OrdinalIgnoreCase))
            {
                // Extract property names from the message, e.g. 'dueDate'
                var startIndex = rawError.IndexOf("including:", StringComparison.OrdinalIgnoreCase);
                if (startIndex >= 0)
                {
                    var propsPart = rawError.Substring(startIndex + "including:".Length).Trim();
                    // Remove trailing punctuation if any
                    propsPart = propsPart.Trim(new char[] { '.', ' ', '"' });
                    rawError = $"Missing required property: {propsPart}";
                }
                else
                {
                    rawError = "Missing required properties.";
                }
            }

            // Remove trailing dots for neatness
            rawError = rawError.TrimEnd('.');

            errorMessages.Add(rawError);
        }

        return new BadRequestObjectResult(new
        {
            Message = "",
            Success = false,
            Errors = errorMessages,
            StatusCode = 500,
        });
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<NotificationHub>("/notificationHub");

app.UseExceptionHandler(_ => { });

app.UseHttpsRedirection();

app.UseAuthentication(); // JWT

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { };