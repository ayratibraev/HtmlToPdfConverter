using CommonUtils.Services;
using CommonUtils.Services.Interfaces;
using MediatR;
using Uploader.Application.HostedServices;
using Uploader.Application.Hub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddCors();
builder.Services.AddSignalR();

builder.Services.AddHostedService<PdfReadyCheckHostedService>();
builder.Services.AddSingleton<IStorage, RedisStorage>();
builder.Services.AddSingleton<IFileSystem, FileSystem>();

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

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials()); // allow credentials

app.MapHub<MessageHub>("/pdfReady");

app.Run();