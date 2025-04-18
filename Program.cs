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
