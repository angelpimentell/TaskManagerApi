using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi;
using TaskManagerApi.Data;

var builder = WebApplication.CreateBuilder(args);

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
        var errorMessage = "";
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .Select(e => new
            {
                Field = e.Key,
                Errors = e.Value.Errors.Select(x => x.ErrorMessage)
            });

        errorMessage = string.Join(", ", errors.Select(e => e.Errors.ToList()[0].Replace(".", "")));

        return new BadRequestObjectResult(new
        {
            Message = errorMessage,
            Success = false,
            StatusCode = 400,
        });
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseOpenApi();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(_ => { });  

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
