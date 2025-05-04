using Core.Dto;
using Core.Dto.Validators;
using Core.Interfaces;
using Core.Models;
using FluentValidation;
using Infrastructure.Context;
using Infrastructure.Data;
using Infrastructure.Data.Providers;
using MoolahTodo.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-9.0#cors-with-named-policy-and-middleware
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy  =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddScoped<ITodoDataProviderFactory, TodoDataProviderFactory>();
builder.Services.AddScoped<SqlServerTodoProvider>();
builder.Services.AddScoped<MongoDbTodoProvider>();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("Mongodb"));

builder.Services.AddScoped<IValidator<TodoDto>, TodoDtoValidator>();

builder.Services.AddDbContext<SqlServerDbContext>();
builder.Services.AddSingleton<MongoDbContext>();

var app = builder.Build();

app.MapTodoEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors(MyAllowSpecificOrigins);
}

app.UseHttpsRedirection();
app.Run();