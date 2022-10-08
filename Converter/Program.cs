using CommonUtils.Services;
using CommonUtils.Services.Interfaces;
using Converter.Application.HostedServices;
using Converter.Application.Services;
using Converter.Application.Services.Interfaces;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddHostedService<HtmlReadyCheckHostedService>();
builder.Services.AddTransient<IPdfConverter, PdfConverter>();
builder.Services.AddSingleton<IFileSystem, FileSystem>();
builder.Services.AddSingleton<IStorage>(x =>
{
    var redisSection = builder.Configuration.GetSection("Redis");
    return new RedisStorage(
        Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING", EnvironmentVariableTarget.Process)
            ?? "localhost",
        x.GetRequiredService<IFileSystem>());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();